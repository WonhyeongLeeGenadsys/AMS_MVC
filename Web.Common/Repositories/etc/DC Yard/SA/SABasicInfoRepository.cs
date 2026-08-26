
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Common
{
    public class SABasicInfoRepository
    {
        // 가장 큰 SA_CODE 값을 반환
        public string GetLatestSACode()
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT MAX(SA_CODE) FROM SA_BASICINFO WHERE SA_CODE LIKE 'SA%'";
                return dbHelper.Conn.QuerySingleOrDefault<string>(query);
            }
        }

        public SABasicInfo GetSABasicInfoByTblIdxRepo(string tblIdx)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT * FROM SA_BASICINFO WHERE TBL_IDX = @Tbl_Idx";
                return dbHelper.Conn.QueryFirstOrDefault<SABasicInfo>(query, new { Tbl_Idx = tblIdx });
            }
        }

        public SABasicInfo GetSABasicInfoByCode(string saCode)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT * FROM SA_BASICINFO WHERE SA_CODE = @SA_Code";
                return dbHelper.Conn.QueryFirstOrDefault<SABasicInfo>(query, new { SA_Code = saCode });
            }
        }

        /// <summary>
        /// SA 기본정보 전체 불러오기
        /// </summary>
        public Result GetAllSABasicInfoRepo(out List<SABasicInfo> saBasicInfo)
        {
            Result res = new Result(true);
            saBasicInfo = new List<SABasicInfo>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = @"SELECT TBL_IDX, SA_CODE, SERIAL_NO, NAME, INSTALL_DATE, OPERATING_DATE, PRICE, 
                                         INSTALL_PLACE, RATED_V, RATED_A, MAKE_COMPANY, MAKE_NO, PHOTO, IS_DIAGNOSTICS, 
                                         IS_HEALTH, REMAIN_LIFE, WRITER, TBL_GETDATE 
                                  FROM SA_BASICINFO";
                    saBasicInfo = dbHelper.Conn.Query<SABasicInfo>(query).AsList();

                    LogHelper.WriteLog("saBasicInfo Data", $"총 {saBasicInfo.Count} 건 조회됨");
                    res.Message = "GetAllSABasicInfoRepo 동작 성공";
                    LogHelper.WriteLog("DB(SA_BASICINFO)", res.Message);
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "GetAllSABasicInfoRepo 실패: " + ex.StackTrace + ex.Message;
                LogHelper.WriteLog("DB(SA_BASICINFO)", res.Message);
            }
            return res;
        }

        //SA Device페이지에서 '설비들' 표시하기 위해 사용 Basic 모델과 RiskMatrix HI 불러옴
        public Result GetAllSABasicInfoWithRiskMatrixRepo(out List<dynamic> saInfoWithRisk)
        {
            Result res = new Result(true);
            saInfoWithRisk = new List<dynamic>();
            
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = @"
                SELECT 
                    b.TBL_IDX, 
                    b.SA_Code, 
                    b.Serial_No, 
                    b.Install_Date, 
                    b.Operating_Date, 
                    b.REMAIN_LIFE   AS Remain_Life, 
                    r.HI
                FROM SA_BASICINFO b
                OUTER APPLY (
                    SELECT TOP (1) r.HI
                    FROM RISKMATRIX r
                    WHERE r.CODE = b.SA_Code
                    ORDER BY r.LASTTIME DESC
                ) r
                ORDER BY b.TBL_IDX";
                    
                    saInfoWithRisk = dbHelper.Conn.Query(query).AsList();
                }
                res.Message = "SA 기본정보와 RISKMATRIX 정보 조회 성공";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = ex.Message;
            }
            return res;
        }

        public Result CreateSABasicInfoRepo(SABasicInfo newSABasicInfo)
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
                            // SA_BASICINFO 테이블에 데이터 삽입
                            var queryBasicInfo = @"
                INSERT INTO SA_BASICINFO (SA_CODE, SERIAL_NO, NAME, INSTALL_DATE, OPERATING_DATE, PRICE, 
                                             INSTALL_PLACE, CAPACITY, RATED_V, RATED_A, MAKE_COMPANY, MAKE_NO, PHOTO, 
                                             IS_DIAGNOSTICS, REMAIN_LIFE, IS_HEALTH, WRITER) 
                VALUES (@SA_Code, @Serial_No, @Name, @Install_Date, @Operating_Date, @Price, @Install_Place, 
                        @Capacity, @Rated_V, @Rated_A, @Make_Company, @Make_No, @Photo, @Is_Diagnostics, @Remain_Life
                        @Is_Health, @Writer)";

                            int affectedRowsBasicInfo = conn.Execute(queryBasicInfo, newSABasicInfo, transaction);

                            if (affectedRowsBasicInfo > 0)
                            {
                                // RISKMATRIX 테이블에 데이터 삽입
                                var queryRiskMatrix = @"
                    INSERT INTO RISKMATRIX (CODE, COF, POF) 
                    VALUES (@SA_Code, @DefaultCof, @DefaultPof)";

                                // 초기 COF와 POF 값은 기본값으로 설정 (필요시 변경)
                                var riskMatrixData = new
                                {
                                    SA_Code = newSABasicInfo.SA_Code,
                                    DefaultCof = "0",
                                    DefaultPof = "0"
                                };

                                int affectedRowsRiskMatrix = conn.Execute(queryRiskMatrix, riskMatrixData, transaction);

                                if (affectedRowsRiskMatrix > 0)
                                {
                                    transaction.Commit();
                                    res.Message = "CreateSABasicInfoRepo 성공: SA Serial_No: " + newSABasicInfo.Serial_No;
                                    LogHelper.WriteLog("DB(SA_BASICINFO)", res.Message);
                                }
                                else
                                {
                                    throw new Exception("RISKMATRIX 테이블에 데이터 삽입 실패");
                                }
                            }
                            else
                            {
                                throw new Exception("SA_BASICINFO 테이블에 데이터 삽입 실패");
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            res.IsSuccess = false;
                            res.Message = "CreateSABasicInfoRepo 실패: " + ex.Message;
                            LogHelper.WriteLog("DB(SA_BASICINFO)", "CreateSABasicInfoRepo 오류: " + ex.Message + " 스택트레이스: " + ex.StackTrace);
                        }
                    }
                }
            }
            return res;
        }

        public Result UpdateSABasicInfoRepo(SABasicInfo saBasicInfo)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    // SA_CODE를 기준으로 업데이트
                    var query = @"
            UPDATE SA_BASICINFO
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
                REMAIN_LIFE = @Remain_Life,
                IS_HEALTH = @Is_Health, 
                WRITER = @Writer
            WHERE SA_CODE = @SA_Code";

                    int affectedRows = dbHelper.Conn.Execute(query, saBasicInfo);
                    if (affectedRows > 0)
                    {
                        res.Message = "UpdateSABasicInfoRepo 성공. SA_CODE: " + saBasicInfo.SA_Code;
                        LogHelper.WriteLog("DB(SA_BASICINFO)", res.Message);
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "UpdateSABasicInfoRepo 실패: 데이터 수정에 실패했습니다.";
                        LogHelper.WriteLog("DB(SA_BASICINFO)", res.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "UpdateSABasicInfoRepo 실패: " + ex.StackTrace + ex.Message;
                LogHelper.WriteLog("DB(SA_BASICINFO)", res.Message);
            }
            return res;
        }

        public Result DeleteSABasicInfoRepo(string tblIdx)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = "DELETE FROM SA_BASICINFO WHERE TBL_IDX = @Tbl_Idx";
                    int affectedRows = dbHelper.Conn.Execute(query, new { Tbl_Idx = tblIdx });

                    if (affectedRows > 0)
                    {
                        res.Message = "DeleteSABasicInfoRepo 성공: SABasicInfo Tbl_Idx: " + tblIdx;
                        LogHelper.WriteLog("DB(SA_BASICINFO)", res.Message);
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "DeleteSABasicInfoRepo 실패: 해당 Tbl_Idx를 찾을 수 없습니다.";
                        LogHelper.WriteLog("DB(SA_BASICINFO)", res.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "DeleteSABasicInfoRepo 실패: " + ex.Message + "\n" + ex.StackTrace;
                LogHelper.WriteLog("DB(SA_BASICINFO)", res.Message);
            }
            return res;
        }
    }
}
