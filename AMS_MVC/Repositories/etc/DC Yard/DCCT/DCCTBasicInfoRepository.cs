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
    public class DCCTBasicInfoRepository
    {
        // 가장 큰 DCCT_CODE 값을 반환
        public string GetLatestDCCTCode()
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT MAX(DCCT_CODE) FROM DCCT_BASICINFO WHERE DCCT_CODE LIKE 'DCCT%'";
                return dbHelper.Conn.QuerySingleOrDefault<string>(query);
            }
        }

        public DCCTBasicInfo GetDCCTBasicInfoByTblIdxRepo(string tblIdx)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT * FROM DCCT_BASICINFO WHERE TBL_IDX = @Tbl_Idx";
                return dbHelper.Conn.QueryFirstOrDefault<DCCTBasicInfo>(query, new { Tbl_Idx = tblIdx });
            }
        }

        public DCCTBasicInfo GetDCCTBasicInfoByCode(string dcctCode)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT * FROM DCCT_BASICINFO WHERE DCCT_CODE = @DCCT_Code";
                return dbHelper.Conn.QueryFirstOrDefault<DCCTBasicInfo>(query, new { DCCT_Code = dcctCode });
            }
        }

        /// <summary>
        /// DCCT 기본정보 전체 불러오기
        /// </summary>
        public Result GetAllDCCTBasicInfoRepo(out List<DCCTBasicInfo> dcctBasicInfo)
        {
            Result res = new Result(true);
            dcctBasicInfo = new List<DCCTBasicInfo>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = @"SELECT TBL_IDX, DCCT_CODE, SERIAL_NO, NAME, INSTALL_DATE, OPERATING_DATE, PRICE, 
                                         INSTALL_PLACE, RATED_V, RATED_A, MAKE_COMPANY, MAKE_NO, PHOTO, IS_DIAGNOSTICS, 
                                         IS_HEALTH, REMAIN_LIFE, WRITER, TBL_GETDATE 
                                  FROM DCCT_BASICINFO";
                    dcctBasicInfo = dbHelper.Conn.Query<DCCTBasicInfo>(query).AsList();

                    LogHelper.WriteLog("dcctBasicInfo Data", $"총 {dcctBasicInfo.Count} 건 조회됨");
                    res.Message = "GetAllDCCTBasicInfoRepo 동작 성공";
                    LogHelper.WriteLog("DB(DCCT_BASICINFO)", res.Message);
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "GetAllDCCTBasicInfoRepo 실패: " + ex.StackTrace + ex.Message;
                LogHelper.WriteLog("DB(DCCT_BASICINFO)", res.Message);
            }
            return res;
        }

        //DCCT Device페이지에서 '설비들' 표시하기 위해 사용 Basic 모델과 RiskMatrix HI 불러옴
        public Result GetAllDCCTBasicInfoWithRiskMatrixRepo(out List<dynamic> dcctInfoWithRisk)
        {
            Result res = new Result(true);
            dcctInfoWithRisk = new List<dynamic>();
            
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = @"
                SELECT 
                    b.TBL_IDX, 
                    b.DCCT_Code, 
                    b.Serial_No, 
                    b.Install_Date, 
                    b.Operating_Date, 
                    b.REMAIN_LIFE   AS Remain_Life, 
                    r.HI
                FROM DCCT_BASICINFO b
                LEFT JOIN RISKMATRIX r ON b.DCCT_Code = r.CODE
                ORDER BY b.TBL_IDX";
                    
                    dcctInfoWithRisk = dbHelper.Conn.Query(query).AsList();
                }
                res.Message = "DCCT 기본정보와 RISKMATRIX 정보 조회 성공";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = ex.Message;
            }
            return res;
        }

        public Result CreateDCCTBasicInfoRepo(DCCTBasicInfo newDCCTBasicInfo)
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
                            // DCCT_BASICINFO 테이블에 데이터 삽입
                            var queryBasicInfo = @"
                INSERT INTO DCCT_BASICINFO (DCCT_CODE, SERIAL_NO, NAME, INSTALL_DATE, OPERATING_DATE, PRICE, 
                                             INSTALL_PLACE, CAPACITY, RATED_V, RATED_A, MAKE_COMPANY, MAKE_NO, PHOTO, 
                                             IS_DIAGNOSTICS, REMAIN_LIFE, IS_HEALTH, WRITER) 
                VALUES (@DCCT_Code, @Serial_No, @Name, @Install_Date, @Operating_Date, @Price, @Install_Place, 
                        @Capacity, @Rated_V, @Rated_A, @Make_Company, @Make_No, @Photo, @Is_Diagnostics, 
                        @Is_Health, @Remain_Life, @Writer)";

                            int affectedRowsBasicInfo = conn.Execute(queryBasicInfo, newDCCTBasicInfo, transaction);

                            if (affectedRowsBasicInfo > 0)
                            {
                                // RISKMATRIX 테이블에 데이터 삽입
                                var queryRiskMatrix = @"
                    INSERT INTO RISKMATRIX (CODE, COF, POF) 
                    VALUES (@DCCT_Code, @DefaultCof, @DefaultPof)";

                                // 초기 COF와 POF 값은 기본값으로 설정 (필요시 변경)
                                var riskMatrixData = new
                                {
                                    DCCT_Code = newDCCTBasicInfo.DCCT_Code,
                                    DefaultCof = "0",
                                    DefaultPof = "0"
                                };

                                int affectedRowsRiskMatrix = conn.Execute(queryRiskMatrix, riskMatrixData, transaction);

                                if (affectedRowsRiskMatrix > 0)
                                {
                                    transaction.Commit();
                                    res.Message = "CreateDCCTBasicInfoRepo 성공: DCCT Serial_No: " + newDCCTBasicInfo.Serial_No;
                                    LogHelper.WriteLog("DB(DCCT_BASICINFO)", res.Message);
                                }
                                else
                                {
                                    throw new Exception("RISKMATRIX 테이블에 데이터 삽입 실패");
                                }
                            }
                            else
                            {
                                throw new Exception("DCCT_BASICINFO 테이블에 데이터 삽입 실패");
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            res.IsSuccess = false;
                            res.Message = "CreateDCCTBasicInfoRepo 실패: " + ex.Message;
                            LogHelper.WriteLog("DB(DCCT_BASICINFO)", "CreateDCCTBasicInfoRepo 오류: " + ex.Message + " 스택트레이스: " + ex.StackTrace);
                        }
                    }
                }
            }
            return res;
        }

        public Result UpdateDCCTBasicInfoRepo(DCCTBasicInfo dcctBasicInfo)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    // DCCT_CODE를 기준으로 업데이트
                    var query = @"
            UPDATE DCCT_BASICINFO
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
            WHERE DCCT_CODE = @DCCT_Code";

                    int affectedRows = dbHelper.Conn.Execute(query, dcctBasicInfo);
                    if (affectedRows > 0)
                    {
                        res.Message = "UpdateDCCTBasicInfoRepo 성공. DCCT_CODE: " + dcctBasicInfo.DCCT_Code;
                        LogHelper.WriteLog("DB(DCCT_BASICINFO)", res.Message);
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "UpdateDCCTBasicInfoRepo 실패: 데이터 수정에 실패했습니다.";
                        LogHelper.WriteLog("DB(DCCT_BASICINFO)", res.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "UpdateDCCTBasicInfoRepo 실패: " + ex.StackTrace + ex.Message;
                LogHelper.WriteLog("DB(DCCT_BASICINFO)", res.Message);
            }
            return res;
        }

        public Result DeleteDCCTBasicInfoRepo(string tblIdx)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = "DELETE FROM DCCT_BASICINFO WHERE TBL_IDX = @Tbl_Idx";
                    int affectedRows = dbHelper.Conn.Execute(query, new { Tbl_Idx = tblIdx });

                    if (affectedRows > 0)
                    {
                        res.Message = "DeleteDCCTBasicInfoRepo 성공: DCCTBasicInfo Tbl_Idx: " + tblIdx;
                        LogHelper.WriteLog("DB(DCCT_BASICINFO)", res.Message);
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "DeleteDCCTBasicInfoRepo 실패: 해당 Tbl_Idx를 찾을 수 없습니다.";
                        LogHelper.WriteLog("DB(DCCT_BASICINFO)", res.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "DeleteDCCTBasicInfoRepo 실패: " + ex.Message + "\n" + ex.StackTrace;
                LogHelper.WriteLog("DB(DCCT_BASICINFO)", res.Message);
            }
            return res;
        }
    }
}
