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
    public class ZIGZAGTRBasicInfoRepository
    {
        // 가장 큰 ZIGZAGTR_CODE 값을 반환
        public string GetLatestZIGZAGTRCode()
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT MAX(ZIGZAGTR_CODE) FROM ZIGZAGTR_BASICINFO WHERE ZIGZAGTR_CODE LIKE 'ZIGZAGTR%'";
                return dbHelper.Conn.QuerySingleOrDefault<string>(query);
            }
        }

        public ZIGZAGTRBasicInfo GetZIGZAGTRBasicInfoByTblIdxRepo(string tblIdx)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT * FROM ZIGZAGTR_BASICINFO WHERE TBL_IDX = @Tbl_Idx";
                return dbHelper.Conn.QueryFirstOrDefault<ZIGZAGTRBasicInfo>(query, new { Tbl_Idx = tblIdx });
            }
        }

        public ZIGZAGTRBasicInfo GetZIGZAGTRBasicInfoByCode(string zigzagtrCode)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT * FROM ZIGZAGTR_BASICINFO WHERE ZIGZAGTR_CODE = @ZIGZAGTR_Code";
                return dbHelper.Conn.QueryFirstOrDefault<ZIGZAGTRBasicInfo>(query, new { ZIGZAGTR_Code = zigzagtrCode });
            }
        }

        /// <summary>
        /// ZIGZAGTR 기본정보 전체 불러오기
        /// </summary>
        public Result GetAllZIGZAGTRBasicInfoRepo(out List<ZIGZAGTRBasicInfo> zigzagtrBasicInfo)
        {
            Result res = new Result(true);
            zigzagtrBasicInfo = new List<ZIGZAGTRBasicInfo>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = @"SELECT TBL_IDX, ZIGZAGTR_CODE, SERIAL_NO, NAME, INSTALL_DATE, OPERATING_DATE, PRICE, 
                                         INSTALL_PLACE, RATED_V, RATED_A, MAKE_COMPANY, MAKE_NO, PHOTO, IS_DIAGNOSTICS, 
                                         IS_HEALTH, REMAIN_LIFE, WRITER, TBL_GETDATE 
                                  FROM ZIGZAGTR_BASICINFO";
                    zigzagtrBasicInfo = dbHelper.Conn.Query<ZIGZAGTRBasicInfo>(query).AsList();

                    LogHelper.WriteLog("zigzagtrBasicInfo Data", $"총 {zigzagtrBasicInfo.Count} 건 조회됨");
                    res.Message = "GetAllZIGZAGTRBasicInfoRepo 동작 성공";
                    LogHelper.WriteLog("DB(ZIGZAGTR_BASICINFO)", res.Message);
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "GetAllZIGZAGTRBasicInfoRepo 실패: " + ex.StackTrace + ex.Message;
                LogHelper.WriteLog("DB(ZIGZAGTR_BASICINFO)", res.Message);
            }
            return res;
        }

        //ZIGZAGTR Device페이지에서 '설비들' 표시하기 위해 사용 Basic 모델과 RiskMatrix HI 불러옴
        public Result GetAllZIGZAGTRBasicInfoWithRiskMatrixRepo(out List<dynamic> zigzagtrInfoWithRisk)
        {
            Result res = new Result(true);
            zigzagtrInfoWithRisk = new List<dynamic>();
            
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = @"
                SELECT 
                    b.TBL_IDX, 
                    b.ZIGZAGTR_Code, 
                    b.Serial_No, 
                    b.Install_Date, 
                    b.Operating_Date, 
                    r.HI
                FROM ZIGZAGTR_BASICINFO b
                LEFT JOIN RISKMATRIX r ON b.ZIGZAGTR_Code = r.CODE
                ORDER BY b.TBL_IDX";
                    
                    zigzagtrInfoWithRisk = dbHelper.Conn.Query(query).AsList();
                }
                res.Message = "ZIGZAGTR 기본정보와 RISKMATRIX 정보 조회 성공";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = ex.Message;
            }
            return res;
        }

        public Result CreateZIGZAGTRBasicInfoRepo(ZIGZAGTRBasicInfo newZIGZAGTRBasicInfo)
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
                            // ZIGZAGTR_BASICINFO 테이블에 데이터 삽입
                            var queryBasicInfo = @"
                INSERT INTO ZIGZAGTR_BASICINFO (ZIGZAGTR_CODE, SERIAL_NO, NAME, INSTALL_DATE, OPERATING_DATE, PRICE, 
                                             INSTALL_PLACE, CAPACITY, RATED_V, RATED_A, MAKE_COMPANY, MAKE_NO, PHOTO, 
                                             IS_DIAGNOSTICS, IS_HEALTH, REMAIN_LIFE, WRITER) 
                VALUES (@ZIGZAGTR_Code, @Serial_No, @Name, @Install_Date, @Operating_Date, @Price, @Install_Place, 
                        @Capacity, @Rated_V, @Rated_A, @Make_Company, @Make_No, @Photo, @Is_Diagnostics, 
                        @Is_Health, @Remain_Life, @Writer)";

                            int affectedRowsBasicInfo = conn.Execute(queryBasicInfo, newZIGZAGTRBasicInfo, transaction);

                            if (affectedRowsBasicInfo > 0)
                            {
                                // RISKMATRIX 테이블에 데이터 삽입
                                var queryRiskMatrix = @"
                    INSERT INTO RISKMATRIX (CODE, COF, POF) 
                    VALUES (@ZIGZAGTR_Code, @DefaultCof, @DefaultPof)";

                                // 초기 COF와 POF 값은 기본값으로 설정 (필요시 변경)
                                var riskMatrixData = new
                                {
                                    ZIGZAGTR_Code = newZIGZAGTRBasicInfo.ZIGZAGTR_Code,
                                    DefaultCof = "0",
                                    DefaultPof = "0"
                                };

                                int affectedRowsRiskMatrix = conn.Execute(queryRiskMatrix, riskMatrixData, transaction);

                                if (affectedRowsRiskMatrix > 0)
                                {
                                    transaction.Commit();
                                    res.Message = "CreateZIGZAGTRBasicInfoRepo 성공: ZIGZAGTR Serial_No: " + newZIGZAGTRBasicInfo.Serial_No;
                                    LogHelper.WriteLog("DB(ZIGZAGTR_BASICINFO)", res.Message);
                                }
                                else
                                {
                                    throw new Exception("RISKMATRIX 테이블에 데이터 삽입 실패");
                                }
                            }
                            else
                            {
                                throw new Exception("ZIGZAGTR_BASICINFO 테이블에 데이터 삽입 실패");
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            res.IsSuccess = false;
                            res.Message = "CreateZIGZAGTRBasicInfoRepo 실패: " + ex.Message;
                            LogHelper.WriteLog("DB(ZIGZAGTR_BASICINFO)", "CreateZIGZAGTRBasicInfoRepo 오류: " + ex.Message + " 스택트레이스: " + ex.StackTrace);
                        }
                    }
                }
            }
            return res;
        }

        public Result UpdateZIGZAGTRBasicInfoRepo(ZIGZAGTRBasicInfo zigzagtrBasicInfo)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    // ZIGZAGTR_CODE를 기준으로 업데이트
                    var query = @"
            UPDATE ZIGZAGTR_BASICINFO
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
            WHERE ZIGZAGTR_CODE = @ZIGZAGTR_Code";

                    int affectedRows = dbHelper.Conn.Execute(query, zigzagtrBasicInfo);
                    if (affectedRows > 0)
                    {
                        res.Message = "UpdateZIGZAGTRBasicInfoRepo 성공. ZIGZAGTR_CODE: " + zigzagtrBasicInfo.ZIGZAGTR_Code;
                        LogHelper.WriteLog("DB(ZIGZAGTR_BASICINFO)", res.Message);
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "UpdateZIGZAGTRBasicInfoRepo 실패: 데이터 수정에 실패했습니다.";
                        LogHelper.WriteLog("DB(ZIGZAGTR_BASICINFO)", res.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "UpdateZIGZAGTRBasicInfoRepo 실패: " + ex.StackTrace + ex.Message;
                LogHelper.WriteLog("DB(ZIGZAGTR_BASICINFO)", res.Message);
            }
            return res;
        }

        public Result DeleteZIGZAGTRBasicInfoRepo(string tblIdx)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = "DELETE FROM ZIGZAGTR_BASICINFO WHERE TBL_IDX = @Tbl_Idx";
                    int affectedRows = dbHelper.Conn.Execute(query, new { Tbl_Idx = tblIdx });

                    if (affectedRows > 0)
                    {
                        res.Message = "DeleteZIGZAGTRBasicInfoRepo 성공: ZIGZAGTRBasicInfo Tbl_Idx: " + tblIdx;
                        LogHelper.WriteLog("DB(ZIGZAGTR_BASICINFO)", res.Message);
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "DeleteZIGZAGTRBasicInfoRepo 실패: 해당 Tbl_Idx를 찾을 수 없습니다.";
                        LogHelper.WriteLog("DB(ZIGZAGTR_BASICINFO)", res.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "DeleteZIGZAGTRBasicInfoRepo 실패: " + ex.Message + "\n" + ex.StackTrace;
                LogHelper.WriteLog("DB(ZIGZAGTR_BASICINFO)", res.Message);
            }
            return res;
        }
    }
}
