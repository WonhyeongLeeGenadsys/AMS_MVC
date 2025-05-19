using AMS_MVC.Database;
using AMS_MVC.Models;
using AMS_MVC.Utlity;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Common.Log;

namespace AMS_MVC.Repositories
{
    public class ESBasicInfoRepository
    {
        // 가장 큰 ES_CODE 값을 반환
        public string GetLatestESCode()
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT MAX(ES_CODE) FROM ES_BASICINFO WHERE ES_CODE LIKE 'ES%'";
                return dbHelper.Conn.QuerySingleOrDefault<string>(query);
            }
        }

        public ESBasicInfo GetESBasicInfoByTblIdxRepo(string tblIdx)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT * FROM ES_BASICINFO WHERE TBL_IDX = @Tbl_Idx";
                return dbHelper.Conn.QueryFirstOrDefault<ESBasicInfo>(query, new { Tbl_Idx = tblIdx });
            }
        }

        public ESBasicInfo GetESBasicInfoByCode(string esCode)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT * FROM ES_BASICINFO WHERE ES_CODE = @ES_Code";
                return dbHelper.Conn.QueryFirstOrDefault<ESBasicInfo>(query, new { ES_Code = esCode });
            }
        }

        /// <summary>
        /// ES 기본정보 전체 불러오기
        /// </summary>
        public Result GetAllESBasicInfoRepo(out List<ESBasicInfo> esBasicInfo)
        {
            Result res = new Result(true);
            esBasicInfo = new List<ESBasicInfo>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = @"SELECT TBL_IDX, ES_CODE, SERIAL_NO, NAME, INSTALL_DATE, OPERATING_DATE, PRICE, 
                                         INSTALL_PLACE, RATED_V, RATED_A, MAKE_COMPANY, MAKE_NO, PHOTO, IS_DIAGNOSTICS, 
                                         IS_HEALTH, WRITER, TBL_GETDATE 
                                  FROM ES_BASICINFO";
                    esBasicInfo = dbHelper.Conn.Query<ESBasicInfo>(query).AsList();

                    LogHelper.WriteLog("esBasicInfo Data", $"총 {esBasicInfo.Count} 건 조회됨");
                    res.Message = "GetAllESBasicInfoRepo 동작 성공";
                    LogHelper.WriteLog("DB(ES_BASICINFO)", res.Message);
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "GetAllESBasicInfoRepo 실패: " + ex.StackTrace + ex.Message;
                LogHelper.WriteLog("DB(ES_BASICINFO)", res.Message);
            }
            return res;
        }

        //ES Device페이지에서 '설비들' 표시하기 위해 사용 Basic 모델과 RiskMatrix HI 불러옴
        public Result GetAllESBasicInfoWithRiskMatrixRepo(out List<dynamic> esInfoWithRisk)
        {
            Result res = new Result(true);
            esInfoWithRisk = new List<dynamic>();
            
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = @"
                SELECT 
                    b.TBL_IDX, 
                    b.ES_Code, 
                    b.Serial_No, 
                    b.Install_Date, 
                    b.Operating_Date, 
                    r.HI
                FROM ES_BASICINFO b
                LEFT JOIN RISKMATRIX r ON b.ES_Code = r.CODE
                ORDER BY b.TBL_IDX";
                    
                    esInfoWithRisk = dbHelper.Conn.Query(query).AsList();
                }
                res.Message = "ES 기본정보와 RISKMATRIX 정보 조회 성공";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = ex.Message;
            }
            return res;
        }

        public Result CreateESBasicInfoRepo(ESBasicInfo newESBasicInfo)
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
                            // ES_BASICINFO 테이블에 데이터 삽입
                            var queryBasicInfo = @"
                INSERT INTO ES_BASICINFO (ES_CODE, SERIAL_NO, NAME, INSTALL_DATE, OPERATING_DATE, PRICE, 
                                             INSTALL_PLACE, CAPACITY, RATED_V, RATED_A, MAKE_COMPANY, MAKE_NO, PHOTO, 
                                             IS_DIAGNOSTICS, IS_HEALTH, WRITER) 
                VALUES (@ES_Code, @Serial_No, @Name, @Install_Date, @Operating_Date, @Price, @Install_Place, 
                        @Capacity, @Rated_V, @Rated_A, @Make_Company, @Make_No, @Photo, @Is_Diagnostics, 
                        @Is_Health, @Writer)";

                            int affectedRowsBasicInfo = conn.Execute(queryBasicInfo, newESBasicInfo, transaction);

                            if (affectedRowsBasicInfo > 0)
                            {
                                // RISKMATRIX 테이블에 데이터 삽입
                                var queryRiskMatrix = @"
                    INSERT INTO RISKMATRIX (CODE, COF, POF) 
                    VALUES (@ES_Code, @DefaultCof, @DefaultPof)";

                                // 초기 COF와 POF 값은 기본값으로 설정 (필요시 변경)
                                var riskMatrixData = new
                                {
                                    ES_Code = newESBasicInfo.ES_Code,
                                    DefaultCof = "0",
                                    DefaultPof = "0"
                                };

                                int affectedRowsRiskMatrix = conn.Execute(queryRiskMatrix, riskMatrixData, transaction);

                                if (affectedRowsRiskMatrix > 0)
                                {
                                    transaction.Commit();
                                    res.Message = "CreateESBasicInfoRepo 성공: ES Serial_No: " + newESBasicInfo.Serial_No;
                                    LogHelper.WriteLog("DB(ES_BASICINFO)", res.Message);
                                }
                                else
                                {
                                    throw new Exception("RISKMATRIX 테이블에 데이터 삽입 실패");
                                }
                            }
                            else
                            {
                                throw new Exception("ES_BASICINFO 테이블에 데이터 삽입 실패");
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            res.IsSuccess = false;
                            res.Message = "CreateESBasicInfoRepo 실패: " + ex.Message;
                            LogHelper.WriteLog("DB(ES_BASICINFO)", "CreateESBasicInfoRepo 오류: " + ex.Message + " 스택트레이스: " + ex.StackTrace);
                        }
                    }
                }
            }
            return res;
        }

        public Result UpdateESBasicInfoRepo(ESBasicInfo esBasicInfo)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    // ES_CODE를 기준으로 업데이트
                    var query = @"
            UPDATE ES_BASICINFO
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
                WRITER = @Writer
            WHERE ES_CODE = @ES_Code";

                    int affectedRows = dbHelper.Conn.Execute(query, esBasicInfo);
                    if (affectedRows > 0)
                    {
                        res.Message = "UpdateESBasicInfoRepo 성공. ES_CODE: " + esBasicInfo.ES_Code;
                        LogHelper.WriteLog("DB(ES_BASICINFO)", res.Message);
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "UpdateESBasicInfoRepo 실패: 데이터 수정에 실패했습니다.";
                        LogHelper.WriteLog("DB(ES_BASICINFO)", res.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "UpdateESBasicInfoRepo 실패: " + ex.StackTrace + ex.Message;
                LogHelper.WriteLog("DB(ES_BASICINFO)", res.Message);
            }
            return res;
        }

        public Result DeleteESBasicInfoRepo(string tblIdx)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = "DELETE FROM ES_BASICINFO WHERE TBL_IDX = @Tbl_Idx";
                    int affectedRows = dbHelper.Conn.Execute(query, new { Tbl_Idx = tblIdx });

                    if (affectedRows > 0)
                    {
                        res.Message = "DeleteESBasicInfoRepo 성공: ESBasicInfo Tbl_Idx: " + tblIdx;
                        LogHelper.WriteLog("DB(ES_BASICINFO)", res.Message);
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "DeleteESBasicInfoRepo 실패: 해당 Tbl_Idx를 찾을 수 없습니다.";
                        LogHelper.WriteLog("DB(ES_BASICINFO)", res.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "DeleteESBasicInfoRepo 실패: " + ex.Message + "\n" + ex.StackTrace;
                LogHelper.WriteLog("DB(ES_BASICINFO)", res.Message);
            }
            return res;
        }
    }
}
