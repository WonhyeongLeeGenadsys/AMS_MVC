
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Common
{
    public class TANKBasicInfoRepository
    {
        // 가장 큰 TANK_CODE 값을 반환
        public string GetLatestTANKCode()
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT MAX(TANK_CODE) FROM TANK_BASICINFO WHERE TANK_CODE LIKE 'TANK%'";
                return dbHelper.Conn.QuerySingleOrDefault<string>(query);
            }
        }

        public TANKBasicInfo GetTANKBasicInfoByTblIdxRepo(string tblIdx)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT * FROM TANK_BASICINFO WHERE TBL_IDX = @Tbl_Idx";
                return dbHelper.Conn.QueryFirstOrDefault<TANKBasicInfo>(query, new { Tbl_Idx = tblIdx });
            }
        }

        public TANKBasicInfo GetTANKBasicInfoByCode(string tankCode)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT * FROM TANK_BASICINFO WHERE TANK_CODE = @TANK_Code";
                return dbHelper.Conn.QueryFirstOrDefault<TANKBasicInfo>(query, new { TANK_Code = tankCode });
            }
        }

        /// <summary>
        /// TANK 기본정보 전체 불러오기
        /// </summary>
        public Result GetAllTANKBasicInfoRepo(out List<TANKBasicInfo> tankBasicInfo)
        {
            Result res = new Result(true);
            tankBasicInfo = new List<TANKBasicInfo>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = @"SELECT TBL_IDX, TANK_CODE, SERIAL_NO, NAME, INSTALL_DATE, OPERATING_DATE, PRICE, 
                                         INSTALL_PLACE, RATED_V, RATED_A, MAKE_COMPANY, MAKE_NO, PHOTO, IS_DIAGNOSTICS, 
                                         IS_HEALTH, REMAIN_LIFE, WRITER, TBL_GETDATE 
                                  FROM TANK_BASICINFO";
                    tankBasicInfo = dbHelper.Conn.Query<TANKBasicInfo>(query).AsList();

                    LogHelper.WriteLog("tankBasicInfo Data", $"총 {tankBasicInfo.Count} 건 조회됨");
                    res.Message = "GetAllTANKBasicInfoRepo 동작 성공";
                    LogHelper.WriteLog("DB(TANK_BASICINFO)", res.Message);
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "GetAllTANKBasicInfoRepo 실패: " + ex.StackTrace + ex.Message;
                LogHelper.WriteLog("DB(TANK_BASICINFO)", res.Message);
            }
            return res;
        }

        //TANK Device페이지에서 '설비들' 표시하기 위해 사용 Basic 모델과 RiskMatrix HI 불러옴
        public Result GetAllTANKBasicInfoWithRiskMatrixRepo(out List<dynamic> tankInfoWithRisk)
        {
            Result res = new Result(true);
            tankInfoWithRisk = new List<dynamic>();
            
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = @"
                SELECT 
                    b.TBL_IDX, 
                    b.TANK_Code, 
                    b.Serial_No, 
                    b.Install_Date, 
                    b.Operating_Date, 
                    b.REMAIN_LIFE   AS Remain_Life, 
                    r.HI
                FROM TANK_BASICINFO b
                OUTER APPLY (
                    SELECT TOP (1) r.HI
                    FROM RISKMATRIX r
                    WHERE r.CODE = b.TANK_Code
                    ORDER BY r.LASTTIME DESC
                ) r
                ORDER BY b.TBL_IDX";
                    
                    tankInfoWithRisk = dbHelper.Conn.Query(query).AsList();
                }
                res.Message = "TANK 기본정보와 RISKMATRIX 정보 조회 성공";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = ex.Message;
            }
            return res;
        }

        public Result CreateTANKBasicInfoRepo(TANKBasicInfo newTANKBasicInfo)
        {
            Result res = new Result(true);
            using (DBHelper dbHelper = new DBHelper())
            {
                using (var conn = dbHelper.Conn)
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // TANK_BASICINFO 테이블에 데이터 삽입
                            var queryBasicInfo = @"
                INSERT INTO TANK_BASICINFO (TANK_CODE, SERIAL_NO, NAME, INSTALL_DATE, OPERATING_DATE, PRICE, 
                                             INSTALL_PLACE, CAPACITY, RATED_V, RATED_A, MAKE_COMPANY, MAKE_NO, PHOTO, 
                                             IS_DIAGNOSTICS, REMAIN_LIFE, IS_HEALTH, WRITER) 
                VALUES (@TANK_Code, @Serial_No, @Name, @Install_Date, @Operating_Date, @Price, @Install_Place, 
                        @Capacity, @Rated_V, @Rated_A, @Make_Company, @Make_No, @Photo, @Is_Diagnostics, 
                        @Is_Health, @Remain_Life, @Writer)";

                            int affectedRowsBasicInfo = conn.Execute(queryBasicInfo, newTANKBasicInfo, transaction);

                            if (affectedRowsBasicInfo > 0)
                            {
                                // RISKMATRIX 테이블에 데이터 삽입
                                var queryRiskMatrix = @"
                    INSERT INTO RISKMATRIX (CODE, COF, POF) 
                    VALUES (@TANK_Code, @DefaultCof, @DefaultPof)";

                                // 초기 COF와 POF 값은 기본값으로 설정 (필요시 변경)
                                var riskMatrixData = new
                                {
                                    TANK_Code = newTANKBasicInfo.TANK_Code,
                                    DefaultCof = "0",
                                    DefaultPof = "0"
                                };

                                int affectedRowsRiskMatrix = conn.Execute(queryRiskMatrix, riskMatrixData, transaction);

                                if (affectedRowsRiskMatrix > 0)
                                {
                                    transaction.Commit();
                                    res.Message = "CreateTANKBasicInfoRepo 성공: TANK Serial_No: " + newTANKBasicInfo.Serial_No;
                                    LogHelper.WriteLog("DB(TANK_BASICINFO)", res.Message);
                                }
                                else
                                {
                                    throw new Exception("RISKMATRIX 테이블에 데이터 삽입 실패");
                                }
                            }
                            else
                            {
                                throw new Exception("TANK_BASICINFO 테이블에 데이터 삽입 실패");
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            res.IsSuccess = false;
                            res.Message = "CreateTANKBasicInfoRepo 실패: " + ex.Message;
                            LogHelper.WriteLog("DB(TANK_BASICINFO)", "CreateTANKBasicInfoRepo 오류: " + ex.Message + " 스택트레이스: " + ex.StackTrace);
                        }
                    }
                }
            }
            return res;
        }

        public Result UpdateTANKBasicInfoRepo(TANKBasicInfo tankBasicInfo)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    // TANK_CODE를 기준으로 업데이트
                    var query = @"
            UPDATE TANK_BASICINFO
            SET 
                NAME = @Name, 
                INSTALL_DATE = @Install_Date, 
                OPERATING_DATE = @Operating_Date, 
                PRICE = @Price, 
                INSTALL_PLACE = @Install_Place, 
                CAPACITY = @Capacity, 
                RATED_V = @Rated_V, 
                RATED_A = @Rated_A, 
                MAKE_COMPANY = @Make_Company, 
                MAKE_NO = @Make_No, 
                PHOTO = @Photo, 
                IS_DIAGNOSTICS = @Is_Diagnostics, 
                IS_HEALTH = @Is_Health, 
                REMAIN_LIFE = @Remain_Life,
                WRITER = @Writer
            WHERE TANK_CODE = @TANK_Code";

                    int affectedRows = dbHelper.Conn.Execute(query, tankBasicInfo);
                    if (affectedRows > 0)
                    {
                        res.Message = "UpdateTANKBasicInfoRepo 성공. TANK_CODE: " + tankBasicInfo.TANK_Code;
                        LogHelper.WriteLog("DB(TANK_BASICINFO)", res.Message);
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "UpdateTANKBasicInfoRepo 실패: 데이터 수정에 실패했습니다.";
                        LogHelper.WriteLog("DB(TANK_BASICINFO)", res.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "UpdateTANKBasicInfoRepo 실패: " + ex.StackTrace + ex.Message;
                LogHelper.WriteLog("DB(TANK_BASICINFO)", res.Message);
            }
            return res;
        }

        public Result DeleteTANKBasicInfoRepo(string tblIdx)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = "DELETE FROM TANK_BASICINFO WHERE TBL_IDX = @Tbl_Idx";
                    int affectedRows = dbHelper.Conn.Execute(query, new { Tbl_Idx = tblIdx });

                    if (affectedRows > 0)
                    {
                        res.Message = "DeleteTANKBasicInfoRepo 성공: TANKBasicInfo Tbl_Idx: " + tblIdx;
                        LogHelper.WriteLog("DB(TANK_BASICINFO)", res.Message);
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "DeleteTANKBasicInfoRepo 실패: 해당 Tbl_Idx를 찾을 수 없습니다.";
                        LogHelper.WriteLog("DB(TANK_BASICINFO)", res.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "DeleteTANKBasicInfoRepo 실패: " + ex.Message + "\n" + ex.StackTrace;
                LogHelper.WriteLog("DB(TANK_BASICINFO)", res.Message);
            }
            return res;
        }
    }
}
