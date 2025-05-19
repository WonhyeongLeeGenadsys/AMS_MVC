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
    public class HEATEXCHANGERBasicInfoRepository
    {
        // 가장 큰 HEATEXCHANGER_CODE 값을 반환
        public string GetLatestHEATEXCHANGERCode()
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT MAX(HEATEXCHANGER_CODE) FROM HEATEXCHANGER_BASICINFO WHERE HEATEXCHANGER_CODE LIKE 'HEATEXCHANGER%'";
                return dbHelper.Conn.QuerySingleOrDefault<string>(query);
            }
        }

        public HEATEXCHANGERBasicInfo GetHEATEXCHANGERBasicInfoByTblIdxRepo(string tblIdx)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT * FROM HEATEXCHANGER_BASICINFO WHERE TBL_IDX = @Tbl_Idx";
                return dbHelper.Conn.QueryFirstOrDefault<HEATEXCHANGERBasicInfo>(query, new { Tbl_Idx = tblIdx });
            }
        }

        public HEATEXCHANGERBasicInfo GetHEATEXCHANGERBasicInfoByCode(string heatexchangerCode)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT * FROM HEATEXCHANGER_BASICINFO WHERE HEATEXCHANGER_CODE = @HEATEXCHANGER_Code";
                return dbHelper.Conn.QueryFirstOrDefault<HEATEXCHANGERBasicInfo>(query, new { HEATEXCHANGER_Code = heatexchangerCode });
            }
        }

        /// <summary>
        /// HEATEXCHANGER 기본정보 전체 불러오기
        /// </summary>
        public Result GetAllHEATEXCHANGERBasicInfoRepo(out List<HEATEXCHANGERBasicInfo> heatexchangerBasicInfo)
        {
            Result res = new Result(true);
            heatexchangerBasicInfo = new List<HEATEXCHANGERBasicInfo>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = @"SELECT TBL_IDX, HEATEXCHANGER_CODE, SERIAL_NO, NAME, INSTALL_DATE, OPERATING_DATE, PRICE, 
                                         INSTALL_PLACE, RATED_V, RATED_A, MAKE_COMPANY, MAKE_NO, PHOTO, IS_DIAGNOSTICS, 
                                         IS_HEALTH, WRITER, TBL_GETDATE 
                                  FROM HEATEXCHANGER_BASICINFO";
                    heatexchangerBasicInfo = dbHelper.Conn.Query<HEATEXCHANGERBasicInfo>(query).AsList();

                    LogHelper.WriteLog("heatexchangerBasicInfo Data", $"총 {heatexchangerBasicInfo.Count} 건 조회됨");
                    res.Message = "GetAllHEATEXCHANGERBasicInfoRepo 동작 성공";
                    LogHelper.WriteLog("DB(HEATEXCHANGER_BASICINFO)", res.Message);
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "GetAllHEATEXCHANGERBasicInfoRepo 실패: " + ex.StackTrace + ex.Message;
                LogHelper.WriteLog("DB(HEATEXCHANGER_BASICINFO)", res.Message);
            }
            return res;
        }

        //HEATEXCHANGER Device페이지에서 '설비들' 표시하기 위해 사용 Basic 모델과 RiskMatrix HI 불러옴
        public Result GetAllHEATEXCHANGERBasicInfoWithRiskMatrixRepo(out List<dynamic> heatexchangerInfoWithRisk)
        {
            Result res = new Result(true);
            heatexchangerInfoWithRisk = new List<dynamic>();
            
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = @"
                SELECT 
                    b.TBL_IDX, 
                    b.HEATEXCHANGER_Code, 
                    b.Serial_No, 
                    b.Install_Date, 
                    b.Operating_Date, 
                    r.HI
                FROM HEATEXCHANGER_BASICINFO b
                LEFT JOIN RISKMATRIX r ON b.HEATEXCHANGER_Code = r.CODE
                ORDER BY b.TBL_IDX";
                    
                    heatexchangerInfoWithRisk = dbHelper.Conn.Query(query).AsList();
                }
                res.Message = "HEATEXCHANGER 기본정보와 RISKMATRIX 정보 조회 성공";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = ex.Message;
            }
            return res;
        }

        public Result CreateHEATEXCHANGERBasicInfoRepo(HEATEXCHANGERBasicInfo newHEATEXCHANGERBasicInfo)
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
                            // HEATEXCHANGER_BASICINFO 테이블에 데이터 삽입
                            var queryBasicInfo = @"
                INSERT INTO HEATEXCHANGER_BASICINFO (HEATEXCHANGER_CODE, SERIAL_NO, NAME, INSTALL_DATE, OPERATING_DATE, PRICE, 
                                             INSTALL_PLACE, CAPACITY, RATED_V, RATED_A, MAKE_COMPANY, MAKE_NO, PHOTO, 
                                             IS_DIAGNOSTICS, IS_HEALTH, WRITER) 
                VALUES (@HEATEXCHANGER_Code, @Serial_No, @Name, @Install_Date, @Operating_Date, @Price, @Install_Place, 
                        @Capacity, @Rated_V, @Rated_A, @Make_Company, @Make_No, @Photo, @Is_Diagnostics, 
                        @Is_Health, @Writer)";

                            int affectedRowsBasicInfo = conn.Execute(queryBasicInfo, newHEATEXCHANGERBasicInfo, transaction);

                            if (affectedRowsBasicInfo > 0)
                            {
                                // RISKMATRIX 테이블에 데이터 삽입
                                var queryRiskMatrix = @"
                    INSERT INTO RISKMATRIX (CODE, COF, POF) 
                    VALUES (@HEATEXCHANGER_Code, @DefaultCof, @DefaultPof)";

                                // 초기 COF와 POF 값은 기본값으로 설정 (필요시 변경)
                                var riskMatrixData = new
                                {
                                    HEATEXCHANGER_Code = newHEATEXCHANGERBasicInfo.HEATEXCHANGER_Code,
                                    DefaultCof = "0",
                                    DefaultPof = "0"
                                };

                                int affectedRowsRiskMatrix = conn.Execute(queryRiskMatrix, riskMatrixData, transaction);

                                if (affectedRowsRiskMatrix > 0)
                                {
                                    transaction.Commit();
                                    res.Message = "CreateHEATEXCHANGERBasicInfoRepo 성공: HEATEXCHANGER Serial_No: " + newHEATEXCHANGERBasicInfo.Serial_No;
                                    LogHelper.WriteLog("DB(HEATEXCHANGER_BASICINFO)", res.Message);
                                }
                                else
                                {
                                    throw new Exception("RISKMATRIX 테이블에 데이터 삽입 실패");
                                }
                            }
                            else
                            {
                                throw new Exception("HEATEXCHANGER_BASICINFO 테이블에 데이터 삽입 실패");
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            res.IsSuccess = false;
                            res.Message = "CreateHEATEXCHANGERBasicInfoRepo 실패: " + ex.Message;
                            LogHelper.WriteLog("DB(HEATEXCHANGER_BASICINFO)", "CreateHEATEXCHANGERBasicInfoRepo 오류: " + ex.Message + " 스택트레이스: " + ex.StackTrace);
                        }
                    }
                }
            }
            return res;
        }

        public Result UpdateHEATEXCHANGERBasicInfoRepo(HEATEXCHANGERBasicInfo heatexchangerBasicInfo)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    // HEATEXCHANGER_CODE를 기준으로 업데이트
                    var query = @"
            UPDATE HEATEXCHANGER_BASICINFO
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
            WHERE HEATEXCHANGER_CODE = @HEATEXCHANGER_Code";

                    int affectedRows = dbHelper.Conn.Execute(query, heatexchangerBasicInfo);
                    if (affectedRows > 0)
                    {
                        res.Message = "UpdateHEATEXCHANGERBasicInfoRepo 성공. HEATEXCHANGER_CODE: " + heatexchangerBasicInfo.HEATEXCHANGER_Code;
                        LogHelper.WriteLog("DB(HEATEXCHANGER_BASICINFO)", res.Message);
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "UpdateHEATEXCHANGERBasicInfoRepo 실패: 데이터 수정에 실패했습니다.";
                        LogHelper.WriteLog("DB(HEATEXCHANGER_BASICINFO)", res.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "UpdateHEATEXCHANGERBasicInfoRepo 실패: " + ex.StackTrace + ex.Message;
                LogHelper.WriteLog("DB(HEATEXCHANGER_BASICINFO)", res.Message);
            }
            return res;
        }

        public Result DeleteHEATEXCHANGERBasicInfoRepo(string tblIdx)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = "DELETE FROM HEATEXCHANGER_BASICINFO WHERE TBL_IDX = @Tbl_Idx";
                    int affectedRows = dbHelper.Conn.Execute(query, new { Tbl_Idx = tblIdx });

                    if (affectedRows > 0)
                    {
                        res.Message = "DeleteHEATEXCHANGERBasicInfoRepo 성공: HEATEXCHANGERBasicInfo Tbl_Idx: " + tblIdx;
                        LogHelper.WriteLog("DB(HEATEXCHANGER_BASICINFO)", res.Message);
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "DeleteHEATEXCHANGERBasicInfoRepo 실패: 해당 Tbl_Idx를 찾을 수 없습니다.";
                        LogHelper.WriteLog("DB(HEATEXCHANGER_BASICINFO)", res.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "DeleteHEATEXCHANGERBasicInfoRepo 실패: " + ex.Message + "\n" + ex.StackTrace;
                LogHelper.WriteLog("DB(HEATEXCHANGER_BASICINFO)", res.Message);
            }
            return res;
        }
    }
}
