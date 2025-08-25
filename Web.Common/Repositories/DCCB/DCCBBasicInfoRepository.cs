
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Common
{
    public class DCCBBasicInfoRepository
    {
        // 가장 큰 DCCB_CODE 값을 반환
        public string GetLatestDCCBCode()
        {           
            using(DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT MAX(DCCB_CODE) FROM DCCB_BASICINFO WHERE DCCB_CODE LIKE 'D%'";

                return dbHelper.Conn.QuerySingleOrDefault<string>(query);
            }
        }

        public DCCBBasicInfo GetDCCBBasicInfoByTblIdxRepo(string tblIdx)
        {
            using(DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT * FROM DCCB_BASICINFO WHERE TBL_IDX = @Tbl_Idx";

                return dbHelper.Conn.QueryFirstOrDefault<DCCBBasicInfo>(query, new { Tbl_Idx = tblIdx });
            }
        }

        public DCCBBasicInfo GetDCCBBasicInfoByCode(string dccbCode)
        {
            using(DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT * FROM DCCB_BASICINFO WHERE DCCB_CODE = @DCCB_Code";

                return dbHelper.Conn.QueryFirstOrDefault<DCCBBasicInfo>(query, new { DCCB_Code = dccbCode });
            }
        }

        /// <summary>
        /// DCCB 기본정보 전체 불러오기
        /// </summary>
        /// <param name="dccbBasicInfo"></param>
        /// <returns></returns>
        public Result GetAllDCCBBasicInfoRepo(out List<DCCBBasicInfo> dccbBasicInfo)
        {
            Result res = new Result(true);
            dccbBasicInfo = new List<DCCBBasicInfo>();

            try
            {
                using(DBHelper dbHelper = new DBHelper())
                {
                    var query = "SELECT TBL_IDX, DCCB_CODE, SERIAL_NO, NAME, INSTALL_DATE, OPERATING_DATE, PRICE, INSTALL_PLACE, RATED_V, RATED_A, MAKE_COMPANY, MAKE_NO, IS_DIAGNOSTICS, IS_HEALTH, WRITER, TBL_GETDATE FROM DCCB_BASICINFO";
                    dccbBasicInfo = dbHelper.Conn.Query<DCCBBasicInfo>(query).AsList();

                    LogHelper.WriteLog("dccbBasicInfo Data", $"{dccbBasicInfo}");
                    res.Message = "GetAllDCCBBasicInfoRepo 동작 성공";
                    LogHelper.WriteLog("DB(DCCB_BASICINFO", res.Message);
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "GetAllDCCBBasicInfoRepo 실패: " + ex.StackTrace + ex.Message;

                LogHelper.WriteLog("DB(DCCB_BASICINFO)", res.Message);
            }
            return res;
        }

        //DCCB Device페이지에서 '설비들' 표시하기 위해 사용 Basic 모델과 RiskMatrix HI 불러옴
        public Result GetAllDCCBBasicInfoWithRiskMatrixRepo(out List<dynamic> dccbInfoWithRisk)
        {
            Result res = new Result(true);
            dccbInfoWithRisk = new List<dynamic>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = @"
                SELECT 
                    b.TBL_IDX, 
                    b.DCCB_Code, 
                    b.Serial_No, 
                    b.Install_Date, 
                    b.Operating_Date, 
                    r.HI
                FROM DCCB_BASICINFO b
                LEFT JOIN RISKMATRIX r ON b.DCCB_Code = r.CODE
                ORDER BY b.TBL_IDX";

                    dccbInfoWithRisk = dbHelper.Conn.Query(query).AsList();
                }
                res.Message = "DCCB 기본정보와 RISKMATRIX 정보 조회 성공";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = ex.Message;
            }
            return res;
        }

        public Result CreateDCCBBasicInfoRepo(DCCBBasicInfo newDCCBBasicInfo)
        {
            Result res = new Result(true);
            using(DBHelper dbHelper = new DBHelper())
            {
                using (var conn = dbHelper.Conn)
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // DCCB_BASICINFO 테이블에 데이터 삽입
                            var queryBasicInfo = @"
                INSERT INTO DCCB_BASICINFO (DCCB_CODE, SERIAL_NO, NAME, INSTALL_DATE, OPERATING_DATE, PRICE, 
                INSTALL_PLACE, CAPACITY, RATED_V, RATED_A, MAKE_COMPANY, MAKE_NO, PHOTO, IS_DIAGNOSTICS, 
                IS_HEALTH, WRITER) 
                VALUES (@DCCB_Code, @Serial_No, @Name, @Install_Date, @Operating_Date, @Price, @Install_Place, 
                @Capacity, @Rated_V, @Rated_A, @Make_Company, @Make_No, @Photo, @Is_Diagnostics, 
                @Is_Health, @Writer)";

                            int affectedRowsBasicInfo = conn.Execute(queryBasicInfo, newDCCBBasicInfo, transaction);

                            if (affectedRowsBasicInfo > 0)
                            {
                                // RISKMATRIX 테이블에 데이터 삽입
                                var queryRiskMatrix = @"
                    INSERT INTO RISKMATRIX (CODE, COF, POF) 
                    VALUES (@DCCB_Code, @DefaultCof, @DefaultPof)";

                                // 초기 COF와 POF 값은 기본값으로 설정 (변경해야됨)
                                var riskMatrixData = new
                                {
                                    DCCB_Code = newDCCBBasicInfo.DCCB_Code,
                                    DefaultCof = "0",
                                    DefaultPof = "0"
                                };

                                int affectedRowsRiskMatrix = conn.Execute(queryRiskMatrix, riskMatrixData, transaction);

                                if (affectedRowsRiskMatrix > 0)
                                {
                                    // 트랜잭션 커밋
                                    transaction.Commit();
                                    res.Message = "CreateDCCBBasicInfoRepo 성공: DCCB Serial_No: " + newDCCBBasicInfo.Serial_No;
                                    LogHelper.WriteLog("DB(DCCB_BASICINFO)", res.Message);
                                }
                                else
                                {
                                    throw new Exception("RISKMATRIX 테이블에 데이터 삽입 실패");
                                }
                            }
                            else
                            {
                                throw new Exception("DCCB_BASICINFO 테이블에 데이터 삽입 실패");
                            }
                        }
                        catch (Exception ex)
                        {
                            // 트랜잭션 롤백
                            transaction.Rollback();
                            res.IsSuccess = false;
                            res.Message = "CreateDCCBBasicInfoRepo 실패: " + ex.Message;
                            LogHelper.WriteLog("DB(DCCB_BASICINFO)", "CreateDCCBBasicInfoRepo 오류: " + ex.Message + " 스택트레이스: " + ex.StackTrace);
                        }
                    }
                }
            }
            return res;
        }

        public Result UpdateDCCBBasicInfoRepo(DCCBBasicInfo dccbBasicInfo)
        {
            Result res = new Result(true);
            try
            {
                using(DBHelper dbHelper = new DBHelper())
                {
                    var query = "UPDATE DCCB_BASICINFO SET NAME = @Name, INSTALL_DATE = @Install_Date, OPERATING_DATE = @Operating_Date, PRICE=@Price, INSTALL_PLACE=@Install_Place, CAPACITY=@Capacity, RATED_V=@Rated_V, RATED_A=@Rated_A, MAKE_COMPANY=@Make_Company, MAKE_NO=@Make_No, PHOTO=@Photo, IS_DIAGNOSTICS=@Is_Diagnostics, IS_HEALTH=@Is_Health, WRITER=@Writer " +
            "WHERE SERIAL_NO = @Serial_No";

                    int affectedRows = dbHelper.Conn.Execute(query, dccbBasicInfo);
                    if (affectedRows > 0)
                    {
                        res.Message = "UpdateDCCBBasicInfoRepo 성공 SERIAL_NO: " + dccbBasicInfo.Serial_No;
                        LogHelper.WriteLog("DB(DCCB_BasicInfo)", res.Message);
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "UpdateDCCBBasicInfoRepo 실패: 데이터 수정에 실패했습니다.";
                        LogHelper.WriteLog("DB(DCCB_BasicInfo)", res.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "UpdateDCCBBasicInfoRepo 실패: " + ex.StackTrace + ex.Message;
                LogHelper.WriteLog("DB(DCCB_BasicInfo)", res.Message);
            }
            return res;
        }

        public Result DeleteDCCBBasicInfoRepo(string tblIdx)
        {
            Result res = new Result(true);
            try
            {
                using(DBHelper dbHelper = new DBHelper())
                {
                    var query = "DELETE FROM DCCB_BASICINFO WHERE TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, new { Tbl_Idx = tblIdx });

                    if (affectedRows > 0)
                    {
                        res.Message = "DeleteDCCBBasicInfoRepo 성공: DCCBBasicInfo Tbl_Idx: " + tblIdx;
                        LogHelper.WriteLog("DB(DCCB_BASICINFO)", res.Message);
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "DeleteDCCBBasicInfoRepo 실패: 해당 Tbl_Idx를 찾을 수 없습니다.";
                        LogHelper.WriteLog("DB(DCCB_BASICINFO)", res.Message);
                    }
                }


            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "DeleteDCCBBasicInfoRepo 실패: " + ex.Message + "\n" + ex.StackTrace;
                LogHelper.WriteLog("DB(DCCB_BASICINFO)", res.Message);
            }
            return res;
        }
    }
}