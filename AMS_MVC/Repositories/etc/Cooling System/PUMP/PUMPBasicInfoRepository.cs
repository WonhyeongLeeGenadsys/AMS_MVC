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
    public class PUMPBasicInfoRepository
    {
        // 가장 큰 PUMP_CODE 값을 반환
        public string GetLatestPUMPCode()
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT MAX(PUMP_CODE) FROM PUMP_BASICINFO WHERE PUMP_CODE LIKE 'PUMP%'";
                return dbHelper.Conn.QuerySingleOrDefault<string>(query);
            }
        }

        public PUMPBasicInfo GetPUMPBasicInfoByTblIdxRepo(string tblIdx)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT * FROM PUMP_BASICINFO WHERE TBL_IDX = @Tbl_Idx";
                return dbHelper.Conn.QueryFirstOrDefault<PUMPBasicInfo>(query, new { Tbl_Idx = tblIdx });
            }
        }

        public PUMPBasicInfo GetPUMPBasicInfoByCode(string pumpCode)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT * FROM PUMP_BASICINFO WHERE PUMP_CODE = @PUMP_Code";
                return dbHelper.Conn.QueryFirstOrDefault<PUMPBasicInfo>(query, new { PUMP_Code = pumpCode });
            }
        }

        /// <summary>
        /// PUMP 기본정보 전체 불러오기
        /// </summary>
        public Result GetAllPUMPBasicInfoRepo(out List<PUMPBasicInfo> pumpBasicInfo)
        {
            Result res = new Result(true);
            pumpBasicInfo = new List<PUMPBasicInfo>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = @"SELECT TBL_IDX, PUMP_CODE, SERIAL_NO, NAME, INSTALL_DATE, OPERATING_DATE, PRICE, 
                                         INSTALL_PLACE, RATED_V, RATED_A, MAKE_COMPANY, MAKE_NO, PHOTO, IS_DIAGNOSTICS, 
                                         IS_HEALTH, REMAIN_LIFE, WRITER, TBL_GETDATE 
                                  FROM PUMP_BASICINFO";
                    pumpBasicInfo = dbHelper.Conn.Query<PUMPBasicInfo>(query).AsList();

                    LogHelper.WriteLog("pumpBasicInfo Data", $"총 {pumpBasicInfo.Count} 건 조회됨");
                    res.Message = "GetAllPUMPBasicInfoRepo 동작 성공";
                    LogHelper.WriteLog("DB(PUMP_BASICINFO)", res.Message);
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "GetAllPUMPBasicInfoRepo 실패: " + ex.StackTrace + ex.Message;
                LogHelper.WriteLog("DB(PUMP_BASICINFO)", res.Message);
            }
            return res;
        }

        //PUMP Device페이지에서 '설비들' 표시하기 위해 사용 Basic 모델과 RiskMatrix HI 불러옴
        public Result GetAllPUMPBasicInfoWithRiskMatrixRepo(out List<dynamic> pumpInfoWithRisk)
        {
            Result res = new Result(true);
            pumpInfoWithRisk = new List<dynamic>();
            
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = @"
                SELECT 
                    b.TBL_IDX, 
                    b.PUMP_Code, 
                    b.Serial_No, 
                    b.Install_Date, 
                    b.Operating_Date, 
                    r.HI
                FROM PUMP_BASICINFO b
                LEFT JOIN RISKMATRIX r ON b.PUMP_Code = r.CODE
                ORDER BY b.TBL_IDX";
                    
                    pumpInfoWithRisk = dbHelper.Conn.Query(query).AsList();
                }
                res.Message = "PUMP 기본정보와 RISKMATRIX 정보 조회 성공";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = ex.Message;
            }
            return res;
        }

        public Result CreatePUMPBasicInfoRepo(PUMPBasicInfo newPUMPBasicInfo)
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
                            // PUMP_BASICINFO 테이블에 데이터 삽입
                            var queryBasicInfo = @"
                INSERT INTO PUMP_BASICINFO (PUMP_CODE, SERIAL_NO, NAME, INSTALL_DATE, OPERATING_DATE, PRICE, 
                                             INSTALL_PLACE, CAPACITY, RATED_V, RATED_A, MAKE_COMPANY, MAKE_NO, PHOTO, 
                                             IS_DIAGNOSTICS, IS_HEALTH, REMAIN_LIFE, WRITER) 
                VALUES (@PUMP_Code, @Serial_No, @Name, @Install_Date, @Operating_Date, @Price, @Install_Place, 
                        @Capacity, @Rated_V, @Rated_A, @Make_Company, @Make_No, @Photo, @Is_Diagnostics, 
                        @Is_Health, @Remain_Life, @Writer)";

                            int affectedRowsBasicInfo = conn.Execute(queryBasicInfo, newPUMPBasicInfo, transaction);

                            if (affectedRowsBasicInfo > 0)
                            {
                                // RISKMATRIX 테이블에 데이터 삽입
                                var queryRiskMatrix = @"
                    INSERT INTO RISKMATRIX (CODE, COF, POF) 
                    VALUES (@PUMP_Code, @DefaultCof, @DefaultPof)";

                                // 초기 COF와 POF 값은 기본값으로 설정 (필요시 변경)
                                var riskMatrixData = new
                                {
                                    PUMP_Code = newPUMPBasicInfo.PUMP_Code,
                                    DefaultCof = "0",
                                    DefaultPof = "0"
                                };

                                int affectedRowsRiskMatrix = conn.Execute(queryRiskMatrix, riskMatrixData, transaction);

                                if (affectedRowsRiskMatrix > 0)
                                {
                                    transaction.Commit();
                                    res.Message = "CreatePUMPBasicInfoRepo 성공: PUMP Serial_No: " + newPUMPBasicInfo.Serial_No;
                                    LogHelper.WriteLog("DB(PUMP_BASICINFO)", res.Message);
                                }
                                else
                                {
                                    throw new Exception("RISKMATRIX 테이블에 데이터 삽입 실패");
                                }
                            }
                            else
                            {
                                throw new Exception("PUMP_BASICINFO 테이블에 데이터 삽입 실패");
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            res.IsSuccess = false;
                            res.Message = "CreatePUMPBasicInfoRepo 실패: " + ex.Message;
                            LogHelper.WriteLog("DB(PUMP_BASICINFO)", "CreatePUMPBasicInfoRepo 오류: " + ex.Message + " 스택트레이스: " + ex.StackTrace);
                        }
                    }
                }
            }
            return res;
        }

        public Result UpdatePUMPBasicInfoRepo(PUMPBasicInfo pumpBasicInfo)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    // PUMP_CODE를 기준으로 업데이트
                    var query = @"
            UPDATE PUMP_BASICINFO
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
            WHERE PUMP_CODE = @PUMP_Code";

                    int affectedRows = dbHelper.Conn.Execute(query, pumpBasicInfo);
                    if (affectedRows > 0)
                    {
                        res.Message = "UpdatePUMPBasicInfoRepo 성공. PUMP_CODE: " + pumpBasicInfo.PUMP_Code;
                        LogHelper.WriteLog("DB(PUMP_BASICINFO)", res.Message);
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "UpdatePUMPBasicInfoRepo 실패: 데이터 수정에 실패했습니다.";
                        LogHelper.WriteLog("DB(PUMP_BASICINFO)", res.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "UpdatePUMPBasicInfoRepo 실패: " + ex.StackTrace + ex.Message;
                LogHelper.WriteLog("DB(PUMP_BASICINFO)", res.Message);
            }
            return res;
        }

        public Result DeletePUMPBasicInfoRepo(string tblIdx)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = "DELETE FROM PUMP_BASICINFO WHERE TBL_IDX = @Tbl_Idx";
                    int affectedRows = dbHelper.Conn.Execute(query, new { Tbl_Idx = tblIdx });

                    if (affectedRows > 0)
                    {
                        res.Message = "DeletePUMPBasicInfoRepo 성공: PUMPBasicInfo Tbl_Idx: " + tblIdx;
                        LogHelper.WriteLog("DB(PUMP_BASICINFO)", res.Message);
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "DeletePUMPBasicInfoRepo 실패: 해당 Tbl_Idx를 찾을 수 없습니다.";
                        LogHelper.WriteLog("DB(PUMP_BASICINFO)", res.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "DeletePUMPBasicInfoRepo 실패: " + ex.Message + "\n" + ex.StackTrace;
                LogHelper.WriteLog("DB(PUMP_BASICINFO)", res.Message);
            }
            return res;
        }
    }
}
