
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Common
{
    public class WALLBUSHINGBasicInfoRepository
    {
        // 가장 큰 WALLBUSHING_CODE 값을 반환
        public string GetLatestWALLBUSHINGCode()
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT MAX(WALLBUSHING_CODE) FROM WALLBUSHING_BASICINFO WHERE WALLBUSHING_CODE LIKE 'WALLBUSHING%'";
                return dbHelper.Conn.QuerySingleOrDefault<string>(query);
            }
        }

        public WALLBUSHINGBasicInfo GetWALLBUSHINGBasicInfoByTblIdxRepo(string tblIdx)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT * FROM WALLBUSHING_BASICINFO WHERE TBL_IDX = @Tbl_Idx";
                return dbHelper.Conn.QueryFirstOrDefault<WALLBUSHINGBasicInfo>(query, new { Tbl_Idx = tblIdx });
            }
        }

        public WALLBUSHINGBasicInfo GetWALLBUSHINGBasicInfoByCode(string wallbushingCode)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT * FROM WALLBUSHING_BASICINFO WHERE WALLBUSHING_CODE = @WALLBUSHING_Code";
                return dbHelper.Conn.QueryFirstOrDefault<WALLBUSHINGBasicInfo>(query, new { WALLBUSHING_Code = wallbushingCode });
            }
        }

        /// <summary>
        /// WALLBUSHING 기본정보 전체 불러오기
        /// </summary>
        public Result GetAllWALLBUSHINGBasicInfoRepo(out List<WALLBUSHINGBasicInfo> wallbushingBasicInfo)
        {
            Result res = new Result(true);
            wallbushingBasicInfo = new List<WALLBUSHINGBasicInfo>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = @"SELECT TBL_IDX, WALLBUSHING_CODE, SERIAL_NO, NAME, INSTALL_DATE, OPERATING_DATE, PRICE, 
                                         INSTALL_PLACE, RATED_V, RATED_A, MAKE_COMPANY, MAKE_NO, PHOTO, IS_DIAGNOSTICS, 
                                         IS_HEALTH, REMAIN_LIFE, WRITER, TBL_GETDATE 
                                  FROM WALLBUSHING_BASICINFO";
                    wallbushingBasicInfo = dbHelper.Conn.Query<WALLBUSHINGBasicInfo>(query).AsList();

                    LogHelper.WriteLog("wallbushingBasicInfo Data", $"총 {wallbushingBasicInfo.Count} 건 조회됨");
                    res.Message = "GetAllWALLBUSHINGBasicInfoRepo 동작 성공";
                    LogHelper.WriteLog("DB(WALLBUSHING_BASICINFO)", res.Message);
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "GetAllWALLBUSHINGBasicInfoRepo 실패: " + ex.StackTrace + ex.Message;
                LogHelper.WriteLog("DB(WALLBUSHING_BASICINFO)", res.Message);
            }
            return res;
        }

        //WALLBUSHING Device페이지에서 '설비들' 표시하기 위해 사용 Basic 모델과 RiskMatrix HI 불러옴
        public Result GetAllWALLBUSHINGBasicInfoWithRiskMatrixRepo(out List<dynamic> wallbushingInfoWithRisk)
        {
            Result res = new Result(true);
            wallbushingInfoWithRisk = new List<dynamic>();
            
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = @"
                SELECT 
                    b.TBL_IDX, 
                    b.WALLBUSHING_Code, 
                    b.Serial_No, 
                    b.Install_Date, 
                    b.Operating_Date, 
                    b.REMAIN_LIFE   AS Remain_Life,
                    r.HI
                FROM WALLBUSHING_BASICINFO b
                LEFT JOIN RISKMATRIX r ON b.WALLBUSHING_Code = r.CODE
                ORDER BY b.TBL_IDX";
                    
                    wallbushingInfoWithRisk = dbHelper.Conn.Query(query).AsList();
                }
                res.Message = "WALLBUSHING 기본정보와 RISKMATRIX 정보 조회 성공";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = ex.Message;
            }
            return res;
        }

        public Result CreateWALLBUSHINGBasicInfoRepo(WALLBUSHINGBasicInfo newWALLBUSHINGBasicInfo)
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
                            // WALLBUSHING_BASICINFO 테이블에 데이터 삽입
                            var queryBasicInfo = @"
                INSERT INTO WALLBUSHING_BASICINFO (WALLBUSHING_CODE, SERIAL_NO, NAME, INSTALL_DATE, OPERATING_DATE, PRICE, 
                                             INSTALL_PLACE, CAPACITY, RATED_V, RATED_A, MAKE_COMPANY, MAKE_NO, PHOTO, 
                                             IS_DIAGNOSTICS, REMAIN_LIFE, IS_HEALTH, WRITER) 
                VALUES (@WALLBUSHING_Code, @Serial_No, @Name, @Install_Date, @Operating_Date, @Price, @Install_Place, 
                        @Capacity, @Rated_V, @Rated_A, @Make_Company, @Make_No, @Photo, @Is_Diagnostics, 
                        @Is_Health, @Remain_Life, @Writer)";

                            int affectedRowsBasicInfo = conn.Execute(queryBasicInfo, newWALLBUSHINGBasicInfo, transaction);

                            if (affectedRowsBasicInfo > 0)
                            {
                                // RISKMATRIX 테이블에 데이터 삽입
                                var queryRiskMatrix = @"
                    INSERT INTO RISKMATRIX (CODE, COF, POF) 
                    VALUES (@WALLBUSHING_Code, @DefaultCof, @DefaultPof)";

                                // 초기 COF와 POF 값은 기본값으로 설정 (필요시 변경)
                                var riskMatrixData = new
                                {
                                    WALLBUSHING_Code = newWALLBUSHINGBasicInfo.WALLBUSHING_Code,
                                    DefaultCof = "0",
                                    DefaultPof = "0"
                                };

                                int affectedRowsRiskMatrix = conn.Execute(queryRiskMatrix, riskMatrixData, transaction);

                                if (affectedRowsRiskMatrix > 0)
                                {
                                    transaction.Commit();
                                    res.Message = "CreateWALLBUSHINGBasicInfoRepo 성공: WALLBUSHING Serial_No: " + newWALLBUSHINGBasicInfo.Serial_No;
                                    LogHelper.WriteLog("DB(WALLBUSHING_BASICINFO)", res.Message);
                                }
                                else
                                {
                                    throw new Exception("RISKMATRIX 테이블에 데이터 삽입 실패");
                                }
                            }
                            else
                            {
                                throw new Exception("WALLBUSHING_BASICINFO 테이블에 데이터 삽입 실패");
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            res.IsSuccess = false;
                            res.Message = "CreateWALLBUSHINGBasicInfoRepo 실패: " + ex.Message;
                            LogHelper.WriteLog("DB(WALLBUSHING_BASICINFO)", "CreateWALLBUSHINGBasicInfoRepo 오류: " + ex.Message + " 스택트레이스: " + ex.StackTrace);
                        }
                    }
                }
            }
            return res;
        }

        public Result UpdateWALLBUSHINGBasicInfoRepo(WALLBUSHINGBasicInfo wallbushingBasicInfo)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    // WALLBUSHING_CODE를 기준으로 업데이트
                    var query = @"
            UPDATE WALLBUSHING_BASICINFO
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
            WHERE WALLBUSHING_CODE = @WALLBUSHING_Code";

                    int affectedRows = dbHelper.Conn.Execute(query, wallbushingBasicInfo);
                    if (affectedRows > 0)
                    {
                        res.Message = "UpdateWALLBUSHINGBasicInfoRepo 성공. WALLBUSHING_CODE: " + wallbushingBasicInfo.WALLBUSHING_Code;
                        LogHelper.WriteLog("DB(WALLBUSHING_BASICINFO)", res.Message);
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "UpdateWALLBUSHINGBasicInfoRepo 실패: 데이터 수정에 실패했습니다.";
                        LogHelper.WriteLog("DB(WALLBUSHING_BASICINFO)", res.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "UpdateWALLBUSHINGBasicInfoRepo 실패: " + ex.StackTrace + ex.Message;
                LogHelper.WriteLog("DB(WALLBUSHING_BASICINFO)", res.Message);
            }
            return res;
        }

        public Result DeleteWALLBUSHINGBasicInfoRepo(string tblIdx)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = "DELETE FROM WALLBUSHING_BASICINFO WHERE TBL_IDX = @Tbl_Idx";
                    int affectedRows = dbHelper.Conn.Execute(query, new { Tbl_Idx = tblIdx });

                    if (affectedRows > 0)
                    {
                        res.Message = "DeleteWALLBUSHINGBasicInfoRepo 성공: WALLBUSHINGBasicInfo Tbl_Idx: " + tblIdx;
                        LogHelper.WriteLog("DB(WALLBUSHING_BASICINFO)", res.Message);
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "DeleteWALLBUSHINGBasicInfoRepo 실패: 해당 Tbl_Idx를 찾을 수 없습니다.";
                        LogHelper.WriteLog("DB(WALLBUSHING_BASICINFO)", res.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "DeleteWALLBUSHINGBasicInfoRepo 실패: " + ex.Message + "\n" + ex.StackTrace;
                LogHelper.WriteLog("DB(WALLBUSHING_BASICINFO)", res.Message);
            }
            return res;
        }
    }
}
