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
        public Result CreateSUBMODULEChkInfoRepo(SUBMODULEChk submoduleChk)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                INSERT INTO SUBMODULE_CHK (
                    SUBMODULE_CODE, CHK_GONGSA_NAME, CHK_WEATHER, CHK_TEMP, CHK_HUM, CHK_COMPANY, 
                    CHK_WORKER, CHK_MANAGER, CHK_URGENT_NO, CHK_TYPE, CHK_START_DATE, 
                    CHK_END_DATE, CHK_LOC, CHK_CHUK_LOC, CHK_CON_STATUS, CHK_BOLT_NUT_STATUS, 
                    CHK_CONTACT_VOLUME, CHK_VACUUM_DEGREE, CHK_COIL_A, CHK_CONTACT_R, 
                    CHK_MAIN_CIRCUIT, CHK_CONTROL_CIRCUIT, CHK_INPUT_TIME, CHK_OPEN_TIME, 
                    CHK_3_PHASE_OPEN_GAP, CHK_CHATTERING_TIME, CHK_O_C_O, CHK_OPERATE_TIME, 
                    CHK_OC_TEST, CHK_INDICATOR, CHK_SUBMODULE_COUNT, CHK_CUTOFF_COUNT, 
                    CHK_A_RATE, CHK_WRITER, CHK_SHORT_A_RATE
                ) VALUES (
                    @SUBMODULE_Code, @CHK_Gongsa_Name, @CHK_Weather, @CHK_Temp, @CHK_Hum, @CHK_Company, 
                    @CHK_Worker, @CHK_Manager, @CHK_Urgent_No, @CHK_Type, @CHK_Start_Date, 
                    @CHK_End_Date, @CHK_Loc, @CHK_Chuk_Loc, @CHK_Con_Status, @CHK_Bolt_Nut_Status, 
                    @CHK_Contact_Volume, @CHK_Vacuum_Degree, @CHK_Coil_A, @CHK_Contact_R, 
                    @CHK_Main_Circuit, @CHK_Control_Circuit, @CHK_Input_Time, @CHK_Open_Time, 
                    @CHK_3_Phase_Open_Gap, @CHK_Chattering_Time, @CHK_O_C_O, @CHK_Operate_Time, 
                    @CHK_OC_Test, @CHK_Indicator, @CHK_SUBMODULE_Count, @CHK_CutOff_Count, 
                    @CHK_A_Rate, @CHK_Writer, @CHK_Short_A_Rate
                )";
                    int affectedRows = dbHelper.Conn.Execute(query, submoduleChk);
                    res.Message = affectedRows > 0 ? "SUBMODULE 보통점검 데이터 추가 성공" : "SUBMODULE 보통점검 데이터 추가 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"CreateSUBMODULEChkInfoRepo 실패: {ex.Message}";
            }

            return res;
        }

        // SUBMODULE 보통점검 데이터 업데이트
        public Result UpdateSUBMODULEChkInfoRepo(SUBMODULEChk submoduleChk)
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
                    CHK_LOC = @CHK_Loc,
                    CHK_CHUK_LOC = @CHK_Chuk_Loc,
                    CHK_CON_STATUS = @CHK_Con_Status,
                    CHK_BOLT_NUT_STATUS = @CHK_Bolt_Nut_Status,
                    CHK_CONTACT_VOLUME = @CHK_Contact_Volume,
                    CHK_VACUUM_DEGREE = @CHK_Vacuum_Degree,
                    CHK_COIL_A = @CHK_Coil_A,
                    CHK_CONTACT_R = @CHK_Contact_R,
                    CHK_MAIN_CIRCUIT = @CHK_Main_Circuit,
                    CHK_CONTROL_CIRCUIT = @CHK_Control_Circuit,
                    CHK_INPUT_TIME = @CHK_Input_Time,
                    CHK_OPEN_TIME = @CHK_Open_Time,
                    CHK_3_PHASE_OPEN_GAP = @CHK_3_Phase_Open_Gap,
                    CHK_CHATTERING_TIME = @CHK_Chattering_Time,
                    CHK_O_C_O = @CHK_O_C_O,
                    CHK_OPERATE_TIME = @CHK_Operate_Time,
                    CHK_OC_TEST = @CHK_OC_Test,
                    CHK_INDICATOR = @CHK_Indicator,
                    CHK_SUBMODULE_COUNT = @CHK_SUBMODULE_Count,
                    CHK_CUTOFF_COUNT = @CHK_Cutoff_Count,
                    CHK_A_RATE = @CHK_A_Rate,
                    CHK_SHORT_A_RATE = @CHK_Short_A_Rate
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
        public Result DeleteSUBMODULEChkInfoRepo(string submoduleCode, string tblIdx)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "DELETE FROM SUBMODULE_CHK WHERE SUBMODULE_CODE = @SUBMODULE_Code AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, new { SUBMODULE_Code = submoduleCode, Tbl_Idx = tblIdx });
                    res.Message = affectedRows > 0 ? "SUBMODULE 보통점검 데이터 삭제 성공" : "SUBMODULE 보통점검 데이터 삭제 실패";
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
