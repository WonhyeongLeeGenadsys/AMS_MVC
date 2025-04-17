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
                            CHK_LOC,
                            CHK_CHUK_LOC,
                            CHK_CON_STATUS,
                            CHK_BOLT_NUT_STATUS,
                            CHK_CONTACT_VOLUME,
                            CHK_VACUUM_DEGREE,
                            CHK_COIL_A,
                            CHK_CONTACT_R,
                            CHK_MAIN_CIRCUIT,
                            CHK_CONTROL_CIRCUIT,
                            CHK_INPUT_TIME,
                            CHK_OPEN_TIME,
                            CHK_3_PHASE_OPEN_GAP,
                            CHK_CHATTERING_TIME,
                            CHK_O_C_O,
                            CHK_OPERATE_TIME,
                            CHK_OC_TEST,
                            CHK_INDICATOR,
                            CHK_DCCABLE_COUNT,
                            CHK_CUTOFF_COUNT,
                            CHK_A_RATE,
                            CHK_SHORT_A_RATE,
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
                            @CHK_Loc,
                            @CHK_Chuk_Loc,
                            @CHK_Con_Status,
                            @CHK_Bolt_Nut_Status,
                            @CHK_Contact_Volume,
                            @CHK_Vacuum_Degree,
                            @CHK_Coil_A,
                            @CHK_Contact_R,
                            @CHK_Main_Circuit,
                            @CHK_Control_Circuit,
                            @CHK_Input_Time,
                            @CHK_Open_Time,
                            @CHK_3_PHASE_OPEN_GAP,
                            @CHK_Chattering_Time,
                            @CHK_O_C_O,
                            @CHK_Operate_Time,
                            @CHK_OC_Test,
                            @CHK_Indicator,
                            @CHK_DCCABLE_Count,
                            @CHK_Cutoff_Count,
                            @CHK_A_Rate,
                            @CHK_Short_A_Rate,
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
        public Result UpdateDCCABLEChkInfoRepo(DCCABLEChk dccableChk)
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
                    CHK_DCCABLE_COUNT = @CHK_DCCABLE_Count,
                    CHK_CUTOFF_COUNT = @CHK_Cutoff_Count,
                    CHK_A_RATE = @CHK_A_Rate,
                    CHK_SHORT_A_RATE = @CHK_Short_A_Rate
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
        public Result DeleteDCCABLEChkInfoRepo(string dccableCode, string tblIdx)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "DELETE FROM DCCABLE_CHK WHERE DCCABLE_CODE = @DCCABLE_Code AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, new { DCCABLE_Code = dccableCode, Tbl_Idx = tblIdx });
                    res.Message = affectedRows > 0 ? "DCCABLE 보통점검 데이터 삭제 성공" : "DCCABLE 보통점검 데이터 삭제 실패";
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
