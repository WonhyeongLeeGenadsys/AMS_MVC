
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Common
{
    public class VCBChkRepository
    {

        // 시리얼 번호로 VCB 전체 보통점검 데이터 조회
        public Result GetVCBChkByVCBCode(string vcbCode, out List<VCBChk> vcbChkList)
        {
            Result res = new Result(true);
            vcbChkList = new List<VCBChk>();

            try
            {
                using(DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM VCB_CHK WHERE VCB_CODE = @VCB_Code";
                    vcbChkList = dbHelper.Conn
                        .Query<VCBChk>(query, new { VCB_Code = vcbCode })
                        .AsList();
                }
                res.Message = $"GetVCBChkByVCBCode 성공: VCB_CODE = {vcbCode}";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetVCBChkByVCBCode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(VCB_CHK)", res.Message);
            }
            return res;
        }

        //최근 점검 데이터 한개만 불러오기 
        public Result GetLatestVCBChkByVCBCode(string vcbCode, out List<VCBChk> vcbChkList)
        {
            Result res = new Result(true);
            vcbChkList = new List<VCBChk>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                    SELECT TOP 1 *
                    FROM VCB_CHK
                    WHERE VCB_CODE = @VCB_Code
                    ORDER BY CHK_TBL_GETDATE DESC;";

                    vcbChkList = dbHelper.Conn
                        .Query<VCBChk>(query, new { VCB_Code = vcbCode })
                        .AsList();
                }

                res.Message = $"GetVCBChkByVCBCode 성공(최신 1건): VCB_CODE = {vcbCode}";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetVCBChkByVCBCode 실패: {ex.Message}";
                LogHelper.WriteLog("DB(VCB_CHK)", res.Message);
            }

            return res;
        }


        public Result GetVCBChkDetailByVCBCode(string vcbCode, string tblIdx, out List<VCBChk> vcbChkList)
        {
            Result res = new Result(true);
            vcbChkList = new List<VCBChk>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                        SELECT *
                        FROM VCB_CHK
                        WHERE VCB_CODE = @VCB_Code
                          AND TBL_IDX = @Tbl_Idx";

                    vcbChkList = dbHelper.Conn
                        .Query<VCBChk>(query, new { VCB_Code = vcbCode, Tbl_Idx = tblIdx })
                        .AsList();

                    if (vcbChkList.Count == 0)
                    {
                        res.IsSuccess = false;
                        res.Message = "조회 결과가 없습니다.";
                    }
                    else
                    {
                        res.Message = $"GetVCBChkDetailByVCBCode 성공: VCB_CODE = {vcbCode}, TBL_IDX = {tblIdx}";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetVCBChkDetailByVCBCode 실패: {ex.Message}";
            }

            return res;
        }

        public List<dynamic> GetMonthlyAllVCBChkCounts()
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                string query = @"
                    SELECT 
                        FORMAT(CHK_Start_Date, 'yyyy-MM') AS Month, 
                        COUNT(*) AS Count,
                        'VCB' AS Type
                    FROM VCB_CHK
                    WHERE CHK_Start_Date IS NOT NULL
                    GROUP BY FORMAT(CHK_Start_Date, 'yyyy-MM')
                    ORDER BY Month;";
                return dbHelper.Conn.Query(query).ToList();
            }
        }

        // 전체 VCB 보통점검 데이터 조회
        public Result GetTotalVCBChk(out List<VCBChk> vcbChkList)
        {
            Result res = new Result(true);
            vcbChkList = new List<VCBChk>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = "SELECT * FROM VCB_CHK";
                    vcbChkList = dbHelper.Conn.Query<VCBChk>(query).AsList();
                }
                res.Message = $"GetTotalVCBChk 성공";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetTotalVCBChk 실패: {ex.Message}";
                LogHelper.WriteLog("DB(VCB_CHK)", res.Message);
            }
            return res;
        }

        public Result CreateVCBChkRepo(VCBChk chk)
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
                                INSERT INTO VCB_CHK (
                                    VCB_CODE,
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
                                    CHK_VCB_COUNT,
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
                                    FOLDINGFUNCTION
                                )
                                VALUES (
                                    @VCB_Code,
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
                                    @CHK_VCB_Count,
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
                                    @FoldingFunction
                                )";

                            int affected = conn.Execute(query, chk, transaction);
                            if (affected <= 0)
                                throw new Exception("VCB_CHK 레코드 삽입 실패");

                            transaction.Commit();
                            res.Message = "VCB_CHK 등록 성공";
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            res.IsSuccess = false;
                            res.Message = "저장 오류: " + ex.Message;
                            LogHelper.WriteLog("DB(VCB_CHK)", res.Message);
                        }
                    }
                }
            }
            return res;
        }

        // VCB 보통점검 데이터 업데이트
        public Result UpdateVCBChkInfoRepo(VCBChk vcbChk)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    const string query = @"
                UPDATE VCB_CHK 
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
                    CHK_VCB_COUNT = @CHK_VCB_Count,
                    CHK_CUTOFF_COUNT = @CHK_Cutoff_Count,
                    CHK_A_RATE = @CHK_A_Rate,
                    CHK_SHORT_A_RATE = @CHK_Short_A_Rate,
                    CHK_CONTACTWEARPERCENT = @CHK_ContactWearPercent,
                    CHK_VACUUMLEAKCURRENT = @CHK_VacuumLeakCurrent,
                    CHK_CONTACTRESISTANCE = @CHK_ContactResistance,
                    CHK_INSULATIONRESISTANCE = @CHK_InsulationResistance,
                    CHK_HOTSPOT = @CHK_HotSpot,
                    CHK_PDPATTERNVALUE = @CHK_PdPatternValue,
                    CHK_MOTORCURRENT = @CHK_MotorCurrent,
                    CHK_ACCUMSHORTCIRCUITCURRENT = @CHK_AccumShortCircuitCurrent,
                    CHK_SHORTCIRCUITCOUNT = @CHK_ShortCircuitCount,
                    CHK_OPERATIONCOUNT = @CHK_OperationCount,
                    CHK_OPENCLOSETIME = @CHK_OpenCloseTime,
                    CHK_VISUALCHECK = @CHK_VisualCheck,
                    FOLDINGFUNCTION = @FoldingFunction                
                 WHERE VCB_CODE = @VCB_Code AND TBL_IDX = @Tbl_Idx";

                    int affectedRows = dbHelper.Conn.Execute(query, vcbChk);
                    res.Message = affectedRows > 0 ? "VCB 보통점검 데이터 업데이트 성공" : "VCB 보통점검 데이터 업데이트 실패";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"UpdateVCBChkInfoRepo 실패: {ex.Message}";
            }

            return res;
        }

        // VCB 보통점검 데이터 삭제
        public Result DeleteVCBChkInfoRepo(string vcbCode, string tblIdx)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    // 1) VCB_CHK 삭제
                    const string deleteQuery = @"
                DELETE FROM VCB_CHK 
                WHERE VCB_CODE = @VCB_Code AND TBL_IDX = @Tbl_Idx";
                    int affectedRows = dbHelper.Conn.Execute(deleteQuery, new { VCB_Code = vcbCode, Tbl_Idx = tblIdx });

                    if (affectedRows > 0)
                    {
                        res.Message = "VCB 보통점검 데이터 삭제 성공";

                        // 2) RISKMATRIX 최신 행의 HI, POF 초기화
                        const string updateRisk = @"
                    UPDATE RISKMATRIX
                    SET HI = 0,
                        POF = 0
                    WHERE CODE = @VCB_Code
                      AND LASTTIME = (
                          SELECT MAX(LASTTIME) 
                          FROM RISKMATRIX 
                          WHERE CODE = @VCB_Code
                      )";
                        int riskUpdated = dbHelper.Conn.Execute(updateRisk, new { VCB_Code = vcbCode });

                        if (riskUpdated > 0)
                            res.Message += " + RISKMATRIX 최신 HI/PoF 초기화";
                        else
                            res.Message += " + RISKMATRIX 업데이트 없음";
                    }
                    else
                    {
                        res.Message = "VCB 보통점검 데이터 삭제 실패";
                        res.IsSuccess = false;
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"DeleteVCBChkInfoRepo 실패: {ex.Message}";
            }

            return res;
        }


    }
}
