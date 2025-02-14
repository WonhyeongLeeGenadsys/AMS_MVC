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
    public class VCBChkRepository
    {
        private readonly DBHelper mDb;

        public VCBChkRepository()
        {
            mDb = new DBHelper();
        }

        // 시리얼 번호로 VCB 보통점검 데이터 조회
        public Result GetVCBChkByVCBCode(string vcbCode, out List<VCBChk> vcbChkList)
        {
            Result res = new Result(true);
            vcbChkList = new List<VCBChk>();

            try
            {
                const string query = "SELECT * FROM VCB_CHK WHERE VCB_CODE = @VCB_Code";
                vcbChkList = mDb.Conn.Query<VCBChk>(query, new { VCB_Code = vcbCode }).AsList();
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

        // 특정 공사명으로 VCB 보통점검 데이터 조회
        public Result GetVCBChkDetailByVCBCode(string vcbCode, string tblIdx, out List<VCBChk> vcbChkList)
        {
            Result res = new Result(true);
            vcbChkList = new List<VCBChk>();

            try
            {
                const string query = @"
                SELECT * 
                FROM VCB_CHK 
                WHERE VCB_CODE = @VCB_Code AND TBL_IDX = @Tbl_Idx";

                vcbChkList = mDb.Conn.Query<VCBChk>(query, new { VCB_Code = vcbCode, Tbl_Idx = tblIdx }).AsList();
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
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"GetVCBChkDetailByVCBCode 실패: {ex.Message}";
            }

            return res;
        }

        // VCB 보통점검 데이터 추가
        public Result CreateVCBChkInfoRepo(VCBChk vcbChk)
        {
            Result res = new Result(true);

            try
            {
                const string query = @"
                INSERT INTO VCB_CHK (
                    VCB_CODE, CHK_GONGSA_NAME, CHK_WEATHER, CHK_TEMP, CHK_HUM, CHK_COMPANY, 
                    CHK_WORKER, CHK_MANAGER, CHK_URGENT_NO, CHK_TYPE, CHK_START_DATE, 
                    CHK_END_DATE, CHK_LOC, CHK_CHUK_LOC, CHK_CON_STATUS, CHK_BOLT_NUT_STATUS, 
                    CHK_CONTACT_VOLUME, CHK_VACUUM_DEGREE, CHK_COIL_A, CHK_CONTACT_R, 
                    CHK_MAIN_CIRCUIT, CHK_CONTROL_CIRCUIT, CHK_INPUT_TIME, CHK_OPEN_TIME, 
                    CHK_3_PHASE_OPEN_GAP, CHK_CHATTERING_TIME, CHK_O_C_O, CHK_OPERATE_TIME, 
                    CHK_OC_TEST, CHK_INDICATOR, CHK_VCB_COUNT, CHK_CUTOFF_COUNT, 
                    CHK_A_RATE, CHK_SHORT_A_RATE
                ) VALUES (
                    @VCB_Code, @CHK_Gongsa_Name, @CHK_Weather, @CHK_Temp, @CHK_Hum, @CHK_Company, 
                    @CHK_Worker, @CHK_Manager, @CHK_Urgent_No, @CHK_Type, @CHK_Start_Date, 
                    @CHK_End_Date, @CHK_Loc, @CHK_Chuk_Loc, @CHK_Con_Status, @CHK_Bolt_Nut_Status, 
                    @CHK_Contact_Volume, @CHK_Vacuum_Degree, @CHK_Coil_A, @CHK_Contact_R, 
                    @CHK_Main_Circuit, @CHK_Control_Circuit, @CHK_Input_Time, @CHK_Open_Time, 
                    @CHK_3_Phase_Open_Gap, @CHK_Chattering_Time, @CHK_O_C_O, @CHK_Operate_Time, 
                    @CHK_OC_Test, @CHK_Indicator, @CHK_VCB_Count, @CHK_CutOff_Count, 
                    @CHK_A_Rate, @CHK_Short_A_Rate
                )";

                int affectedRows = mDb.Conn.Execute(query, vcbChk);
                res.Message = affectedRows > 0 ? "VCB 보통점검 데이터 추가 성공" : "VCB 보통점검 데이터 추가 실패";
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = $"CreateVCBChkInfoRepo 실패: {ex.Message}";
            }

            return res;
        }

        // VCB 보통점검 데이터 업데이트
        public Result UpdateVCBChkInfoRepo(VCBChk vcbChk)
        {
            Result res = new Result(true);

            try
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
                    CHK_SHORT_A_RATE = @CHK_Short_A_Rate
                WHERE VCB_CODE = @VCB_Code AND TBL_IDX = @Tbl_Idx";

                int affectedRows = mDb.Conn.Execute(query, vcbChk);
                res.Message = affectedRows > 0 ? "VCB 보통점검 데이터 업데이트 성공" : "VCB 보통점검 데이터 업데이트 실패";
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
                const string query = "DELETE FROM VCB_CHK WHERE VCB_CODE = @VCB_Code AND TBL_IDX = @Tbl_Idx";

                int affectedRows = mDb.Conn.Execute(query, new { VCB_Code = vcbCode, Tbl_Idx = tblIdx });
                res.Message = affectedRows > 0 ? "VCB 보통점검 데이터 삭제 성공" : "VCB 보통점검 데이터 삭제 실패";
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
