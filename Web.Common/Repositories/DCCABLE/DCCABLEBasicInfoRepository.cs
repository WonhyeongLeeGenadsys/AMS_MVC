
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Common
{
    public class DCCABLEBasicInfoRepository
    {
        // 가장 큰 DCCABLE_CODE 값을 반환
        public string GetLatestDCCABLECode()
        {           
            using(DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT MAX(DCCABLE_CODE) FROM DCCABLE_BASICINFO WHERE DCCABLE_CODE LIKE 'DC%'";

                return dbHelper.Conn.QuerySingleOrDefault<string>(query);
            }
        }

        public DCCABLEBasicInfo GetDCCABLEBasicInfoByTblIdxRepo(string tblIdx)
        {
            using(DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT * FROM DCCABLE_BASICINFO WHERE TBL_IDX = @Tbl_Idx";

                return dbHelper.Conn.QueryFirstOrDefault<DCCABLEBasicInfo>(query, new { Tbl_Idx = tblIdx });
            }
        }

        public DCCABLEBasicInfo GetDCCABLEBasicInfoByCode(string dccableCode)
        {
            using(DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT * FROM DCCABLE_BASICINFO WHERE DCCABLE_CODE = @DCCABLE_Code";

                return dbHelper.Conn.QueryFirstOrDefault<DCCABLEBasicInfo>(query, new { DCCABLE_Code = dccableCode });
            }
        }

        /// <summary>
        /// DCCABLE 기본정보 전체 불러오기
        /// </summary>
        /// <param name="dccableBasicInfo"></param>
        /// <returns></returns>
        public Result GetAllDCCABLEBasicInfoRepo(out List<DCCABLEBasicInfo> dccableBasicInfo)
        {
            Result res = new Result(true);
            dccableBasicInfo = new List<DCCABLEBasicInfo>();

            try
            {
                using(DBHelper dbHelper = new DBHelper())
                {
                    var query = "SELECT TBL_IDX, DCCABLE_CODE, SERIAL_NO, NAME, INSTALL_DATE, OPERATING_DATE, PRICE, INSTALL_PLACE, RATED_V, RATED_A, MAKE_COMPANY, MAKE_NO, IS_DIAGNOSTICS, IS_HEALTH, WRITER, TBL_GETDATE FROM DCCABLE_BASICINFO";
                    dccableBasicInfo = dbHelper.Conn.Query<DCCABLEBasicInfo>(query).AsList();

                    LogHelper.WriteLog("dccableBasicInfo Data", $"{dccableBasicInfo}");
                    res.Message = "GetAllDCCABLEBasicInfoRepo 동작 성공";
                    LogHelper.WriteLog("DB(DCCABLE_BASICINFO", res.Message);
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "GetAllDCCABLEBasicInfoRepo 실패: " + ex.StackTrace + ex.Message;

                LogHelper.WriteLog("DB(DCCABLE_BASICINFO)", res.Message);
            }
            return res;
        }

        //DCCABLE Device페이지에서 '설비들' 표시하기 위해 사용 Basic 모델과 RiskMatrix HI 불러옴
        public Result GetAllDCCABLEBasicInfoWithRiskMatrixRepo(out List<dynamic> dccableInfoWithRisk)
        {
            Result res = new Result(true);
            dccableInfoWithRisk = new List<dynamic>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = @"
                SELECT 
                    b.TBL_IDX, 
                    b.DCCABLE_Code, 
                    b.Serial_No, 
                    b.Install_Date, 
                    b.Operating_Date, 
                    r.HI
                FROM DCCABLE_BASICINFO b
                LEFT JOIN RISKMATRIX r ON b.DCCABLE_Code = r.CODE
                ORDER BY b.TBL_IDX";

                    dccableInfoWithRisk = dbHelper.Conn.Query(query).AsList();
                }
                res.Message = "DCCABLE 기본정보와 RISKMATRIX 정보 조회 성공";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = ex.Message;
            }
            return res;
        }

        public Result CreateDCCABLEBasicInfoRepo(DCCABLEBasicInfo newDCCABLEBasicInfo)
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
                            // DCCABLE_BASICINFO 테이블에 데이터 삽입
                            var queryBasicInfo = @"
                INSERT INTO DCCABLE_BASICINFO (DCCABLE_CODE, SERIAL_NO, NAME, INSTALL_DATE, OPERATING_DATE, PRICE, 
                INSTALL_PLACE, CAPACITY, RATED_V, RATED_A, MAKE_COMPANY, MAKE_NO, PHOTO, IS_DIAGNOSTICS, 
                IS_HEALTH, WRITER) 
                VALUES (@DCCABLE_Code, @Serial_No, @Name, @Install_Date, @Operating_Date, @Price, @Install_Place, 
                @Capacity, @Rated_V, @Rated_A, @Make_Company, @Make_No, @Photo, @Is_Diagnostics, 
                @Is_Health, @Writer)";

                            int affectedRowsBasicInfo = conn.Execute(queryBasicInfo, newDCCABLEBasicInfo, transaction);

                            if (affectedRowsBasicInfo > 0)
                            {
                                // RISKMATRIX 테이블에 데이터 삽입
                                var queryRiskMatrix = @"
                    INSERT INTO RISKMATRIX (CODE, COF, POF) 
                    VALUES (@DCCABLE_Code, @DefaultCof, @DefaultPof)";

                                // 초기 COF와 POF 값은 기본값으로 설정 (변경해야됨)
                                var riskMatrixData = new
                                {
                                    DCCABLE_Code = newDCCABLEBasicInfo.DCCABLE_Code,
                                    DefaultCof = "0",
                                    DefaultPof = "0"
                                };

                                int affectedRowsRiskMatrix = conn.Execute(queryRiskMatrix, riskMatrixData, transaction);

                                if (affectedRowsRiskMatrix > 0)
                                {
                                    // 트랜잭션 커밋
                                    transaction.Commit();
                                    res.Message = "CreateDCCABLEBasicInfoRepo 성공: DCCABLE Serial_No: " + newDCCABLEBasicInfo.Serial_No;
                                    LogHelper.WriteLog("DB(DCCABLE_BASICINFO)", res.Message);
                                }
                                else
                                {
                                    throw new Exception("RISKMATRIX 테이블에 데이터 삽입 실패");
                                }
                            }
                            else
                            {
                                throw new Exception("DCCABLE_BASICINFO 테이블에 데이터 삽입 실패");
                            }
                        }
                        catch (Exception ex)
                        {
                            // 트랜잭션 롤백
                            transaction.Rollback();
                            res.IsSuccess = false;
                            res.Message = "CreateDCCABLEBBasicInfoRepo 실패: " + ex.Message;
                            LogHelper.WriteLog("DB(DCCABLE_BASICINFO)", "CreateDCCABLEBasicInfoRepo 오류: " + ex.Message + " 스택트레이스: " + ex.StackTrace);
                        }
                    }
                }
            }
            return res;
        }

        public Result UpdateDCCABLEBasicInfoRepo(DCCABLEBasicInfo dccableBasicInfo)
        {
            Result res = new Result(true);
            try
            {
                using(DBHelper dbHelper = new DBHelper())
                {
                    var query = "UPDATE DCCABLE_BASICINFO SET NAME = @Name, INSTALL_DATE = @Install_Date, OPERATING_DATE = @Operating_Date, PRICE=@Price, INSTALL_PLACE=@Install_Place, CAPACITY=@Capacity, RATED_V=@Rated_V, RATED_A=@Rated_A, MAKE_COMPANY=@Make_Company, MAKE_NO=@Make_No, PHOTO=@Photo, IS_DIAGNOSTICS=@Is_Diagnostics, IS_HEALTH=@Is_Health, WRITER=@Writer " +
            "WHERE SERIAL_NO = @Serial_No";

                    int affectedRows = dbHelper.Conn.Execute(query, dccableBasicInfo);
                    if (affectedRows > 0)
                    {
                        res.Message = "UpdateDCCABLEBasicInfoRepo 성공 SERIAL_NO: " + dccableBasicInfo.Serial_No;
                        LogHelper.WriteLog("DB(DCCABLE_BasicInfo)", res.Message);
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "UpdateDCCABLEBasicInfoRepo 실패: 데이터 수정에 실패했습니다.";
                        LogHelper.WriteLog("DB(DCCABLE_BasicInfo)", res.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "UpdateDCCABLEBasicInfoRepo 실패: " + ex.StackTrace + ex.Message;
                LogHelper.WriteLog("DB(DCCABLE_BasicInfo)", res.Message);
            }
            return res;
        }

        public Result DeleteDCCABLEBasicInfoRepo(string tblIdx)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    // 1) 먼저 DCCABLE_CODE 조회
                    const string getCodeQuery = "SELECT DCCABLE_CODE FROM DCCABLE_BASICINFO WHERE TBL_IDX = @Tbl_Idx";
                    var dccableCode = dbHelper.Conn.QueryFirstOrDefault<string>(getCodeQuery, new { Tbl_Idx = tblIdx });

                    if (string.IsNullOrEmpty(dccableCode))
                    {
                        res.IsSuccess = false;
                        res.Message = "DeleteDCCABLEBasicInfoRepo 실패: 해당 Tbl_Idx의 장비를 찾을 수 없습니다.";
                        LogHelper.WriteLog("DB(DCCABLE_BASICINFO)", res.Message);
                        return res;
                    }

                    // 2) DCCABLE_BASICINFO 삭제
                    const string deleteBasicQuery = "DELETE FROM DCCABLE_BASICINFO WHERE TBL_IDX = @Tbl_Idx";
                    int affectedRows = dbHelper.Conn.Execute(deleteBasicQuery, new { Tbl_Idx = tblIdx });

                    if (affectedRows > 0)
                    {
                        // 3) RISKMATRIX에서 해당 CODE 모든 행 삭제
                        const string deleteRiskQuery = "DELETE FROM RISKMATRIX WHERE CODE = @DCCABLE_Code";
                        int riskDeleted = dbHelper.Conn.Execute(deleteRiskQuery, new { DCCABLE_Code = dccableCode });

                        res.Message = $"DeleteDCCABLEBasicInfoRepo 성공: Tbl_Idx={tblIdx}, DCCABLE_CODE={dccableCode}, " +
                                      $"RISKMATRIX {riskDeleted}건 삭제됨";
                        LogHelper.WriteLog("DB(DCCABLE_BASICINFO)", res.Message);
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "DeleteDCCABLEBasicInfoRepo 실패: 해당 Tbl_Idx를 찾을 수 없습니다.";
                        LogHelper.WriteLog("DB(DCCABLE_BASICINFO)", res.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "DeleteDCCABLEBasicInfoRepo 오류: " + ex.Message + "\n" + ex.StackTrace;
                LogHelper.WriteLog("DB(DCCABLE_BASICINFO)", res.Message);
            }

            return res;
        }
    }
}