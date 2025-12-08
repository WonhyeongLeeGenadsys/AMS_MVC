
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Common
{
    public class ARMREACTORBasicInfoRepository
    {
        // 가장 큰 ARMREACTOR_CODE 값을 반환
        public string GetLatestARMREACTORCode()
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT MAX(ARMREACTOR_CODE) FROM ARMREACTOR_BASICINFO WHERE ARMREACTOR_CODE LIKE 'ARMREACTOR%'";
                return dbHelper.Conn.QuerySingleOrDefault<string>(query);
            }
        }

        public ARMREACTORBasicInfo GetARMREACTORBasicInfoByTblIdxRepo(string tblIdx)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT * FROM ARMREACTOR_BASICINFO WHERE TBL_IDX = @Tbl_Idx";
                return dbHelper.Conn.QueryFirstOrDefault<ARMREACTORBasicInfo>(query, new { Tbl_Idx = tblIdx });
            }
        }

        public ARMREACTORBasicInfo GetARMREACTORBasicInfoByCode(string armreactorCode)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT * FROM ARMREACTOR_BASICINFO WHERE ARMREACTOR_CODE = @ARMREACTOR_Code";
                return dbHelper.Conn.QueryFirstOrDefault<ARMREACTORBasicInfo>(query, new { ARMREACTOR_Code = armreactorCode });
            }
        }

        /// <summary>
        /// ARMREACTOR 기본정보 전체 불러오기
        /// </summary>
        public Result GetAllARMREACTORBasicInfoRepo(out List<ARMREACTORBasicInfo> armreactorBasicInfo)
        {
            Result res = new Result(true);
            armreactorBasicInfo = new List<ARMREACTORBasicInfo>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = @"SELECT TBL_IDX, ARMREACTOR_CODE, SERIAL_NO, NAME, INSTALL_DATE, OPERATING_DATE, PRICE, 
                                         INSTALL_PLACE, RATED_V, RATED_A, MAKE_COMPANY, MAKE_NO, PHOTO, IS_DIAGNOSTICS, 
                                         IS_HEALTH, REMAIN_LIFE, WRITER, TBL_GETDATE 
                                  FROM ARMREACTOR_BASICINFO";
                    armreactorBasicInfo = dbHelper.Conn.Query<ARMREACTORBasicInfo>(query).AsList();

                    LogHelper.WriteLog("armreactorBasicInfo Data", $"총 {armreactorBasicInfo.Count} 건 조회됨");
                    res.Message = "GetAllARMREACTORBasicInfoRepo 동작 성공";
                    LogHelper.WriteLog("DB(ARMREACTOR_BASICINFO)", res.Message);
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "GetAllARMREACTORBasicInfoRepo 실패: " + ex.StackTrace + ex.Message;
                LogHelper.WriteLog("DB(ARMREACTOR_BASICINFO)", res.Message);
            }
            return res;
        }

        //ARMREACTOR Device페이지에서 '설비들' 표시하기 위해 사용 Basic 모델과 RiskMatrix HI 불러옴
        public Result GetAllARMREACTORBasicInfoWithRiskMatrixRepo(out List<dynamic> armreactorInfoWithRisk)
        {
            Result res = new Result(true);
            armreactorInfoWithRisk = new List<dynamic>();
            
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = @"
                SELECT 
                    b.TBL_IDX, 
                    b.ARMREACTOR_Code, 
                    b.Serial_No, 
                    b.Install_Date, 
                    b.Operating_Date, 
                    b.REMAIN_LIFE   AS Remain_Life,
                    r.HI
                FROM ARMREACTOR_BASICINFO b
                LEFT JOIN RISKMATRIX r ON b.ARMREACTOR_Code = r.CODE
                ORDER BY b.TBL_IDX";
                    
                    armreactorInfoWithRisk = dbHelper.Conn.Query(query).AsList();
                }
                res.Message = "ARMREACTOR 기본정보와 RISKMATRIX 정보 조회 성공";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = ex.Message;
            }
            return res;
        }

        public Result CreateARMREACTORBasicInfoRepo(ARMREACTORBasicInfo newARMREACTORBasicInfo)
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
                            // ARMREACTOR_BASICINFO 테이블에 데이터 삽입
                            var queryBasicInfo = @"
                INSERT INTO ARMREACTOR_BASICINFO (ARMREACTOR_CODE, SERIAL_NO, NAME, INSTALL_DATE, OPERATING_DATE, PRICE, 
                                             INSTALL_PLACE, CAPACITY, RATED_V, RATED_A, MAKE_COMPANY, MAKE_NO, PHOTO, 
                                             IS_DIAGNOSTICS, REMAIN_LIFE, IS_HEALTH, WRITER) 
                VALUES (@ARMREACTOR_Code, @Serial_No, @Name, @Install_Date, @Operating_Date, @Price, @Install_Place, 
                        @Capacity, @Rated_V, @Rated_A, @Make_Company, @Make_No, @Photo, @Is_Diagnostics, 
                        @Is_Health, @Remain_Life, @Writer)";

                            int affectedRowsBasicInfo = conn.Execute(queryBasicInfo, newARMREACTORBasicInfo, transaction);

                            if (affectedRowsBasicInfo > 0)
                            {
                                // RISKMATRIX 테이블에 데이터 삽입
                                var queryRiskMatrix = @"
                    INSERT INTO RISKMATRIX (CODE, COF, POF) 
                    VALUES (@ARMREACTOR_Code, @DefaultCof, @DefaultPof)";

                                // 초기 COF와 POF 값은 기본값으로 설정 (필요시 변경)
                                var riskMatrixData = new
                                {
                                    ARMREACTOR_Code = newARMREACTORBasicInfo.ARMREACTOR_Code,
                                    DefaultCof = "0",
                                    DefaultPof = "0"
                                };

                                int affectedRowsRiskMatrix = conn.Execute(queryRiskMatrix, riskMatrixData, transaction);

                                if (affectedRowsRiskMatrix > 0)
                                {
                                    transaction.Commit();
                                    res.Message = "CreateARMREACTORBasicInfoRepo 성공: ARMREACTOR Serial_No: " + newARMREACTORBasicInfo.Serial_No;
                                    LogHelper.WriteLog("DB(ARMREACTOR_BASICINFO)", res.Message);
                                }
                                else
                                {
                                    throw new Exception("RISKMATRIX 테이블에 데이터 삽입 실패");
                                }
                            }
                            else
                            {
                                throw new Exception("ARMREACTOR_BASICINFO 테이블에 데이터 삽입 실패");
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            res.IsSuccess = false;
                            res.Message = "CreateARMREACTORBasicInfoRepo 실패: " + ex.Message;
                            LogHelper.WriteLog("DB(ARMREACTOR_BASICINFO)", "CreateARMREACTORBasicInfoRepo 오류: " + ex.Message + " 스택트레이스: " + ex.StackTrace);
                        }
                    }
                }
            }
            return res;
        }

        public Result UpdateARMREACTORBasicInfoRepo(ARMREACTORBasicInfo armreactorBasicInfo)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    // ARMREACTOR_CODE를 기준으로 업데이트
                    var query = @"
            UPDATE ARMREACTOR_BASICINFO
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
            WHERE ARMREACTOR_CODE = @ARMREACTOR_Code";

                    int affectedRows = dbHelper.Conn.Execute(query, armreactorBasicInfo);
                    if (affectedRows > 0)
                    {
                        res.Message = "UpdateARMREACTORBasicInfoRepo 성공. ARMREACTOR_CODE: " + armreactorBasicInfo.ARMREACTOR_Code;
                        LogHelper.WriteLog("DB(ARMREACTOR_BASICINFO)", res.Message);
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "UpdateARMREACTORBasicInfoRepo 실패: 데이터 수정에 실패했습니다.";
                        LogHelper.WriteLog("DB(ARMREACTOR_BASICINFO)", res.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "UpdateARMREACTORBasicInfoRepo 실패: " + ex.StackTrace + ex.Message;
                LogHelper.WriteLog("DB(ARMREACTOR_BASICINFO)", res.Message);
            }
            return res;
        }

        public Result DeleteARMREACTORBasicInfoRepo(string tblIdx)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = "DELETE FROM ARMREACTOR_BASICINFO WHERE TBL_IDX = @Tbl_Idx";
                    int affectedRows = dbHelper.Conn.Execute(query, new { Tbl_Idx = tblIdx });

                    if (affectedRows > 0)
                    {
                        res.Message = "DeleteARMREACTORBasicInfoRepo 성공: ARMREACTORBasicInfo Tbl_Idx: " + tblIdx;
                        LogHelper.WriteLog("DB(ARMREACTOR_BASICINFO)", res.Message);
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "DeleteARMREACTORBasicInfoRepo 실패: 해당 Tbl_Idx를 찾을 수 없습니다.";
                        LogHelper.WriteLog("DB(ARMREACTOR_BASICINFO)", res.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "DeleteARMREACTORBasicInfoRepo 실패: " + ex.Message + "\n" + ex.StackTrace;
                LogHelper.WriteLog("DB(ARMREACTOR_BASICINFO)", res.Message);
            }
            return res;
        }
    }
}
