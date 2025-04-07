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
        public Result CreateDCCBChkInfoRepo(DCCBChk dccbChk)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                INSERT INTO DCCB_CHK (
                    DCCB_CODE, CHK_GONGSA_NAME, CHK_WEATHER, CHK_TEMP, CHK_HUM, CHK_COMPANY, 
                    CHK_WORKER, CHK_MANAGER, CHK_URGENT_NO, CHK_TYPE, CHK_START_DATE, 
                    CHK_END_DATE, CHK_LOC, CHK_CHUK_LOC, CHK_CON_STATUS, CHK_BOLT_NUT_STATUS, 
                    CHK_CONTACT_VOLUME, CHK_VACUUM_DEGREE, CHK_COIL_A, CHK_CONTACT_R, 
                    CHK_MAIN_CIRCUIT, CHK_CONTROL_CIRCUIT, CHK_INPUT_TIME, CHK_OPEN_TIME, 
                    CHK_3_PHASE_OPEN_GAP, CHK_CHATTERING_TIME, CHK_O_C_O, CHK_OPERATE_TIME, 
                    CHK_OC_TEST, CHK_INDICATOR, CHK_DCCB_COUNT, CHK_CUTOFF_COUNT, 
                    CHK_A_RATE, CHK_WRITER, CHK_SHORT_A_RATE
                ) VALUES (
                    @DCCB_Code, @CHK_Gongsa_Name, @CHK_Weather, @CHK_Temp, @CHK_Hum, @CHK_Company, 
                    @CHK_Worker, @CHK_Manager, @CHK_Urgent_No, @CHK_Type, @CHK_Start_Date, 
                    @CHK_End_Date, @CHK_Loc, @CHK_Chuk_Loc, @CHK_Con_Status, @CHK_Bolt_Nut_Status, 
                    @CHK_Contact_Volume, @CHK_Vacuum_Degree, @CHK_Coil_A, @CHK_Contact_R, 
                    @CHK_Main_Circuit, @CHK_Control_Circuit, @CHK_Input_Time, @CHK_Open_Time, 
                    @CHK_3_Phase_Open_Gap, @CHK_Chattering_Time, @CHK_O_C_O, @CHK_Operate_Time, 
                    @CHK_OC_Test, @CHK_Indicator, @CHK_DCCB_Count, @CHK_CutOff_Count, 
                    @CHK_A_Rate, @CHK_Writer, @CHK_Short_A_Rate
                )";
                    int affectedRows = dbHelper.Conn.Execute(query, dccbChk);
                    res.Message = affectedRows > 0 ? "DCCB 보통점검 데이터 추가 성공" : "DCCB 보통점검 데이터 추가 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"CreateDCCBChkInfoRepo 실패: {ex.Message}";
            }

            return res;
        }

        // DCCB 보통점검 데이터 업데이트
        public Result UpdateDCCBChkInfoRepo(DCCBChk dccbChk)
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
                    CHK_DCCB_COUNT = @CHK_DCCB_Count,
                    CHK_CUTOFF_COUNT = @CHK_Cutoff_Count,
                    CHK_A_RATE = @CHK_A_Rate,
                    CHK_SHORT_A_RATE = @CHK_Short_A_Rate
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
        public Result DeleteDCCBChkInfoRepo(string dccbCode, string tblIdx)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "DELETE FROM DCCB_CHK WHERE DCCB_CODE = @DCCB_Code AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, new { DCCB_Code = dccbCode, Tbl_Idx = tblIdx });
                    res.Message = affectedRows > 0 ? "DCCB 보통점검 데이터 삭제 성공" : "DCCB 보통점검 데이터 삭제 실패";
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
