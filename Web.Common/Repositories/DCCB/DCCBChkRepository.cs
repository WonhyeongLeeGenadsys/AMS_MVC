
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Common
{
    public class DCCBChkRepository
    {

        // 시리얼 번호로 DCCB 보통점검 데이터 조회
        public Result GetDCCBChkByDCCBCode(string dccbCode, out List<DCCBChk> dccbChkList)
        {
            Result res = new Result(true);
            dccbChkList = new List<DCCBChk>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM DCCB_CHK WHERE DCCB_CODE = @DCCB_Code";
                    dccbChkList = dbHelper.Conn
                        .Query<DCCBChk>(query, new { DCCB_Code = dccbCode })
                        .AsList();
                }
                res.Message = $"GetDCCBChkByDCCBCode 성공: DCCB_CODE = {dccbCode}";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetDCCBChkByDCCBCode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(DCCB_CHK)", res.Message);
            }
            return res;
        }

        //최근 점검 데이터 한개만 불러오기 
        public Result GetLatestDCCBChkByDCCBCode(string dccbCode, out List<DCCBChk> dccbChkList)
        {
            Result res = new Result(true);
            dccbChkList = new List<DCCBChk>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                    SELECT TOP 1 *
                    FROM DCCB_CHK
                    WHERE DCCB_CODE = @DCCB_Code
                    ORDER BY CHK_TBL_GETDATE DESC;";

                    dccbChkList = dbHelper.Conn
                        .Query<DCCBChk>(query, new { DCCB_Code = dccbCode })
                        .AsList();
                }

                res.Message = $"GetLatestDCCBChkByDCCBCode 성공(최신 1건): DCCB_CODE = {dccbCode}";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetLatestDCCBChkByDCCBCode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(DCCB_CHK)", res.Message);
            }

            return res;
        }

        public DCCBChk GetDCCBChkByCode(string dccbCode)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT * FROM DCCB_CHK WHERE DCCB_CODE = @DCCB_Code";

                return dbHelper.Conn.QueryFirstOrDefault<DCCBChk>(query, new { DCCB_Code = dccbCode });
            }
        }

        public List<dynamic> GetMonthlyAllDCCBChkCounts()
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                string query = @"
                    SELECT 
                        FORMAT(CHK_Start_Date, 'yyyy-MM') AS Month, 
                        COUNT(*) AS Count,
                        'DCCB' AS Type
                    FROM DCCB_CHK
                    WHERE CHK_Start_Date IS NOT NULL
                    GROUP BY FORMAT(CHK_Start_Date, 'yyyy-MM')
                    ORDER BY Month;";
                return dbHelper.Conn.Query(query).ToList();
            }
        }

        // 전체 DCCB 보통점검 데이터 조회
        public Result GetTotalDCCBChk(out List<DCCBChk> dccbChkList)
        {
            Result res = new Result(true);
            dccbChkList = new List<DCCBChk>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM DCCB_CHK";
                    dccbChkList = dbHelper.Conn.Query<DCCBChk>(query).AsList();
                }
                res.Message = $"GetTotalDCCBChk 성공";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetTotalDCCBChk 실패: {ex.Message}";
                LogHelper.WriteLog("DB(DCCB_CHK)", res.Message);
            }
            return res;
        }

        // 특정 공사명으로 DCCB 보통점검 데이터 조회
        public Result GetDCCBChkDetailByDCCBCode(string dccbCode, string tblIdx, out List<DCCBChk> dccbChkList)
        {
            Result res = new Result(true);
            dccbChkList = new List<DCCBChk>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * 
                FROM DCCB_CHK 
                WHERE DCCB_CODE = @DCCB_Code AND TBL_IDX = @Tbl_Idx";

                    dccbChkList = dbHelper.Conn.Query<DCCBChk>(query, new { DCCB_Code = dccbCode, Tbl_Idx = tblIdx }).AsList();
                    if (dccbChkList.Count == 0)
                    {
                        res.IsSuccess = false;
                        res.Message = "조회 결과가 없습니다.";
                    }
                    else
                    {
                        res.Message = $"GetDCCBChkDetailByDCCBCode 성공: DCCB_CODE = {dccbCode}, TBL_IDX = {tblIdx}";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetDCCBChkDetailByDCCBCode 실패: {ex.Message}";
            }

            return res;
        }



        // DCCB 보통점검 데이터 추가
        public Result CreateDCCBChkRepo(DCCBChk chk)
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
                        INSERT INTO DCCB_CHK (
                            DCCB_CODE,
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
                            CHK_MAINCIRCUIT_INSULATIONSTRENGTH,
                            CHK_LEAKTEST,
                            CHK_MECHANICALOPERATION,
                            CHK_AUXCONTROLCIRCUIT,
                            CHK_CE_VOLTAGE,
                            CHK_G_VOLTAGE,
                            CHK_ON_RESISTANCE,
                            CHK_THERMAL_RESISTANCE,
                            CHK_C_CURRENT,
                            CHK_ONOFF_TIME,
                            FOLDINGFUNCTION,
                            CHK_TBL_GETDATE
                        )
                        VALUES (
                            @DCCB_Code,
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
                            @CHK_MainCircuit_InsulationStrength,
                            @CHK_LeakTest,
                            @CHK_MechanicalOperation,
                            @CHK_AuxControlCircuit,
                            @CHK_CE_Voltage,
                            @CHK_G_Voltage,
                            @CHK_On_Resistance,
                            @CHK_Thermal_Resistance,
                            @CHK_C_Current,
                            @CHK_OnOff_Time,
                            @FoldingFunction,
                            @CHK_Tbl_GetDate
                        )";

                            int affected = conn.Execute(query, chk, transaction);
                            if (affected <= 0)
                                throw new Exception("DCCB_CHK 레코드 삽입 실패");

                            transaction.Commit();
                            res.Message = "DCCB_CHK 등록 성공";
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            res.IsSuccess = false;
                            res.Message = "저장 오류: " + ex.Message;
                            LogHelper.WriteLog("DB(DCCB_CHK)", res.Message);
                        }
                    }
                }
            }
            return res;
        }

        // DCCB 보통점검 데이터 업데이트
        public Result UpdateDCCBChkRepo(DCCBChk dccbChk)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                UPDATE DCCB_CHK 
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
                    CHK_MAINCIRCUIT_INSULATIONSTRENGTH = @CHK_MainCircuit_InsulationStrength,
                    CHK_LEAKTEST = @CHK_LeakTest,
                    CHK_MECHANICALOPERATION = @CHK_MechanicalOperation,
                    CHK_AUXCONTROLCIRCUIT = @CHK_AuxControlCircuit,
                    CHK_CE_VOLTAGE = @CHK_CE_Voltage,
                    CHK_G_VOLTAGE = @CHK_G_Voltage,
                    CHK_ON_RESISTANCE = @CHK_On_Resistance,
                    CHK_THERMAL_RESISTANCE = @CHK_Thermal_Resistance,
                    CHK_C_CURRENT = @CHK_C_Current,
                    CHK_ONOFF_TIME = @CHK_OnOff_Time,
                    CHK_UPDATE_TIME = GETDATE(),
                    FOLDINGFUNCTION = @FoldingFunction

                WHERE DCCB_CODE = @DCCB_Code AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, dccbChk);
                    res.Message = affectedRows > 0 ? "DCCB 보통점검 데이터 업데이트 성공" : "DCCB 보통점검 데이터 업데이트 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"UpdateDCCBChkInfoRepo 실패: {ex.Message}";
            }

            return res;
        }

        // DCCB 보통점검 데이터 삭제
        //public Result DeleteDCCBChkInfoRepo(string dccbCode, string tblIdx)
        //{
        //    Result res = new Result(true);

        //    try
        //    {
        //        using (DBHelper dbHelper = new DBHelper())
        //        {
        //            const string query = "DELETE FROM DCCB_CHK WHERE DCCB_CODE = @DCCB_Code AND TBL_IDX = @Tbl_Idx";

        //            int affectedRows = dbHelper.Conn.Execute(query, new { DCCB_Code = dccbCode, Tbl_Idx = tblIdx });
        //            res.Message = affectedRows > 0 ? "DCCB 보통점검 데이터 삭제 성공" : "DCCB 보통점검 데이터 삭제 실패";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        res.IsSuccess = false;
        //        res.Message = $"DeleteDCCBChkInfoRepo 실패: {ex.Message}";
        //    }
        //    return res;
        //}
        public Result DeleteDCCBChkInfoRepo(string dccbCode, string tblIdx)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    // 1) DCCB_CHK 삭제
                    const string deleteQuery = @"
                DELETE FROM DCCB_CHK 
                WHERE DCCB_CODE = @DCCB_Code AND TBL_IDX = @Tbl_Idx";
                    int affectedRows = dbHelper.Conn.Execute(deleteQuery, new { DCCB_Code = dccbCode, Tbl_Idx = tblIdx });

                    if (affectedRows > 0)
                    {
                        res.Message = "DCCB 보통점검 데이터 삭제 성공";

                        // 2) RISKMATRIX 최신 행의 HI, POF 초기화
                        const string updateRisk = @"
                    UPDATE RISKMATRIX
                    SET HI = 0,
                        POF = 0
                    WHERE CODE = @DCCB_Code
                      AND LASTTIME = (
                          SELECT MAX(LASTTIME) 
                          FROM RISKMATRIX 
                          WHERE CODE = @DCCB_Code
                      )";
                        int riskUpdated = dbHelper.Conn.Execute(updateRisk, new { DCCB_Code = dccbCode });

                        if (riskUpdated > 0)
                            res.Message += " + RISKMATRIX 최신 HI/PoF 초기화";
                        else
                            res.Message += " + RISKMATRIX 업데이트 없음";
                    }
                    else
                    {
                        res.Message = "DCCB 보통점검 데이터 삭제 실패";
                        res.IsSuccess = false;
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"DeleteDCCBChkInfoRepo 실패: {ex.Message}";
            }

            return res;
        }

    }
}
