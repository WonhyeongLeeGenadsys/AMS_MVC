
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Common
{
    public class SUBMODULEBasicInfoRepository
    {
        // 가장 큰 SUBMODULE_CODE 값을 반환
        public string GetLatestSUBMODULECode()
        {           
            using(DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT MAX(SUBMODULE_CODE) FROM SUBMODULE_BASICINFO WHERE SUBMODULE_CODE LIKE 'S%'";

                return dbHelper.Conn.QuerySingleOrDefault<string>(query);
            }
        }

        public SUBMODULEBasicInfo GetSUBMODULEBasicInfoByTblIdxRepo(string tblIdx)
        {
            using(DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT * FROM SUBMODULE_BASICINFO WHERE TBL_IDX = @Tbl_Idx";

                return dbHelper.Conn.QueryFirstOrDefault<SUBMODULEBasicInfo>(query, new { Tbl_Idx = tblIdx });
            }
        }

        public SUBMODULEBasicInfo GetSUBMODULEBasicInfoByCode(string submoduleCode)
        {
            using(DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT * FROM SUBMODULE_BASICINFO WHERE SUBMODULE_CODE = @SUBMODULE_Code";

                return dbHelper.Conn.QueryFirstOrDefault<SUBMODULEBasicInfo>(query, new { SUBMODULE_Code = submoduleCode });
            }
        }

        /// <summary>
        /// SUBMODULE 기본정보 전체 불러오기
        /// </summary>
        /// <param name="submoduleBasicInfo"></param>
        /// <returns></returns>
        public Result GetAllSUBMODULEBasicInfoRepo(out List<SUBMODULEBasicInfo> submoduleBasicInfo)
        {
            Result res = new Result(true);
            submoduleBasicInfo = new List<SUBMODULEBasicInfo>();

            try
            {
                using(DBHelper dbHelper = new DBHelper())
                {
                    var query = "SELECT TBL_IDX, SUBMODULE_CODE, SERIAL_NO, NAME, INSTALL_DATE, OPERATING_DATE, PRICE, INSTALL_PLACE, RATED_V, RATED_A, MAKE_COMPANY, MAKE_NO, IS_DIAGNOSTICS, IS_HEALTH, WRITER, TBL_GETDATE FROM SUBMODULE_BASICINFO";
                    submoduleBasicInfo = dbHelper.Conn.Query<SUBMODULEBasicInfo>(query).AsList();

                    LogHelper.WriteLog("submoduleBasicInfo Data", $"{submoduleBasicInfo}");
                    res.Message = "GetAllSUBMODULEBasicInfoRepo 동작 성공";
                    LogHelper.WriteLog("DB(SUBMODULE_BASICINFO", res.Message);
                }
            }
            catch (Exception ex)

            {
                res.IsSuccess = false;
                res.Message = "GetAllSUBMODULEBasicInfoRepo 실패: " + ex.StackTrace + ex.Message;

                LogHelper.WriteLog("DB(SUBMODULE_BASICINFO)", res.Message);
            }
            return res;
        }

        //SUBMODULE Device페이지에서 '설비들' 표시하기 위해 사용 Basic 모델과 RiskMatrix HI 불러옴
        public Result GetAllSUBMODULEBasicInfoWithRiskMatrixRepo(out List<dynamic> submoduleInfoWithRisk)
        {
            Result res = new Result(true);
            submoduleInfoWithRisk = new List<dynamic>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    // 각 CODE별로 LASTTIME 기준 최신 행만 뽑아서 JOIN
                    var query = @"
SELECT 
    b.TBL_IDX, 
    b.SUBMODULE_Code, 
    b.Serial_No, 
    b.Install_Date, 
    b.Operating_Date, 
    r_latest.HI
FROM SUBMODULE_BASICINFO b
LEFT JOIN (
    SELECT CODE, HI
    FROM (
        SELECT 
            CODE, 
            HI,
            ROW_NUMBER() OVER(PARTITION BY CODE ORDER BY LASTTIME DESC) AS rn
        FROM RISKMATRIX
    ) t
    WHERE t.rn = 1
) r_latest
    ON b.SUBMODULE_Code = r_latest.CODE
ORDER BY b.TBL_IDX;
";

                    submoduleInfoWithRisk = dbHelper.Conn.Query(query).AsList();
                }
                res.Message = "SUBMODULE 기본정보와 최신 RISKMATRIX 정보 조회 성공";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = ex.Message;
            }
            return res;
        }

        public Result CreateSUBMODULEBasicInfoRepo(SUBMODULEBasicInfo newSUBMODULEBasicInfo)
        {
            Result res = new Result(true);
            using(DBHelper dbHelper = new DBHelper())
            {
                using (var conn = dbHelper.Conn)
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // SUBMODULE_BASICINFO 테이블에 데이터 삽입
                            var queryBasicInfo = @"
                INSERT INTO SUBMODULE_BASICINFO (SUBMODULE_CODE, SERIAL_NO, NAME, INSTALL_DATE, OPERATING_DATE, PRICE, 
                INSTALL_PLACE, CAPACITY, RATED_V, RATED_A, MAKE_COMPANY, MAKE_NO, PHOTO, IS_DIAGNOSTICS, 
                IS_HEALTH, WRITER) 
                VALUES (@SUBMODULE_Code, @Serial_No, @Name, @Install_Date, @Operating_Date, @Price, @Install_Place, 
                @Capacity, @Rated_V, @Rated_A, @Make_Company, @Make_No, @Photo, @Is_Diagnostics, 
                @Is_Health, @Writer)";

                            int affectedRowsBasicInfo = conn.Execute(queryBasicInfo, newSUBMODULEBasicInfo, transaction);

                            if (affectedRowsBasicInfo > 0)
                            {
                                // RISKMATRIX 테이블에 데이터 삽입
                                var queryRiskMatrix = @"
                    INSERT INTO RISKMATRIX (CODE, COF, POF) 
                    VALUES (@SUBMODULE_Code, @DefaultCof, @DefaultPof)";

                                // 초기 COF와 POF 값은 기본값으로 설정 (변경해야됨)
                                var riskMatrixData = new
                                {
                                    SUBMODULE_Code = newSUBMODULEBasicInfo.SUBMODULE_Code,
                                    DefaultCof = "0",
                                    DefaultPof = "0"
                                };

                                int affectedRowsRiskMatrix = conn.Execute(queryRiskMatrix, riskMatrixData, transaction);

                                if (affectedRowsRiskMatrix > 0)
                                {
                                    // 트랜잭션 커밋
                                    transaction.Commit();
                                    res.Message = "CreateSUBMODULEBasicInfoRepo 성공: SUBMODULE Serial_No: " + newSUBMODULEBasicInfo.Serial_No;
                                    LogHelper.WriteLog("DB(SUBMODULE_BASICINFO)", res.Message);
                                }
                                else
                                {
                                    throw new Exception("RISKMATRIX 테이블에 데이터 삽입 실패");
                                }
                            }
                            else
                            {
                                throw new Exception("SUBMODULE_BASICINFO 테이블에 데이터 삽입 실패");
                            }
                        }
                        catch (Exception ex)
                        {
                            // 트랜잭션 롤백
                            transaction.Rollback();
                            res.IsSuccess = false;
                            res.Message = "CreateSUBMODULEBasicInfoRepo 실패: " + ex.Message;
                            LogHelper.WriteLog("DB(SUBMODULE_BASICINFO)", "CreateSUBMODULEBasicInfoRepo 오류: " + ex.Message + " 스택트레이스: " + ex.StackTrace);
                        }
                    }
                }
            }
            return res;
        }

        public Result UpdateSUBMODULEBasicInfoRepo(SUBMODULEBasicInfo submoduleBasicInfo)
        {
            Result res = new Result(true);
            try
            {
                using(DBHelper dbHelper = new DBHelper())
                {
                    var query = "UPDATE SUBMODULE_BASICINFO SET NAME = @Name, INSTALL_DATE = @Install_Date, OPERATING_DATE = @Operating_Date, PRICE=@Price, INSTALL_PLACE=@Install_Place, CAPACITY=@Capacity, RATED_V=@Rated_V, RATED_A=@Rated_A, MAKE_COMPANY=@Make_Company, MAKE_NO=@Make_No, PHOTO=@Photo, IS_DIAGNOSTICS=@Is_Diagnostics, IS_HEALTH=@Is_Health, WRITER=@Writer " +
            "WHERE SERIAL_NO = @Serial_No";

                    int affectedRows = dbHelper.Conn.Execute(query, submoduleBasicInfo);
                    if (affectedRows > 0)
                    {
                        res.Message = "UpdateSUBMODULEBasicInfoRepo 성공 SERIAL_NO: " + submoduleBasicInfo.Serial_No;
                        LogHelper.WriteLog("DB(SUBMODULE_BasicInfo)", res.Message);
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "UpdateSUBMODULEBasicInfoRepo 실패: 데이터 수정에 실패했습니다.";
                        LogHelper.WriteLog("DB(SUBMODULE_BasicInfo)", res.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "UpdateSUBMODULEBasicInfoRepo 실패: " + ex.StackTrace + ex.Message;
                LogHelper.WriteLog("DB(SUBMODULE_BasicInfo)", res.Message);
            }
            return res;
        }

        public Result DeleteSUBMODULEBasicInfoRepo(string tblIdx)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    // 1) 먼저 SUBMODULE_CODE 조회
                    const string getCodeQuery = "SELECT SUBMODULE_CODE FROM SUBMODULE_BASICINFO WHERE TBL_IDX = @Tbl_Idx";
                    var submoduleCode = dbHelper.Conn.QueryFirstOrDefault<string>(getCodeQuery, new { Tbl_Idx = tblIdx });

                    if (string.IsNullOrEmpty(submoduleCode))
                    {
                        res.IsSuccess = false;
                        res.Message = "DeleteSUBMODULEBasicInfoRepo 실패: 해당 Tbl_Idx의 장비를 찾을 수 없습니다.";
                        LogHelper.WriteLog("DB(SUBMODULE_BASICINFO)", res.Message);
                        return res;
                    }

                    // 2) SUBMODULE_BASICINFO 삭제
                    const string deleteBasicQuery = "DELETE FROM SUBMODULE_BASICINFO WHERE TBL_IDX = @Tbl_Idx";
                    int affectedRows = dbHelper.Conn.Execute(deleteBasicQuery, new { Tbl_Idx = tblIdx });

                    if (affectedRows > 0)
                    {
                        // 3) RISKMATRIX에서 해당 CODE 모든 행 삭제
                        const string deleteRiskQuery = "DELETE FROM RISKMATRIX WHERE CODE = @SUBMODULE_Code";
                        int riskDeleted = dbHelper.Conn.Execute(deleteRiskQuery, new { SUBMODULE_Code = submoduleCode });

                        res.Message = $"DeleteSUBMODULEBasicInfoRepo 성공: Tbl_Idx={tblIdx}, SUBMODULE_CODE={submoduleCode}, " +
                                      $"RISKMATRIX {riskDeleted}건 삭제됨";
                        LogHelper.WriteLog("DB(SUBMODULE_BASICINFO)", res.Message);
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "DeleteSUBMODULEBasicInfoRepo 실패: 해당 Tbl_Idx를 찾을 수 없습니다.";
                        LogHelper.WriteLog("DB(SUBMODULE_BASICINFO)", res.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "DeleteSUBMODULEBasicInfoRepo 오류: " + ex.Message + "\n" + ex.StackTrace;
                LogHelper.WriteLog("DB(SUBMODULE_BASICINFO)", res.Message);
            }

            return res;
        }
    }
}