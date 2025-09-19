
using Dapper;
using System;
using System.Collections.Generic;

namespace Web.Common
{
    public class VCBBasicInfoRepository
    {
        // 가장 큰 VCB_CODE 값을 반환
        public string GetLatestVCBCode()
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT MAX(VCB_CODE) FROM VCB_BASICINFO WHERE VCB_CODE LIKE 'VCB%'";
                return dbHelper.Conn.QuerySingleOrDefault<string>(query);
            }
        }

        public VCBBasicInfo GetVCBBasicInfoByTblIdxRepo(string tblIdx)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT * FROM VCB_BASICINFO WHERE TBL_IDX = @Tbl_Idx";
                return dbHelper.Conn.QueryFirstOrDefault<VCBBasicInfo>(query, new { Tbl_Idx = tblIdx });
            }
        }

        public VCBBasicInfo GetVCBBasicInfoByCode(string vcbCode)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT * FROM VCB_BASICINFO WHERE VCB_CODE = @VCB_Code";
                return dbHelper.Conn.QueryFirstOrDefault<VCBBasicInfo>(query, new { VCB_Code = vcbCode });
            }
        }

        /// <summary>
        /// VCB 기본정보 전체 불러오기
        /// </summary>
        public Result GetAllVCBBasicInfoRepo(out List<VCBBasicInfo> vcbBasicInfo)
        {
            Result res = new Result(true);
            vcbBasicInfo = new List<VCBBasicInfo>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = @"SELECT TBL_IDX, VCB_CODE, SERIAL_NO, NAME, INSTALL_DATE, OPERATING_DATE, PRICE, 
                                         INSTALL_PLACE, RATED_V, RATED_A, MAKE_COMPANY, MAKE_NO, PHOTO, IS_DIAGNOSTICS, 
                                         IS_HEALTH, WRITER, TBL_GETDATE 
                                  FROM VCB_BASICINFO";
                    vcbBasicInfo = dbHelper.Conn.Query<VCBBasicInfo>(query).AsList();

                    LogHelper.WriteLog("vcbBasicInfo Data", $"총 {vcbBasicInfo.Count} 건 조회됨");
                    res.Message = "GetAllVCBBasicInfoRepo 동작 성공";
                    LogHelper.WriteLog("DB(VCB_BASICINFO)", res.Message);
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "GetAllVCBBasicInfoRepo 실패: " + ex.StackTrace + ex.Message;
                LogHelper.WriteLog("DB(VCB_BASICINFO)", res.Message);
            }
            return res;
        }

        //VCB Device페이지에서 '설비들' 표시하기 위해 사용 Basic 모델과 RiskMatrix HI 불러옴
        public Result GetAllVCBBasicInfoWithRiskMatrixRepo(out List<dynamic> vcbInfoWithRisk)
        {
            Result res = new Result(true);
            vcbInfoWithRisk = new List<dynamic>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    // 각 CODE별로 LASTTIME 기준 최신 행만 뽑아서 JOIN
                    var query = @"
                    SELECT 
                        b.TBL_IDX, 
                        b.VCB_Code, 
                        b.Serial_No, 
                        b.Install_Date, 
                        b.Operating_Date, 
                        r_latest.HI
                    FROM VCB_BASICINFO b
                    LEFT JOIN (
                        SELECT CODE, HI
                        FROM (
                            SELECT 
                                CODE, 
                                HI,
                                ROW_NUMBER() OVER(PARTITION BY CODE ORDER BY LASTTIME DESC) AS rn
                            FROM RISKMATRIX
                        ) t
                        WHERE t.rn = 1
                    ) r_latest
                        ON b.VCB_Code = r_latest.CODE
                    ORDER BY b.TBL_IDX;
                    ";

                    vcbInfoWithRisk = dbHelper.Conn.Query(query).AsList();
                }
                res.Message = "VCB 기본정보와 최신 RISKMATRIX 정보 조회 성공";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = ex.Message;
            }
            return res;
        }

        public Result CreateVCBBasicInfoRepo(VCBBasicInfo newVCBBasicInfo)
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
                            // VCB_BASICINFO 테이블에 데이터 삽입
                            var queryBasicInfo = @"
                            INSERT INTO VCB_BASICINFO (VCB_CODE, SERIAL_NO, NAME, INSTALL_DATE, OPERATING_DATE, PRICE, 
                                             INSTALL_PLACE, CAPACITY, RATED_V, RATED_A, MAKE_COMPANY, MAKE_NO, PHOTO, 
                                             IS_DIAGNOSTICS, IS_HEALTH, WRITER) 
                            VALUES (@VCB_Code, @Serial_No, @Name, @Install_Date, @Operating_Date, @Price, @Install_Place, 
                                    @Capacity, @Rated_V, @Rated_A, @Make_Company, @Make_No, @Photo, @Is_Diagnostics, 
                                    @Is_Health, @Writer)";

                            int affectedRowsBasicInfo = conn.Execute(queryBasicInfo, newVCBBasicInfo, transaction);

                            if (affectedRowsBasicInfo > 0)
                            {
                                // RISKMATRIX 테이블에 데이터 삽입
                                var queryRiskMatrix = @"
                                INSERT INTO RISKMATRIX (CODE, COF, POF,LASTTIME) 
                                VALUES (@VCB_Code, @DefaultCof, @DefaultPof, GETDATE())";

                                var riskMatrixData = new
                                {
                                    VCB_Code = newVCBBasicInfo.VCB_Code,
                                    DefaultCof = "0",
                                    DefaultPof = "0"
                                };

                                int affectedRowsRiskMatrix = conn.Execute(queryRiskMatrix, riskMatrixData, transaction);

                                if (affectedRowsRiskMatrix > 0)
                                {
                                    transaction.Commit();
                                    res.Message = "CreateVCBBasicInfoRepo 성공: VCB Serial_No: " + newVCBBasicInfo.Serial_No;
                                    LogHelper.WriteLog("DB(VCB_BASICINFO)", res.Message);
                                }
                                else
                                {
                                    throw new Exception("RISKMATRIX 테이블에 데이터 삽입 실패");
                                }
                            }
                            else
                            {
                                throw new Exception("VCB_BASICINFO 테이블에 데이터 삽입 실패");
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            res.IsSuccess = false;
                            res.Message = "CreateVCBBasicInfoRepo 실패: " + ex.Message;
                            LogHelper.WriteLog("DB(VCB_BASICINFO)", "CreateVCBBasicInfoRepo 오류: " + ex.Message + " 스택트레이스: " + ex.StackTrace);
                        }
                    }
                }
            }
            return res;
        }

        public Result UpdateVCBBasicInfoRepo(VCBBasicInfo vcbBasicInfo)
        {
            Result res = new Result(true);
            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    // VCB_CODE를 기준으로 업데이트
                    var query = @"
                    UPDATE VCB_BASICINFO
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
                    WHERE VCB_CODE = @VCB_Code";

                    int affectedRows = dbHelper.Conn.Execute(query, vcbBasicInfo);
                    if (affectedRows > 0)
                    {
                        res.Message = "UpdateVCBBasicInfoRepo 성공. VCB_CODE: " + vcbBasicInfo.VCB_Code;
                        LogHelper.WriteLog("DB(VCB_BASICINFO)", res.Message);
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "UpdateVCBBasicInfoRepo 실패: 데이터 수정에 실패했습니다.";
                        LogHelper.WriteLog("DB(VCB_BASICINFO)", res.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "UpdateVCBBasicInfoRepo 실패: " + ex.StackTrace + ex.Message;
                LogHelper.WriteLog("DB(VCB_BASICINFO)", res.Message);
            }
            return res;
        }

        public Result DeleteVCBBasicInfoRepo(string tblIdx)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    // 1) 먼저 VCB_CODE 조회
                    const string getCodeQuery = "SELECT VCB_CODE FROM VCB_BASICINFO WHERE TBL_IDX = @Tbl_Idx";
                    var vcbCode = dbHelper.Conn.QueryFirstOrDefault<string>(getCodeQuery, new { Tbl_Idx = tblIdx });

                    if (string.IsNullOrEmpty(vcbCode))
                    {
                        res.IsSuccess = false;
                        res.Message = "DeleteVCBBasicInfoRepo 실패: 해당 Tbl_Idx의 장비를 찾을 수 없습니다.";
                        LogHelper.WriteLog("DB(VCB_BASICINFO)", res.Message);
                        return res;
                    }

                    // 2) VCB_BASICINFO 삭제
                    const string deleteBasicQuery = "DELETE FROM VCB_BASICINFO WHERE TBL_IDX = @Tbl_Idx";
                    int affectedRows = dbHelper.Conn.Execute(deleteBasicQuery, new { Tbl_Idx = tblIdx });

                    if (affectedRows > 0)
                    {
                        // 3) RISKMATRIX에서 해당 CODE 모든 행 삭제
                        const string deleteRiskQuery = "DELETE FROM RISKMATRIX WHERE CODE = @VCB_Code";
                        int riskDeleted = dbHelper.Conn.Execute(deleteRiskQuery, new { VCB_Code = vcbCode });

                        res.Message = $"DeleteVCBBasicInfoRepo 성공: Tbl_Idx={tblIdx}, VCB_CODE={vcbCode}, " +
                                      $"RISKMATRIX {riskDeleted}건 삭제됨";
                        LogHelper.WriteLog("DB(VCB_BASICINFO)", res.Message);
                    }
                    else
                    {
                        res.IsSuccess = false;
                        res.Message = "DeleteVCBBasicInfoRepo 실패: 해당 Tbl_Idx를 찾을 수 없습니다.";
                        LogHelper.WriteLog("DB(VCB_BASICINFO)", res.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "DeleteVCBBasicInfoRepo 오류: " + ex.Message + "\n" + ex.StackTrace;
                LogHelper.WriteLog("DB(VCB_BASICINFO)", res.Message);
            }

            return res;
        }
    }
}
