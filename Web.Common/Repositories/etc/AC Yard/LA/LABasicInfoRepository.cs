
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Common
{
    public class LABasicInfoRepository
    {
        // 가장 큰 LA_CODE 값을 반환
        public string GetLatestLACode()
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT MAX(LA_CODE) FROM LA_BASICINFO WHERE LA_CODE LIKE 'LA%'";
                return dbHelper.Conn.QuerySingleOrDefault<string>(query);
            }
        }

        public LABasicInfo GetLABasicInfoByTblIdxRepo(string tblIdx)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT * FROM LA_BASICINFO WHERE TBL_IDX = @Tbl_Idx";
                return dbHelper.Conn.QueryFirstOrDefault<LABasicInfo>(query, new { Tbl_Idx = tblIdx });
            }
        }

        public LABasicInfo GetLABasicInfoByCode(string laCode)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT * FROM LA_BASICINFO WHERE LA_CODE = @LA_Code";
                return dbHelper.Conn.QueryFirstOrDefault<LABasicInfo>(query, new { LA_Code = laCode });
            }
        }

        /// <summary>
        /// LA 기본정보 전체 불러오기
        /// </summary>
        public Result GetAllLABasicInfoRepo(out List<LABasicInfo> laBasicInfo)
        {
            Result res = new Result(true);
            laBasicInfo = new List<LABasicInfo>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = @"SELECT TBL_IDX, LA_CODE, SERIAL_NO, NAME, INSTALL_DATE, OPERATING_DATE, PRICE, 
                                         INSTALL_PLACE, RATED_V, RATED_A, MAKE_COMPANY, MAKE_NO, PHOTO, IS_DIAGNOSTICS, 
                                         IS_HEALTH, REMAIN_LIFE, WRITER, TBL_GETDATE 
                                  FROM LA_BASICINFO";
                    laBasicInfo = dbHelper.Conn.Query<LABasicInfo>(query).AsList();

                    LogHelper.WriteLog("laBasicInfo Data", $"총 {laBasicInfo.Count} 건 조회됨");
                    res.Message = "GetAllLABasicInfoRepo 동작 성공";
                    LogHelper.WriteLog("DB(LA_BASICINFO)", res.Message);
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "GetAllLABasicInfoRepo 실패: " + ex.StackTrace + ex.Message;
                LogHelper.WriteLog("DB(LA_BASICINFO)", res.Message);
            }
            return res;
        }

        //LA Device페이지에서 '설비들' 표시하기 위해 사용 Basic 모델과 RiskMatrix HI 불러옴
        public Result GetAllLABasicInfoWithRiskMatrixRepo(out List<dynamic> laInfoWithRisk)
        {
            Result res = new Result(true);
            laInfoWithRisk = new List<dynamic>();
            
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = @"
                SELECT 
                    b.TBL_IDX, 
                    b.LA_Code, 
                    b.Serial_No, 
                    b.Install_Date, 
                    b.Operating_Date, 
                    b.REMAIN_LIFE   AS Remain_Life,
                    r.HI
                FROM LA_BASICINFO b
                LEFT JOIN RISKMATRIX r ON b.LA_Code = r.CODE
                ORDER BY b.TBL_IDX";
                    
                    laInfoWithRisk = dbHelper.Conn.Query(query).AsList();
                }
                res.Message = "LA 기본정보와 RISKMATRIX 정보 조회 성공";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = ex.Message;
            }
            return res;
        }

        public Result CreateLABasicInfoRepo(LABasicInfo newLABasicInfo)
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
                            // LA_BASICINFO 테이블에 데이터 삽입
                            var queryBasicInfo = @"
                INSERT INTO LA_BASICINFO (LA_CODE, SERIAL_NO, NAME, INSTALL_DATE, OPERATING_DATE, PRICE, 
                                             INSTALL_PLACE, CAPACITY, RATED_V, RATED_A, MAKE_COMPANY, MAKE_NO, PHOTO, 
                                             IS_DIAGNOSTICS, IS_HEALTH, REMAIN_LIFE, WRITER) 
                VALUES (@LA_Code, @Serial_No, @Name, @Install_Date, @Operating_Date, @Price, @Install_Place, 
                        @Capacity, @Rated_V, @Rated_A, @Make_Company, @Make_No, @Photo, @Is_Diagnostics, 
                        @Is_Health, @Remain_Life, @Writer)";

                            int affectedRowsBasicInfo = conn.Execute(queryBasicInfo, newLABasicInfo, transaction);

                            if (affectedRowsBasicInfo > 0)
                            {
                                // RISKMATRIX 테이블에 데이터 삽입
                                var queryRiskMatrix = @"
                    INSERT INTO RISKMATRIX (CODE, COF, POF) 
                    VALUES (@LA_Code, @DefaultCof, @DefaultPof)";

                                // 초기 COF와 POF 값은 기본값으로 설정 (필요시 변경)
                                var riskMatrixData = new
                                {
                                    LA_Code = newLABasicInfo.LA_Code,
                                    DefaultCof = "0",
                                    DefaultPof = "0"
                                };

                                int affectedRowsRiskMatrix = conn.Execute(queryRiskMatrix, riskMatrixData, transaction);

                                if (affectedRowsRiskMatrix > 0)
                                {
                                    transaction.Commit();
                                    res.Message = "CreateLABasicInfoRepo 성공: LA Serial_No: " + newLABasicInfo.Serial_No;
                                    LogHelper.WriteLog("DB(LA_BASICINFO)", res.Message);
                                }
                                else
                                {
                                    throw new Exception("RISKMATRIX 테이블에 데이터 삽입 실패");
                                }
                            }
                            else
                            {
                                throw new Exception("LA_BASICINFO 테이블에 데이터 삽입 실패");
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            res.IsSuccess = false;
                            res.Message = "CreateLABasicInfoRepo 실패: " + ex.Message;
                            LogHelper.WriteLog("DB(LA_BASICINFO)", "CreateLABasicInfoRepo 오류: " + ex.Message + " 스택트레이스: " + ex.StackTrace);
                        }
                    }
                }
            }
            return res;
        }

        public Result UpdateLABasicInfoRepo(LABasicInfo laBasicInfo)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    // LA_CODE를 기준으로 업데이트
                    var query = @"
            UPDATE LA_BASICINFO
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
            WHERE LA_CODE = @LA_Code";

                    int affectedRows = dbHelper.Conn.Execute(query, laBasicInfo);
                    if (affectedRows > 0)
                    {
                        res.Message = "UpdateLABasicInfoRepo 성공. LA_CODE: " + laBasicInfo.LA_Code;
                        LogHelper.WriteLog("DB(LA_BASICINFO)", res.Message);
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "UpdateLABasicInfoRepo 실패: 데이터 수정에 실패했습니다.";
                        LogHelper.WriteLog("DB(LA_BASICINFO)", res.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "UpdateLABasicInfoRepo 실패: " + ex.StackTrace + ex.Message;
                LogHelper.WriteLog("DB(LA_BASICINFO)", res.Message);
            }
            return res;
        }

        public Result DeleteLABasicInfoRepo(string tblIdx)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = "DELETE FROM LA_BASICINFO WHERE TBL_IDX = @Tbl_Idx";
                    int affectedRows = dbHelper.Conn.Execute(query, new { Tbl_Idx = tblIdx });

                    if (affectedRows > 0)
                    {
                        res.Message = "DeleteLABasicInfoRepo 성공: LABasicInfo Tbl_Idx: " + tblIdx;
                        LogHelper.WriteLog("DB(LA_BASICINFO)", res.Message);
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "DeleteLABasicInfoRepo 실패: 해당 Tbl_Idx를 찾을 수 없습니다.";
                        LogHelper.WriteLog("DB(LA_BASICINFO)", res.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "DeleteLABasicInfoRepo 실패: " + ex.Message + "\n" + ex.StackTrace;
                LogHelper.WriteLog("DB(LA_BASICINFO)", res.Message);
            }
            return res;
        }
    }
}
