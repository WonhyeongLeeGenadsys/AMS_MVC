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
    public class ARMREACTORChkRepository
    {

        // 시리얼 번호로 ARMREACTOR 보통점검 데이터 조회
        public Result GetARMREACTORChkByARMREACTORCode(string armreactorCode, out List<ARMREACTORChk> armreactorChkList)
        {
            Result res = new Result(true);
            armreactorChkList = new List<ARMREACTORChk>();

            try
            {
                using(DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM ARMREACTOR_CHK WHERE ARMREACTOR_CODE = @ARMREACTOR_Code";
                    armreactorChkList = dbHelper.Conn
                        .Query<ARMREACTORChk>(query, new { ARMREACTOR_Code = armreactorCode })
                        .AsList();
                }
                res.Message = $"GetARMREACTORChkByARMREACTORCode 성공: ARMREACTOR_CODE = {armreactorCode}";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetARMREACTORChkByARMREACTORCode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(ARMREACTOR_CHK)", res.Message);
            }
            return res;
        }

        public Result GetARMREACTORChkDetailByARMREACTORCode(string armreactorCode, string tblIdx, out List<ARMREACTORChk> armreactorChkList)
        {
            Result res = new Result(true);
            armreactorChkList = new List<ARMREACTORChk>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        SELECT *
                        FROM ARMREACTOR_CHK
                        WHERE ARMREACTOR_CODE = @ARMREACTOR_Code
                          AND TBL_IDX = @Tbl_Idx";

                    armreactorChkList = dbHelper.Conn
                        .Query<ARMREACTORChk>(query, new { ARMREACTOR_Code = armreactorCode, Tbl_Idx = tblIdx })
                        .AsList();

                    if (armreactorChkList.Count == 0)
                    {
                        res.IsSuccess = false;
                        res.Message = "조회 결과가 없습니다.";
                    }
                    else
                    {
                        res.Message = $"GetARMREACTORChkDetailByARMREACTORCode 성공: ARMREACTOR_CODE = {armreactorCode}, TBL_IDX = {tblIdx}";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetARMREACTORChkDetailByARMREACTORCode 실패: {ex.Message}";
            }

            return res;
        }

        public List<dynamic> GetMonthlyAllARMREACTORChkCounts()
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                string query = @"
                    SELECT 
                        FORMAT(CHK_Start_Date, 'yyyy-MM') AS Month, 
                        COUNT(*) AS Count,
                        'ARMREACTOR' AS Type
                    FROM ARMREACTOR_CHK
                    WHERE CHK_Start_Date IS NOT NULL
                    GROUP BY FORMAT(CHK_Start_Date, 'yyyy-MM')
                    ORDER BY Month;";
                return dbHelper.Conn.Query(query).ToList();
            }
        }

        // 전체 ARMREACTOR 보통점검 데이터 조회
        public Result GetTotalARMREACTORChk(out List<ARMREACTORChk> armreactorChkList)
        {
            Result res = new Result(true);
            armreactorChkList = new List<ARMREACTORChk>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM ARMREACTOR_CHK";
                    armreactorChkList = dbHelper.Conn.Query<ARMREACTORChk>(query).AsList();
                }
                res.Message = $"GetTotalARMREACTORChk 성공";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetTotalARMREACTORChk 실패: {ex.Message}";
                LogHelper.WriteLog("DB(ARMREACTOR_CHK)", res.Message);
            }
            return res;
        }

        // ARMREACTOR 보통점검 데이터 추가
        //public Result CreateARMREACTORChkInfoRepo(ARMREACTORChk armreactorChk)
        //{
        //    Result res = new Result(true);

        //    try
        //    {
        //        using(DBHelper dbHelper = new DBHelper())
        //        {
        //            const string query = @"
        //        INSERT INTO ARMREACTOR_CHK (
        //            ARMREACTOR_CODE, CHK_GONGSA_NAME, CHK_WEATHER, CHK_TEMP, CHK_HUM, CHK_COMPANY, 
        //            CHK_WORKER, CHK_MANAGER, CHK_URGENT_NO, CHK_TYPE, CHK_START_DATE, 
        //            CHK_END_DATE, CHK_LOC, CHK_CHUK_LOC, CHK_CON_STATUS, CHK_BOLT_NUT_STATUS, 
        //            CHK_CONTACT_VOLUME, CHK_VACUUM_DEGREE, CHK_COIL_A, CHK_CONTACT_R, 
        //            CHK_MAIN_CIRCUIT, CHK_CONTROL_CIRCUIT, CHK_INPUT_TIME, CHK_OPEN_TIME, 
        //            CHK_3_PHASE_OPEN_GAP, CHK_CHATTERING_TIME, CHK_O_C_O, CHK_OPERATE_TIME, 
        //            CHK_OC_TEST, CHK_INDICATOR, CHK_ARMREACTOR_COUNT, CHK_CUTOFF_COUNT, 
        //            CHK_A_RATE, CHK_WRITER, CHK_SHORT_A_RATE
        //        ) VALUES (
        //            @ARMREACTOR_Code, @CHK_Gongsa_Name, @CHK_Weather, @CHK_Temp, @CHK_Hum, @CHK_Company, 
        //            @CHK_Worker, @CHK_Manager, @CHK_Urgent_No, @CHK_Type, @CHK_Start_Date, 
        //            @CHK_End_Date, @CHK_Loc, @CHK_Chuk_Loc, @CHK_Con_Status, @CHK_Bolt_Nut_Status, 
        //            @CHK_Contact_Volume, @CHK_Vacuum_Degree, @CHK_Coil_A, @CHK_Contact_R, 
        //            @CHK_Main_Circuit, @CHK_Control_Circuit, @CHK_Input_Time, @CHK_Open_Time, 
        //            @CHK_3_Phase_Open_Gap, @CHK_Chattering_Time, @CHK_O_C_O, @CHK_Operate_Time, 
        //            @CHK_OC_Test, @CHK_Indicator, @CHK_ARMREACTOR_Count, @CHK_CutOff_Count, 
        //            @CHK_A_Rate, @CHK_Writer, @CHK_Short_A_Rate
        //        )";
        //            int affectedRows = dbHelper.Conn.Execute(query, armreactorChk);
        //            res.Message = affectedRows > 0 ? "ARMREACTOR 보통점검 데이터 추가 성공" : "ARMREACTOR 보통점검 데이터 추가 실패";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        res.IsSuccess = false;
        //        res.Message = $"CreateARMREACTORChkInfoRepo 실패: {ex.Message}";
        //    }

        //    return res;
        //}
        public Result CreateARMREACTORChkRepo(ARMREACTORChk chk)
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
                                INSERT INTO ARMREACTOR_CHK (
                                    ARMREACTOR_CODE,
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
                                    CHK_ARMREACTOR_COUNT,
                                    CHK_CUTOFF_COUNT,
                                    CHK_A_RATE,
                                    CHK_SHORT_A_RATE,
                                    CHK_WRITER,
                                    CHK_CONTACTWEARPERCENT,
                                    CHK_VACUUMLEAKCURRENT,
                                    CHK_CONTACTRESISTANCE,
                                    CHK_INSULATIONRESISTANCE,
                                    CHK_HOTSPOT,
                                    CHK_PDPATTERNVALUE,
                                    CHK_MOTORCURRENT,
                                    CHK_ACCUMSHORTCIRCUITCURRENT,
                                    CHK_SHORTCIRCUITCOUNT,
                                    CHK_OPERATIONCOUNT,
                                    CHK_OPENCLOSETIME,
                                    CHK_VISUALCHECK,
                                    FOLDINGFUNCTION,
                                    CHK_TBL_GETDATE
                                )
                                VALUES (
                                    @ARMREACTOR_Code,
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
                                    @CHK_3_Phase_Open_Gap,
                                    @CHK_Chattering_Time,
                                    @CHK_O_C_O,
                                    @CHK_Operate_Time,
                                    @CHK_OC_Test,
                                    @CHK_Indicator,
                                    @CHK_ARMREACTOR_Count,
                                    @CHK_Cutoff_Count,
                                    @CHK_A_Rate,
                                    @CHK_Short_A_Rate,
                                    @CHK_Writer,
                                    @CHK_ContactWearPercent,
                                    @CHK_VacuumLeakCurrent,
                                    @CHK_ContactResistance,
                                    @CHK_InsulationResistance,
                                    @CHK_HotSpot,
                                    @CHK_PdPatternValue,
                                    @CHK_MotorCurrent,
                                    @CHK_AccumShortCircuitCurrent,
                                    @CHK_ShortCircuitCount,
                                    @CHK_OperationCount,
                                    @CHK_OpenCloseTime,
                                    @CHK_VisualCheck,
                                    @FoldingFunction,
                                    @CHK_Tbl_GetDate
                                )";

                            int affected = conn.Execute(query, chk, transaction);
                            if (affected <= 0)
                                throw new Exception("ARMREACTOR_CHK 레코드 삽입 실패");

                            transaction.Commit();
                            res.Message = "ARMREACTOR_CHK 등록 성공";
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            res.IsSuccess = false;
                            res.Message = "저장 오류: " + ex.Message;
                            LogHelper.WriteLog("DB(ARMREACTOR_CHK)", res.Message);
                        }
                    }
                }
            }
            return res;
        }

        // ARMREACTOR 보통점검 데이터 업데이트
        public Result UpdateARMREACTORChkInfoRepo(ARMREACTORChk armreactorChk)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                UPDATE ARMREACTOR_CHK 
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
                    CHK_ARMREACTOR_COUNT = @CHK_ARMREACTOR_Count,
                    CHK_CUTOFF_COUNT = @CHK_Cutoff_Count,
                    CHK_A_RATE = @CHK_A_Rate,
                    CHK_SHORT_A_RATE = @CHK_Short_A_Rate
                WHERE ARMREACTOR_CODE = @ARMREACTOR_Code AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, armreactorChk);
                    res.Message = affectedRows > 0 ? "ARMREACTOR 보통점검 데이터 업데이트 성공" : "ARMREACTOR 보통점검 데이터 업데이트 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"UpdateARMREACTORChkInfoRepo 실패: {ex.Message}";
            }

            return res;
        }

        // ARMREACTOR 보통점검 데이터 삭제
        public Result DeleteARMREACTORChkInfoRepo(string armreactorCode, string tblIdx)
        {
            Result res = new Result(true);

            try
            {
                using(DBHelper dbHelper = new DBHelper())
                {
                    const string query = "DELETE FROM ARMREACTOR_CHK WHERE ARMREACTOR_CODE = @ARMREACTOR_Code AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, new { ARMREACTOR_Code = armreactorCode, Tbl_Idx = tblIdx });
                    res.Message = affectedRows > 0 ? "ARMREACTOR 보통점검 데이터 삭제 성공" : "ARMREACTOR 보통점검 데이터 삭제 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"DeleteARMREACTORChkInfoRepo 실패: {ex.Message}";
            }
            return res;
        }
    }
}
