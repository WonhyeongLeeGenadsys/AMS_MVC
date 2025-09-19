
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Common
{
    public class DCCABLEChkRepository
    {

        // 시리얼 번호로 DCCABLE 보통점검 데이터 조회
        public Result GetDCCABLEChkByDCCABLECode(string dccableCode, out List<DCCABLEChk> dccableChkList)
        {
            Result res = new Result(true);
            dccableChkList = new List<DCCABLEChk>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM DCCABLE_CHK WHERE DCCABLE_CODE = @DCCABLE_Code";
                    dccableChkList = dbHelper.Conn
                        .Query<DCCABLEChk>(query, new { DCCABLE_Code = dccableCode })
                        .AsList();
                }
                res.Message = $"GetDCCABLEChkByDCCABLECode 성공: DCCABLE_CODE = {dccableCode}";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetDCCABLEChkByDCCABLECode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(DCCABLE_CHK)", res.Message);
            }
            return res;
        }

        //최근 점검 데이터 한개만 불러오기 
        public Result GetLatestDCCABLEChkByDCCABLECode(string dccableCode, out List<DCCABLEChk> dccableChkList)
        {
            Result res = new Result(true);
            dccableChkList = new List<DCCABLEChk>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                    SELECT TOP 1 *
                    FROM DCCABLE_CHK
                    WHERE DCCABLE_CODE = @DCCABLE_Code
                    ORDER BY CHK_TBL_GETDATE DESC;";

                    dccableChkList = dbHelper.Conn
                        .Query<DCCABLEChk>(query, new { DCCABLE_Code = dccableCode })
                        .AsList();
                }

                res.Message = $"GetLatestDCCABLEChkByDCCABLECode 성공(최신 1건): DCCABLE_CODE = {dccableCode}";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetLatestDCCABLEChkByDCCABLECode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(DCCABLE_CHK)", res.Message);
            }

            return res;
        }
        public DCCABLEChk GetDCCABLEChkByCode(string dccableCode)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT * FROM DCCABLE_CHK WHERE DCCABLE_CODE = @DCCABLE_Code";

                return dbHelper.Conn.QueryFirstOrDefault<DCCABLEChk>(query, new { DCCABLE_Code = dccableCode });
            }
        }

        public List<dynamic> GetMonthlyAllDCCABLEChkCounts()
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                string query = @"
                    SELECT 
                        FORMAT(CHK_Start_Date, 'yyyy-MM') AS Month, 
                        COUNT(*) AS Count,
                        'DCCABLE' AS Type
                    FROM DCCABLE_CHK
                    WHERE CHK_Start_Date IS NOT NULL
                    GROUP BY FORMAT(CHK_Start_Date, 'yyyy-MM')
                    ORDER BY Month;";
                return dbHelper.Conn.Query(query).ToList();
            }
        }

        // 전체 DCCABLE 보통점검 데이터 조회
        public Result GetTotalDCCABLEChk(out List<DCCABLEChk> dccableChkList)
        {
            Result res = new Result(true);
            dccableChkList = new List<DCCABLEChk>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM DCCABLE_CHK";
                    dccableChkList = dbHelper.Conn.Query<DCCABLEChk>(query).AsList();
                }
                res.Message = $"GetTotalDCCABLEChk 성공";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetTotalDCCABLEChk 실패: {ex.Message}";
                LogHelper.WriteLog("DB(DCCABLE_CHK)", res.Message);
            }
            return res;
        }

        // 특정 공사명으로 DCCABLE 보통점검 데이터 조회
        public Result GetDCCABLEChkDetailByDCCABLECode(string dccableCode, string tblIdx, out List<DCCABLEChk> dccableChkList)
        {
            Result res = new Result(true);
            dccableChkList = new List<DCCABLEChk>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * 
                FROM DCCABLE_CHK 
                WHERE DCCABLE_CODE = @DCCABLE_Code AND TBL_IDX = @Tbl_Idx";

                    dccableChkList = dbHelper.Conn.Query<DCCABLEChk>(query, new { DCCABLE_Code = dccableCode, Tbl_Idx = tblIdx }).AsList();
                    if (dccableChkList.Count == 0)
                    {
                        res.IsSuccess = false;
                        res.Message = "조회 결과가 없습니다.";
                    }
                    else
                    {
                        res.Message = $"GetDCCABLEChkDetailByDCCABLECode 성공: DCCABLE_CODE = {dccableCode}, TBL_IDX = {tblIdx}";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetDCCABLEChkDetailByDCCABLECode 실패: {ex.Message}";
            }

            return res;
        }



        // DCCABLE 보통점검 데이터 추가
        public Result CreateDCCABLEChkRepo(DCCABLEChk chk)
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
                            var query = @"
                        INSERT INTO DCCABLE_CHK (
                            DCCABLE_CODE,
                            CHK_GONGSA_NAME,
                            CHK_WEATHER,
                            CHK_TEMP,
                            CHK_HUM,
                            CHK_COMPANY,
                            CHK_WORKER,
                            CHK_MANAGER,
                            CHK_URGENT_NO,
                            CHK_TYPE,
                            CHK_START_DATE,
                            CHK_END_DATE,
                            CHK_WRITER,
                            CHK_PARTIAL_DISCHARGE,
                            CHK_RATED_VOLTAGE,
                            CHK_TAN_DELTA,
                            CHK_RESISTANCE,
                            CHK_TDR,
                            FOLDINGFUNCTION,
                            CHK_TBL_GETDATE
                        )
                        VALUES (
                            @DCCABLE_Code,
                            @CHK_Gongsa_Name,
                            @CHK_Weather,
                            @CHK_Temp,
                            @CHK_Hum,
                            @CHK_Company,
                            @CHK_Worker,
                            @CHK_Manager,
                            @CHK_Urgent_No,
                            @CHK_Type,
                            @CHK_Start_Date,
                            @CHK_End_Date,
                            @CHK_Writer,
                            @CHK_Partial_Discharge,
                            @CHK_Rated_Voltage,
                            @CHK_Tan_Delta,
                            @CHK_Resistance,
                            @CHK_TDR,
                            @FoldingFunction,
                            @CHK_Tbl_GetDate
                        )";

                            int affected = conn.Execute(query, chk, transaction);
                            if (affected <= 0)
                                throw new Exception("DCCABLE_CHK 레코드 삽입 실패");

                            transaction.Commit();
                            res.Message = "DCCABLE_CHK 등록 성공";
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            res.IsSuccess = false;
                            res.Message = "저장 오류: " + ex.Message;
                            LogHelper.WriteLog("DB(DCCABLE_CHK)", res.Message);
                        }
                    }
                }
            }
            return res;
        }

        // DCCABLE 보통점검 데이터 업데이트
        public Result UpdateDCCABLEChkRepo(DCCABLEChk dccableChk)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                UPDATE DCCABLE_CHK 
                SET 
                    CHK_WEATHER = @CHK_Weather,
                    CHK_TEMP = @CHK_Temp,
                    CHK_HUM = @CHK_Hum,
                    CHK_COMPANY = @CHK_Company,
                    CHK_WORKER = @CHK_Worker,
                    CHK_MANAGER = @CHK_Manager,
                    CHK_URGENT_NO = @CHK_Urgent_No,
                    CHK_TYPE = @CHK_Type,
                    CHK_START_DATE = @CHK_Start_Date,
                    CHK_END_DATE = @CHK_End_Date,
                    CHK_WRITER = @CHK_Writer,
                    CHK_PARTIAL_DISCHARGE = @CHK_Partial_Discharge,
                    CHK_RATED_VOLTAGE = @CHK_Rated_Voltage,
                    CHK_TAN_DELTA = @CHK_Tan_Delta,
                    CHK_RESISTANCE = @CHK_Resistance,
                    CHK_TDR = @CHK_TDR,
                    CHK_UPDATE_TIME = GETDATE(),
                    FOLDINGFUNCTION = @FoldingFunction
                    
                WHERE DCCABLE_CODE = @DCCABLE_Code AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, dccableChk);
                    res.Message = affectedRows > 0 ? "DCCABLE 보통점검 데이터 업데이트 성공" : "DCCABLE 보통점검 데이터 업데이트 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"UpdateDCCABLEChkInfoRepo 실패: {ex.Message}";
            }

            return res;
        }

        // DCCABLE 보통점검 데이터 삭제
        //public Result DeleteDCCABLEChkInfoRepo(string dccableCode, string tblIdx)
        //{
        //    Result res = new Result(true);

        //    try
        //    {
        //        using (DBHelper dbHelper = new DBHelper())
        //        {
        //            const string query = "DELETE FROM DCCABLE_CHK WHERE DCCABLE_CODE = @DCCABLE_Code AND TBL_IDX = @Tbl_Idx";

        //            int affectedRows = dbHelper.Conn.Execute(query, new { DCCABLE_Code = dccableCode, Tbl_Idx = tblIdx });
        //            res.Message = affectedRows > 0 ? "DCCABLE 보통점검 데이터 삭제 성공" : "DCCABLE 보통점검 데이터 삭제 실패";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        res.IsSuccess = false;
        //        res.Message = $"DeleteDCCABLEChkInfoRepo 실패: {ex.Message}";
        //    }
        //    return res;
        //}
        public Result DeleteDCCABLEChkInfoRepo(string dccableCode, string tblIdx)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    // 1) DCCABLE_CHK 삭제
                    const string deleteQuery = @"
                DELETE FROM DCCABLE_CHK 
                WHERE DCCABLE_CODE = @DCCABLE_Code AND TBL_IDX = @Tbl_Idx";
                    int affectedRows = dbHelper.Conn.Execute(deleteQuery, new { DCCABLE_Code = dccableCode, Tbl_Idx = tblIdx });

                    if (affectedRows > 0)
                    {
                        res.Message = "DCCABLE 보통점검 데이터 삭제 성공";

                        // 2) RISKMATRIX 최신 행의 HI, POF 초기화
                        const string updateRisk = @"
                    UPDATE RISKMATRIX
                    SET HI = 0,
                        POF = 0
                    WHERE CODE = @DCCABLE_Code
                      AND LASTTIME = (
                          SELECT MAX(LASTTIME) 
                          FROM RISKMATRIX 
                          WHERE CODE = @DCCABLE_Code
                      )";
                        int riskUpdated = dbHelper.Conn.Execute(updateRisk, new { DCCABLE_Code = dccableCode });

                        if (riskUpdated > 0)
                            res.Message += " + RISKMATRIX 최신 HI/PoF 초기화";
                        else
                            res.Message += " + RISKMATRIX 업데이트 없음";
                    }
                    else
                    {
                        res.Message = "DCCABLE 보통점검 데이터 삭제 실패";
                        res.IsSuccess = false;
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"DeleteDCCABLEChkInfoRepo 실패: {ex.Message}";
            }

            return res;
        }

    }
}
