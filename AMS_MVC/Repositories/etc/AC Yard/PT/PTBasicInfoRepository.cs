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
    public class PTBasicInfoRepository
    {
        // 가장 큰 PT_CODE 값을 반환
        public string GetLatestPTCode()
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT MAX(PT_CODE) FROM PT_BASICINFO WHERE PT_CODE LIKE 'PT%'";
                return dbHelper.Conn.QuerySingleOrDefault<string>(query);
            }
        }

        public PTBasicInfo GetPTBasicInfoByTblIdxRepo(string tblIdx)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT * FROM PT_BASICINFO WHERE TBL_IDX = @Tbl_Idx";
                return dbHelper.Conn.QueryFirstOrDefault<PTBasicInfo>(query, new { Tbl_Idx = tblIdx });
            }
        }

        public PTBasicInfo GetPTBasicInfoByCode(string ptCode)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT * FROM PT_BASICINFO WHERE PT_CODE = @PT_Code";
                return dbHelper.Conn.QueryFirstOrDefault<PTBasicInfo>(query, new { PT_Code = ptCode });
            }
        }

        /// <summary>
        /// PT 기본정보 전체 불러오기
        /// </summary>
        public Result GetAllPTBasicInfoRepo(out List<PTBasicInfo> ptBasicInfo)
        {
            Result res = new Result(true);
            ptBasicInfo = new List<PTBasicInfo>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = @"SELECT TBL_IDX, PT_CODE, SERIAL_NO, NAME, INSTALL_DATE, OPERATING_DATE, PRICE, 
                                         INSTALL_PLACE, RATED_V, RATED_A, MAKE_COMPANY, MAKE_NO, PHOTO, IS_DIAGNOSTICS, 
                                         IS_HEALTH, WRITER, TBL_GETDATE 
                                  FROM PT_BASICINFO";
                    ptBasicInfo = dbHelper.Conn.Query<PTBasicInfo>(query).AsList();

                    LogHelper.WriteLog("ptBasicInfo Data", $"총 {ptBasicInfo.Count} 건 조회됨");
                    res.Message = "GetAllPTBasicInfoRepo 동작 성공";
                    LogHelper.WriteLog("DB(PT_BASICINFO)", res.Message);
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "GetAllPTBasicInfoRepo 실패: " + ex.StackTrace + ex.Message;
                LogHelper.WriteLog("DB(PT_BASICINFO)", res.Message);
            }
            return res;
        }

        //PT Device페이지에서 '설비들' 표시하기 위해 사용 Basic 모델과 RiskMatrix HI 불러옴
        public Result GetAllPTBasicInfoWithRiskMatrixRepo(out List<dynamic> ptInfoWithRisk)
        {
            Result res = new Result(true);
            ptInfoWithRisk = new List<dynamic>();
            
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = @"
                SELECT 
                    b.TBL_IDX, 
                    b.PT_Code, 
                    b.Serial_No, 
                    b.Install_Date, 
                    b.Operating_Date, 
                    r.HI
                FROM PT_BASICINFO b
                LEFT JOIN RISKMATRIX r ON b.PT_Code = r.CODE
                ORDER BY b.TBL_IDX";
                    
                    ptInfoWithRisk = dbHelper.Conn.Query(query).AsList();
                }
                res.Message = "PT 기본정보와 RISKMATRIX 정보 조회 성공";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = ex.Message;
            }
            return res;
        }

        public Result CreatePTBasicInfoRepo(PTBasicInfo newPTBasicInfo)
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
                            // PT_BASICINFO 테이블에 데이터 삽입
                            var queryBasicInfo = @"
                INSERT INTO PT_BASICINFO (PT_CODE, SERIAL_NO, NAME, INSTALL_DATE, OPERATING_DATE, PRICE, 
                                             INSTALL_PLACE, CAPACITY, RATED_V, RATED_A, MAKE_COMPANY, MAKE_NO, PHOTO, 
                                             IS_DIAGNOSTICS, IS_HEALTH, WRITER) 
                VALUES (@PT_Code, @Serial_No, @Name, @Install_Date, @Operating_Date, @Price, @Install_Place, 
                        @Capacity, @Rated_V, @Rated_A, @Make_Company, @Make_No, @Photo, @Is_Diagnostics, 
                        @Is_Health, @Writer)";

                            int affectedRowsBasicInfo = conn.Execute(queryBasicInfo, newPTBasicInfo, transaction);

                            if (affectedRowsBasicInfo > 0)
                            {
                                // RISKMATRIX 테이블에 데이터 삽입
                                var queryRiskMatrix = @"
                    INSERT INTO RISKMATRIX (CODE, COF, POF) 
                    VALUES (@PT_Code, @DefaultCof, @DefaultPof)";

                                // 초기 COF와 POF 값은 기본값으로 설정 (필요시 변경)
                                var riskMatrixData = new
                                {
                                    PT_Code = newPTBasicInfo.PT_Code,
                                    DefaultCof = "0",
                                    DefaultPof = "0"
                                };

                                int affectedRowsRiskMatrix = conn.Execute(queryRiskMatrix, riskMatrixData, transaction);

                                if (affectedRowsRiskMatrix > 0)
                                {
                                    transaction.Commit();
                                    res.Message = "CreatePTBasicInfoRepo 성공: PT Serial_No: " + newPTBasicInfo.Serial_No;
                                    LogHelper.WriteLog("DB(PT_BASICINFO)", res.Message);
                                }
                                else
                                {
                                    throw new Exception("RISKMATRIX 테이블에 데이터 삽입 실패");
                                }
                            }
                            else
                            {
                                throw new Exception("PT_BASICINFO 테이블에 데이터 삽입 실패");
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            res.IsSuccess = false;
                            res.Message = "CreatePTBasicInfoRepo 실패: " + ex.Message;
                            LogHelper.WriteLog("DB(PT_BASICINFO)", "CreatePTBasicInfoRepo 오류: " + ex.Message + " 스택트레이스: " + ex.StackTrace);
                        }
                    }
                }
            }
            return res;
        }

        public Result UpdatePTBasicInfoRepo(PTBasicInfo ptBasicInfo)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    // PT_CODE를 기준으로 업데이트
                    var query = @"
            UPDATE PT_BASICINFO
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
            WHERE PT_CODE = @PT_Code";

                    int affectedRows = dbHelper.Conn.Execute(query, ptBasicInfo);
                    if (affectedRows > 0)
                    {
                        res.Message = "UpdatePTBasicInfoRepo 성공. PT_CODE: " + ptBasicInfo.PT_Code;
                        LogHelper.WriteLog("DB(PT_BASICINFO)", res.Message);
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "UpdatePTBasicInfoRepo 실패: 데이터 수정에 실패했습니다.";
                        LogHelper.WriteLog("DB(PT_BASICINFO)", res.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "UpdatePTBasicInfoRepo 실패: " + ex.StackTrace + ex.Message;
                LogHelper.WriteLog("DB(PT_BASICINFO)", res.Message);
            }
            return res;
        }

        public Result DeletePTBasicInfoRepo(string tblIdx)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = "DELETE FROM PT_BASICINFO WHERE TBL_IDX = @Tbl_Idx";
                    int affectedRows = dbHelper.Conn.Execute(query, new { Tbl_Idx = tblIdx });

                    if (affectedRows > 0)
                    {
                        res.Message = "DeletePTBasicInfoRepo 성공: PTBasicInfo Tbl_Idx: " + tblIdx;
                        LogHelper.WriteLog("DB(PT_BASICINFO)", res.Message);
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "DeletePTBasicInfoRepo 실패: 해당 Tbl_Idx를 찾을 수 없습니다.";
                        LogHelper.WriteLog("DB(PT_BASICINFO)", res.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "DeletePTBasicInfoRepo 실패: " + ex.Message + "\n" + ex.StackTrace;
                LogHelper.WriteLog("DB(PT_BASICINFO)", res.Message);
            }
            return res;
        }
    }
}
