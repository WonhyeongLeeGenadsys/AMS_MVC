
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Common
{
    public class SUBMODULEChkRepository
    {

        // 시리얼 번호로 SUBMODULE 보통점검 데이터 조회
        public Result GetSUBMODULEChkBySUBMODULECode(string submoduleCode, out List<SUBMODULEChk> submoduleChkList)
        {
            Result res = new Result(true);
            submoduleChkList = new List<SUBMODULEChk>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM SUBMODULE_CHK WHERE SUBMODULE_CODE = @SUBMODULE_Code";
                    submoduleChkList = dbHelper.Conn
                        .Query<SUBMODULEChk>(query, new { SUBMODULE_Code = submoduleCode })
                        .AsList();
                }
                res.Message = $"GetSUBMODULEChkBySUBMODULECode 성공: SUBMODULE_CODE = {submoduleCode}";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetSUBMODULEChkBySUBMODULECode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(SUBMODULE_CHK)", res.Message);
            }
            return res;
        }

        //최근 점검 데이터 한개만 불러오기 
        public Result GetLatestSUBMODULEChkBySUBMODULECode(string submoduleCode, out List<SUBMODULEChk> submoduleChkList)
        {
            Result res = new Result(true);
            submoduleChkList = new List<SUBMODULEChk>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                    SELECT TOP 1 *
                    FROM SUBMODULE_CHK
                    WHERE SUBMODULE_CODE = @SUBMODULE_Code
                    ORDER BY CHK_TBL_GETDATE DESC;";

                    submoduleChkList = dbHelper.Conn
                        .Query<SUBMODULEChk>(query, new { SUBMODULE_Code = submoduleCode })
                        .AsList();
                }

                res.Message = $"GetLatestSUBMODULEChkBySUBMODULECode 성공(최신 1건): SUBMODULE_CODE = {submoduleCode}";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetLatestSUBMODULEChkBySUBMODULECode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(SUBMODULE_CHK)", res.Message);
            }

            return res;
        }


        public SUBMODULEChk GetSUBMODULEChkByCode(string submoduleCode)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = "SELECT * FROM SUBMODULE_CHK WHERE SUBMODULE_CODE = @SUBMODULE_Code";

                return dbHelper.Conn.QueryFirstOrDefault<SUBMODULEChk>(query, new { SUBMODULE_Code = submoduleCode });
            }
        }

        public List<dynamic> GetMonthlyAllSUBMODULEChkCounts()
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                string query = @"
                    SELECT 
                        FORMAT(CHK_Start_Date, 'yyyy-MM') AS Month, 
                        COUNT(*) AS Count,
                        'SUBMODULE' AS Type
                    FROM SUBMODULE_CHK
                    WHERE CHK_Start_Date IS NOT NULL
                    GROUP BY FORMAT(CHK_Start_Date, 'yyyy-MM')
                    ORDER BY Month;";
                return dbHelper.Conn.Query(query).ToList();
            }
        }

        // 전체 SUBMODULE 보통점검 데이터 조회
        public Result GetTotalSUBMODULEChk(out List<SUBMODULEChk> submoduleChkList)
        {
            Result res = new Result(true);
            submoduleChkList = new List<SUBMODULEChk>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM SUBMODULE_CHK";
                    submoduleChkList = dbHelper.Conn.Query<SUBMODULEChk>(query).AsList();
                }
                res.Message = $"GetTotalSUBMODULEChk 성공";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetTotalSUBMODULEChk 실패: {ex.Message}";
                LogHelper.WriteLog("DB(SUBMODULE_CHK)", res.Message);
            }
            return res;
        }

        // 특정 공사명으로 SUBMODULE 보통점검 데이터 조회
        public Result GetSUBMODULEChkDetailBySUBMODULECode(string submoduleCode, string tblIdx, out List<SUBMODULEChk> submoduleChkList)
        {
            Result res = new Result(true);
            submoduleChkList = new List<SUBMODULEChk>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                SELECT * 
                FROM SUBMODULE_CHK 
                WHERE SUBMODULE_CODE = @SUBMODULE_Code AND TBL_IDX = @Tbl_Idx";

                    submoduleChkList = dbHelper.Conn.Query<SUBMODULEChk>(query, new { SUBMODULE_Code = submoduleCode, Tbl_Idx = tblIdx }).AsList();
                    if (submoduleChkList.Count == 0)
                    {
                        res.IsSuccess = false;
                        res.Message = "조회 결과가 없습니다.";
                    }
                    else
                    {
                        res.Message = $"GetSUBMODULEChkDetailBySUBMODULECode 성공: SUBMODULE_CODE = {submoduleCode}, TBL_IDX = {tblIdx}";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetSUBMODULEChkDetailBySUBMODULECode 실패: {ex.Message}";
            }

            return res;
        }



        // SUBMODULE 보통점검 데이터 추가
        public Result CreateSUBMODULEChkRepo(SUBMODULEChk chk)
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
                        INSERT INTO SUBMODULE_CHK (
                            SUBMODULE_CODE,
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
                            CHK_CE_VOLTAGE,
                            CHK_G_VOLTAGE,
                            CHK_ON_RESISTANCE,
                            CHK_THERMAL_RESISTANCE,
                            CHK_C_CURRENT,
                            CHK_ONOFF_TIME,
                            CHK_INSULATION_RESISTANCE,
                            CHK_ESR,
                            CHK_CAPACITANCE,
                            CHK_TEMPERATURE,
                            FOLDINGFUNCTION,
                            CHK_TBL_GETDATE
                        )
                        VALUES (
                            @SUBMODULE_Code,
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
                            @CHK_CE_Voltage,
                            @CHK_G_Voltage,
                            @CHK_On_Resistance,
                            @CHK_Thermal_Resistance,
                            @CHK_C_Current,
                            @CHK_OnOff_Time,
                            @CHK_Insulation_Resistance,
                            @CHK_ESR,
                            @CHK_Capacitance,
                            @CHK_Temperature,
                            @FoldingFunction,
                            @CHK_Tbl_GetDate
                        )";

                            int affected = conn.Execute(query, chk, transaction);
                            if (affected <= 0)
                                throw new Exception("SUBMODULE_CHK 레코드 삽입 실패");

                            transaction.Commit();
                            res.Message = "SUBMODULE_CHK 등록 성공";
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            res.IsSuccess = false;
                            res.Message = "저장 오류: " + ex.Message;
                            LogHelper.WriteLog("DB(SUBMODULE_CHK)", res.Message);
                        }
                    }
                }
            }
            return res;
        }

        // SUBMODULE 보통점검 데이터 업데이트
        public Result UpdateSUBMODULEChkRepo(SUBMODULEChk submoduleChk)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                UPDATE SUBMODULE_CHK 
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
                    CHK_CE_VOLTAGE = @CHK_CE_VoltagE,
                    CHK_G_VOLTAGE = @CHK_G_VOltagE,
                    CHK_ON_RESISTANCE = @CHK_On_ResistancE,
                    CHK_THERMAL_RESISTANCE = @CHK_Thermal_Resistance,
                    CHK_C_CURRENT = @CHK_C_Current,
                    CHK_ONOFF_TIME = @CHK_OnOff_Time,
                    CHK_INSULATION_RESISTANCE = @CHK_Insulation_Resistance,
                    CHK_ESR = @CHK_ESR,
                    CHK_CAPACITANCE = @CHK_Capacitance,
                    CHK_TEMPERATURE = @CHK_Temperature,
                    CHK_UPDATE_TIME = GETDATE(),
                    FOLDINGFUNCTION = @FoldingFunction

                WHERE SUBMODULE_CODE = @SUBMODULE_Code AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, submoduleChk);
                    res.Message = affectedRows > 0 ? "SUBMODULE 보통점검 데이터 업데이트 성공" : "SUBMODULE 보통점검 데이터 업데이트 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"UpdateSUBMODULEChkInfoRepo 실패: {ex.Message}";
            }

            return res;
        }

        // SUBMODULE 보통점검 데이터 삭제
        //public Result DeleteSUBMODULEChkInfoRepo(string submoduleCode, string tblIdx)
        //{
        //    Result res = new Result(true);

        //    try
        //    {
        //        using (DBHelper dbHelper = new DBHelper())
        //        {
        //            const string query = "DELETE FROM SUBMODULE_CHK WHERE SUBMODULE_CODE = @SUBMODULE_Code AND TBL_IDX = @Tbl_Idx";

        //            int affectedRows = dbHelper.Conn.Execute(query, new { SUBMODULE_Code = submoduleCode, Tbl_Idx = tblIdx });
        //            res.Message = affectedRows > 0 ? "SUBMODULE 보통점검 데이터 삭제 성공" : "SUBMODULE 보통점검 데이터 삭제 실패";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        res.IsSuccess = false;
        //        res.Message = $"DeleteSUBMODULEChkInfoRepo 실패: {ex.Message}";
        //    }
        //    return res;
        //}
        public Result DeleteSUBMODULEChkInfoRepo(string submoduleCode, string tblIdx)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    // 1) SUBMODULE_CHK 삭제
                    const string deleteQuery = @"
                DELETE FROM SUBMODULE_CHK 
                WHERE SUBMODULE_CODE = @SUBMODULE_Code AND TBL_IDX = @Tbl_Idx";
                    int affectedRows = dbHelper.Conn.Execute(deleteQuery, new { SUBMODULE_Code = submoduleCode, Tbl_Idx = tblIdx });

                    if (affectedRows > 0)
                    {
                        res.Message = "SUBMODULE 보통점검 데이터 삭제 성공";

                        // 2) RISKMATRIX 최신 행의 HI, POF 초기화
                        const string updateRisk = @"
                    UPDATE RISKMATRIX
                    SET HI = 0,
                        POF = 0
                    WHERE CODE = @SUBMODULE_Code
                      AND LASTTIME = (
                          SELECT MAX(LASTTIME) 
                          FROM RISKMATRIX 
                          WHERE CODE = @SUBMODULE_Code
                      )";
                        int riskUpdated = dbHelper.Conn.Execute(updateRisk, new { SUBMODULE_Code = submoduleCode });

                        if (riskUpdated > 0)
                            res.Message += " + RISKMATRIX 최신 HI/PoF 초기화";
                        else
                            res.Message += " + RISKMATRIX 업데이트 없음";
                    }
                    else
                    {
                        res.Message = "SUBMODULE 보통점검 데이터 삭제 실패";
                        res.IsSuccess = false;
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"DeleteSUBMODULEChkInfoRepo 실패: {ex.Message}";
            }

            return res;
        }

    }
}
