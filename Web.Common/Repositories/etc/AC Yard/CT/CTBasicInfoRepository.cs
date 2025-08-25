
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Common
{
    public class CTBasicInfoRepository
    {
        // 가장 큰 CT_CODE 값을 반환
        public string GetLatestCTCode()
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT MAX(CT_CODE) FROM CT_BASICINFO WHERE CT_CODE LIKE 'CT%'";
                return dbHelper.Conn.QuerySingleOrDefault<string>(query);
            }
        }

        public CTBasicInfo GetCTBasicInfoByTblIdxRepo(string tblIdx)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT * FROM CT_BASICINFO WHERE TBL_IDX = @Tbl_Idx";
                return dbHelper.Conn.QueryFirstOrDefault<CTBasicInfo>(query, new { Tbl_Idx = tblIdx });
            }
        }

        public CTBasicInfo GetCTBasicInfoByCode(string ctCode)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT * FROM CT_BASICINFO WHERE CT_CODE = @CT_Code";
                return dbHelper.Conn.QueryFirstOrDefault<CTBasicInfo>(query, new { CT_Code = ctCode });
            }
        }

        /// <summary>
        /// CT 기본정보 전체 불러오기
        /// </summary>
        public Result GetAllCTBasicInfoRepo(out List<CTBasicInfo> ctBasicInfo)
        {
            Result res = new Result(true);
            ctBasicInfo = new List<CTBasicInfo>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = @"SELECT TBL_IDX, CT_CODE, SERIAL_NO, NAME, INSTALL_DATE, OPERATING_DATE, PRICE, 
                                         INSTALL_PLACE, RATED_V, RATED_A, MAKE_COMPANY, MAKE_NO, PHOTO, IS_DIAGNOSTICS, 
                                         IS_HEALTH, REMAIN_LIFE, WRITER, TBL_GETDATE 
                                  FROM CT_BASICINFO";
                    ctBasicInfo = dbHelper.Conn.Query<CTBasicInfo>(query).AsList();

                    LogHelper.WriteLog("ctBasicInfo Data", $"총 {ctBasicInfo.Count} 건 조회됨");
                    res.Message = "GetAllCTBasicInfoRepo 동작 성공";
                    LogHelper.WriteLog("DB(CT_BASICINFO)", res.Message);
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "GetAllCTBasicInfoRepo 실패: " + ex.StackTrace + ex.Message;
                LogHelper.WriteLog("DB(CT_BASICINFO)", res.Message);
            }
            return res;
        }

        //CT Device페이지에서 '설비들' 표시하기 위해 사용 Basic 모델과 RiskMatrix HI 불러옴
        public Result GetAllCTBasicInfoWithRiskMatrixRepo(out List<dynamic> ctInfoWithRisk)
        {
            Result res = new Result(true);
            ctInfoWithRisk = new List<dynamic>();
            
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = @"
                SELECT 
                    b.TBL_IDX, 
                    b.CT_Code, 
                    b.Serial_No, 
                    b.Install_Date, 
                    b.Operating_Date, 
                    b.REMAIN_LIFE   AS Remain_Life,
                    r.HI
                FROM CT_BASICINFO b
                LEFT JOIN RISKMATRIX r ON b.CT_Code = r.CODE
                ORDER BY b.TBL_IDX";
                    
                    ctInfoWithRisk = dbHelper.Conn.Query(query).AsList();
                }
                res.Message = "CT 기본정보와 RISKMATRIX 정보 조회 성공";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = ex.Message;
            }
            return res;
        }

        public Result CreateCTBasicInfoRepo(CTBasicInfo newCTBasicInfo)
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
                            // CT_BASICINFO 테이블에 데이터 삽입
                            var queryBasicInfo = @"
                INSERT INTO CT_BASICINFO (CT_CODE, SERIAL_NO, NAME, INSTALL_DATE, OPERATING_DATE, PRICE, 
                                             INSTALL_PLACE, CAPACITY, RATED_V, RATED_A, MAKE_COMPANY, MAKE_NO, PHOTO, 
                                             IS_DIAGNOSTICS, REMAIN_LIFE, IS_HEALTH, WRITER) 
                VALUES (@CT_Code, @Serial_No, @Name, @Install_Date, @Operating_Date, @Price, @Install_Place, 
                        @Capacity, @Rated_V, @Rated_A, @Make_Company, @Make_No, @Photo, @Is_Diagnostics, 
                        @Is_Health, @Remain_Life, @Writer)";

                            int affectedRowsBasicInfo = conn.Execute(queryBasicInfo, newCTBasicInfo, transaction);

                            if (affectedRowsBasicInfo > 0)
                            {
                                // RISKMATRIX 테이블에 데이터 삽입
                                var queryRiskMatrix = @"
                    INSERT INTO RISKMATRIX (CODE, COF, POF) 
                    VALUES (@CT_Code, @DefaultCof, @DefaultPof)";

                                // 초기 COF와 POF 값은 기본값으로 설정 (필요시 변경)
                                var riskMatrixData = new
                                {
                                    CT_Code = newCTBasicInfo.CT_Code,
                                    DefaultCof = "0",
                                    DefaultPof = "0"
                                };

                                int affectedRowsRiskMatrix = conn.Execute(queryRiskMatrix, riskMatrixData, transaction);

                                if (affectedRowsRiskMatrix > 0)
                                {
                                    transaction.Commit();
                                    res.Message = "CreateCTBasicInfoRepo 성공: CT Serial_No: " + newCTBasicInfo.Serial_No;
                                    LogHelper.WriteLog("DB(CT_BASICINFO)", res.Message);
                                }
                                else
                                {
                                    throw new Exception("RISKMATRIX 테이블에 데이터 삽입 실패");
                                }
                            }
                            else
                            {
                                throw new Exception("CT_BASICINFO 테이블에 데이터 삽입 실패");
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            res.IsSuccess = false;
                            res.Message = "CreateCTBasicInfoRepo 실패: " + ex.Message;
                            LogHelper.WriteLog("DB(CT_BASICINFO)", "CreateCTBasicInfoRepo 오류: " + ex.Message + " 스택트레이스: " + ex.StackTrace);
                        }
                    }
                }
            }
            return res;
        }

        public Result UpdateCTBasicInfoRepo(CTBasicInfo ctBasicInfo)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    // CT_CODE를 기준으로 업데이트
                    var query = @"
            UPDATE CT_BASICINFO
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
            WHERE CT_CODE = @CT_Code";

                    int affectedRows = dbHelper.Conn.Execute(query, ctBasicInfo);
                    if (affectedRows > 0)
                    {
                        res.Message = "UpdateCTBasicInfoRepo 성공. CT_CODE: " + ctBasicInfo.CT_Code;
                        LogHelper.WriteLog("DB(CT_BASICINFO)", res.Message);
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "UpdateCTBasicInfoRepo 실패: 데이터 수정에 실패했습니다.";
                        LogHelper.WriteLog("DB(CT_BASICINFO)", res.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "UpdateCTBasicInfoRepo 실패: " + ex.StackTrace + ex.Message;
                LogHelper.WriteLog("DB(CT_BASICINFO)", res.Message);
            }
            return res;
        }

        public Result DeleteCTBasicInfoRepo(string tblIdx)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = "DELETE FROM CT_BASICINFO WHERE TBL_IDX = @Tbl_Idx";
                    int affectedRows = dbHelper.Conn.Execute(query, new { Tbl_Idx = tblIdx });

                    if (affectedRows > 0)
                    {
                        res.Message = "DeleteCTBasicInfoRepo 성공: CTBasicInfo Tbl_Idx: " + tblIdx;
                        LogHelper.WriteLog("DB(CT_BASICINFO)", res.Message);
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "DeleteCTBasicInfoRepo 실패: 해당 Tbl_Idx를 찾을 수 없습니다.";
                        LogHelper.WriteLog("DB(CT_BASICINFO)", res.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "DeleteCTBasicInfoRepo 실패: " + ex.Message + "\n" + ex.StackTrace;
                LogHelper.WriteLog("DB(CT_BASICINFO)", res.Message);
            }
            return res;
        }
    }
}
