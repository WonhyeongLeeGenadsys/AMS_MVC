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
    public class BYPASSVALVEBasicInfoRepository
    {
        // 가장 큰 BYPASSVALVE_CODE 값을 반환
        public string GetLatestBYPASSVALVECode()
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT MAX(BYPASSVALVE_CODE) FROM BYPASSVALVE_BASICINFO WHERE BYPASSVALVE_CODE LIKE 'BYPASSVALVE%'";
                return dbHelper.Conn.QuerySingleOrDefault<string>(query);
            }
        }

        public BYPASSVALVEBasicInfo GetBYPASSVALVEBasicInfoByTblIdxRepo(string tblIdx)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT * FROM BYPASSVALVE_BASICINFO WHERE TBL_IDX = @Tbl_Idx";
                return dbHelper.Conn.QueryFirstOrDefault<BYPASSVALVEBasicInfo>(query, new { Tbl_Idx = tblIdx });
            }
        }

        public BYPASSVALVEBasicInfo GetBYPASSVALVEBasicInfoByCode(string bypassvalveCode)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT * FROM BYPASSVALVE_BASICINFO WHERE BYPASSVALVE_CODE = @BYPASSVALVE_Code";
                return dbHelper.Conn.QueryFirstOrDefault<BYPASSVALVEBasicInfo>(query, new { BYPASSVALVE_Code = bypassvalveCode });
            }
        }

        /// <summary>
        /// BYPASSVALVE 기본정보 전체 불러오기
        /// </summary>
        public Result GetAllBYPASSVALVEBasicInfoRepo(out List<BYPASSVALVEBasicInfo> bypassvalveBasicInfo)
        {
            Result res = new Result(true);
            bypassvalveBasicInfo = new List<BYPASSVALVEBasicInfo>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = @"SELECT TBL_IDX, BYPASSVALVE_CODE, SERIAL_NO, NAME, INSTALL_DATE, OPERATING_DATE, PRICE, 
                                         INSTALL_PLACE, RATED_V, RATED_A, MAKE_COMPANY, MAKE_NO, PHOTO, IS_DIAGNOSTICS, 
                                         IS_HEALTH, WRITER, TBL_GETDATE 
                                  FROM BYPASSVALVE_BASICINFO";
                    bypassvalveBasicInfo = dbHelper.Conn.Query<BYPASSVALVEBasicInfo>(query).AsList();

                    LogHelper.WriteLog("bypassvalveBasicInfo Data", $"총 {bypassvalveBasicInfo.Count} 건 조회됨");
                    res.Message = "GetAllBYPASSVALVEBasicInfoRepo 동작 성공";
                    LogHelper.WriteLog("DB(BYPASSVALVE_BASICINFO)", res.Message);
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "GetAllBYPASSVALVEBasicInfoRepo 실패: " + ex.StackTrace + ex.Message;
                LogHelper.WriteLog("DB(BYPASSVALVE_BASICINFO)", res.Message);
            }
            return res;
        }

        //BYPASSVALVE Device페이지에서 '설비들' 표시하기 위해 사용 Basic 모델과 RiskMatrix HI 불러옴
        public Result GetAllBYPASSVALVEBasicInfoWithRiskMatrixRepo(out List<dynamic> bypassvalveInfoWithRisk)
        {
            Result res = new Result(true);
            bypassvalveInfoWithRisk = new List<dynamic>();
            
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = @"
                SELECT 
                    b.TBL_IDX, 
                    b.BYPASSVALVE_Code, 
                    b.Serial_No, 
                    b.Install_Date, 
                    b.Operating_Date, 
                    r.HI
                FROM BYPASSVALVE_BASICINFO b
                LEFT JOIN RISKMATRIX r ON b.BYPASSVALVE_Code = r.CODE
                ORDER BY b.TBL_IDX";
                    
                    bypassvalveInfoWithRisk = dbHelper.Conn.Query(query).AsList();
                }
                res.Message = "BYPASSVALVE 기본정보와 RISKMATRIX 정보 조회 성공";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = ex.Message;
            }
            return res;
        }

        public Result CreateBYPASSVALVEBasicInfoRepo(BYPASSVALVEBasicInfo newBYPASSVALVEBasicInfo)
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
                            // BYPASSVALVE_BASICINFO 테이블에 데이터 삽입
                            var queryBasicInfo = @"
                INSERT INTO BYPASSVALVE_BASICINFO (BYPASSVALVE_CODE, SERIAL_NO, NAME, INSTALL_DATE, OPERATING_DATE, PRICE, 
                                             INSTALL_PLACE, CAPACITY, RATED_V, RATED_A, MAKE_COMPANY, MAKE_NO, PHOTO, 
                                             IS_DIAGNOSTICS, IS_HEALTH, WRITER) 
                VALUES (@BYPASSVALVE_Code, @Serial_No, @Name, @Install_Date, @Operating_Date, @Price, @Install_Place, 
                        @Capacity, @Rated_V, @Rated_A, @Make_Company, @Make_No, @Photo, @Is_Diagnostics, 
                        @Is_Health, @Writer)";

                            int affectedRowsBasicInfo = conn.Execute(queryBasicInfo, newBYPASSVALVEBasicInfo, transaction);

                            if (affectedRowsBasicInfo > 0)
                            {
                                // RISKMATRIX 테이블에 데이터 삽입
                                var queryRiskMatrix = @"
                    INSERT INTO RISKMATRIX (CODE, COF, POF) 
                    VALUES (@BYPASSVALVE_Code, @DefaultCof, @DefaultPof)";

                                // 초기 COF와 POF 값은 기본값으로 설정 (필요시 변경)
                                var riskMatrixData = new
                                {
                                    BYPASSVALVE_Code = newBYPASSVALVEBasicInfo.BYPASSVALVE_Code,
                                    DefaultCof = "0",
                                    DefaultPof = "0"
                                };

                                int affectedRowsRiskMatrix = conn.Execute(queryRiskMatrix, riskMatrixData, transaction);

                                if (affectedRowsRiskMatrix > 0)
                                {
                                    transaction.Commit();
                                    res.Message = "CreateBYPASSVALVEBasicInfoRepo 성공: BYPASSVALVE Serial_No: " + newBYPASSVALVEBasicInfo.Serial_No;
                                    LogHelper.WriteLog("DB(BYPASSVALVE_BASICINFO)", res.Message);
                                }
                                else
                                {
                                    throw new Exception("RISKMATRIX 테이블에 데이터 삽입 실패");
                                }
                            }
                            else
                            {
                                throw new Exception("BYPASSVALVE_BASICINFO 테이블에 데이터 삽입 실패");
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            res.IsSuccess = false;
                            res.Message = "CreateBYPASSVALVEBasicInfoRepo 실패: " + ex.Message;
                            LogHelper.WriteLog("DB(BYPASSVALVE_BASICINFO)", "CreateBYPASSVALVEBasicInfoRepo 오류: " + ex.Message + " 스택트레이스: " + ex.StackTrace);
                        }
                    }
                }
            }
            return res;
        }

        public Result UpdateBYPASSVALVEBasicInfoRepo(BYPASSVALVEBasicInfo bypassvalveBasicInfo)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    // BYPASSVALVE_CODE를 기준으로 업데이트
                    var query = @"
            UPDATE BYPASSVALVE_BASICINFO
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
            WHERE BYPASSVALVE_CODE = @BYPASSVALVE_Code";

                    int affectedRows = dbHelper.Conn.Execute(query, bypassvalveBasicInfo);
                    if (affectedRows > 0)
                    {
                        res.Message = "UpdateBYPASSVALVEBasicInfoRepo 성공. BYPASSVALVE_CODE: " + bypassvalveBasicInfo.BYPASSVALVE_Code;
                        LogHelper.WriteLog("DB(BYPASSVALVE_BASICINFO)", res.Message);
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "UpdateBYPASSVALVEBasicInfoRepo 실패: 데이터 수정에 실패했습니다.";
                        LogHelper.WriteLog("DB(BYPASSVALVE_BASICINFO)", res.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "UpdateBYPASSVALVEBasicInfoRepo 실패: " + ex.StackTrace + ex.Message;
                LogHelper.WriteLog("DB(BYPASSVALVE_BASICINFO)", res.Message);
            }
            return res;
        }

        public Result DeleteBYPASSVALVEBasicInfoRepo(string tblIdx)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = "DELETE FROM BYPASSVALVE_BASICINFO WHERE TBL_IDX = @Tbl_Idx";
                    int affectedRows = dbHelper.Conn.Execute(query, new { Tbl_Idx = tblIdx });

                    if (affectedRows > 0)
                    {
                        res.Message = "DeleteBYPASSVALVEBasicInfoRepo 성공: BYPASSVALVEBasicInfo Tbl_Idx: " + tblIdx;
                        LogHelper.WriteLog("DB(BYPASSVALVE_BASICINFO)", res.Message);
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "DeleteBYPASSVALVEBasicInfoRepo 실패: 해당 Tbl_Idx를 찾을 수 없습니다.";
                        LogHelper.WriteLog("DB(BYPASSVALVE_BASICINFO)", res.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "DeleteBYPASSVALVEBasicInfoRepo 실패: " + ex.Message + "\n" + ex.StackTrace;
                LogHelper.WriteLog("DB(BYPASSVALVE_BASICINFO)", res.Message);
            }
            return res;
        }
    }
}
