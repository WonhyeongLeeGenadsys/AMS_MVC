using AMS_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Check
{
    public partial class SUBMODULEChkController : Controller
    {
        public ActionResult SUBMODULEChkList(string SUBMODULE_Code)
        {
            var basicInfo = submoduleBasicInfoRepository.GetSUBMODULEBasicInfoByCode(SUBMODULE_Code);
            ViewBag.SUBMODULECode = SUBMODULE_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Check/SUBMODULE/SUBMODULEChkList.cshtml");
        }

        public ActionResult SUBMODULEChkTotalList()
        {
            return View("~/Views/Check/Total/SUBMODULEChkTotalList.cshtml");
        }

        [HttpPost]
        public ActionResult GetSUBMODULEChkListData(string submoduleCode)
        {
            try
            {
                LogHelper.WriteLog("SUBMODULEChkController.List", "GetSUBMODULEChkListData 실행");
                LogHelper.WriteLog("submoduleCode 조회 : ",$"{submoduleCode}");

                List<SUBMODULEChk> submoduleChks = new List<SUBMODULEChk>();
                var repoResult = submoduleChkRepository.GetSUBMODULEChkBySUBMODULECode(submoduleCode, out submoduleChks);
                if (repoResult.IsSuccess)
                {
                    var formattedData = submoduleChks.Select(item => new
                    {
                        item.Tbl_Idx,
                        item.SUBMODULE_Code,
                        item.CHK_Gongsa_Name,
                        item.CHK_Weather,
                        item.CHK_Temp,
                        item.CHK_Hum,
                        item.CHK_Company,
                        item.CHK_Worker,
                        item.CHK_Manager,
                        item.CHK_Urgent_No,
                        item.CHK_Type,
                        CHK_Start_Date = item.CHK_Start_Date?.ToString("yy.MM.dd"),
                        CHK_End_Date = item.CHK_End_Date?.ToString("yy.MM.dd"),
                        item.CHK_Loc,
                        item.CHK_Chuk_Loc,
                        item.CHK_Con_Status,
                        item.CHK_Bolt_Nut_Status,
                        item.CHK_Contact_Volume,
                        item.CHK_Vacuum_Degree,
                        item.CHK_Coil_A,
                        item.CHK_Contact_R,
                        item.CHK_Main_Circuit,
                        item.CHK_Control_Circuit,
                        item.CHK_Input_Time,
                        item.CHK_Open_Time,
                        item.CHK_3_Phase_Open_Gap,
                        item.CHK_Chattering_Time,
                        item.CHK_O_C_O,
                        item.CHK_Operate_Time,
                        item.CHK_OC_Test,
                        item.CHK_Indicator,
                        item.CHK_SUBMODULE_Count,
                        item.CHK_Cutoff_Count,
                        item.CHK_A_Rate,
                        item.CHK_Short_A_Rate,
                        item.CHK_Writer,
                    }).ToList();


                    LogHelper.WriteLog("SUBMODULEChkController.List", $"조회된 데이터: {submoduleChks.Count}건");
                    return Json(formattedData);
                }
                else
                {
                    LogHelper.WriteLog("SUBMODULEChkController.List", "SUBMODULE 보통점검 데이터 로드 실패");
                    return Json(new { success = false, message = "SUBMODULE 보통점검 데이터 로드 실패" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("SUBMODULEChkController.List", $"GetSUBMODULEListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalSUBMODULEChkListData()
        {
            try
            {
                LogHelper.WriteLog("TotalSUBMODULEChkController.List", "GetTotalSUBMODULEChkListData 실행");

                List<SUBMODULEChk> submoduleChks = new List<SUBMODULEChk>();
                var repoResult = submoduleChkRepository.GetTotalSUBMODULEChk(out submoduleChks);
                if (repoResult.IsSuccess)
                {
                    var formattedData = submoduleChks.Select(item => new
                    {
                        item.Tbl_Idx,
                        item.SUBMODULE_Code,
                        item.CHK_Gongsa_Name,
                        item.CHK_Weather,
                        item.CHK_Temp,
                        item.CHK_Hum,
                        item.CHK_Company,
                        item.CHK_Worker,
                        item.CHK_Manager,
                        item.CHK_Urgent_No,
                        item.CHK_Type,
                        CHK_Start_Date = item.CHK_Start_Date?.ToString("yy.MM.dd"),
                        CHK_End_Date = item.CHK_End_Date?.ToString("yy.MM.dd"),
                        item.CHK_Loc,
                        item.CHK_Chuk_Loc,
                        item.CHK_Con_Status,
                        item.CHK_Bolt_Nut_Status,
                        item.CHK_Contact_Volume,
                        item.CHK_Vacuum_Degree,
                        item.CHK_Coil_A,
                        item.CHK_Contact_R,
                        item.CHK_Main_Circuit,
                        item.CHK_Control_Circuit,
                        item.CHK_Input_Time,
                        item.CHK_Open_Time,
                        item.CHK_3_Phase_Open_Gap,
                        item.CHK_Chattering_Time,
                        item.CHK_O_C_O,
                        item.CHK_Operate_Time,
                        item.CHK_OC_Test,
                        item.CHK_Indicator,
                        item.CHK_SUBMODULE_Count,
                        item.CHK_Cutoff_Count,
                        item.CHK_A_Rate,
                        item.CHK_Short_A_Rate,
                        item.CHK_Writer,
                    }).ToList();

                    LogHelper.WriteLog("SUBMODULEChkController.List", $"조회된 데이터: {submoduleChks.Count}건");
                    return Json(formattedData);

                }
                else
                {
                    LogHelper.WriteLog("SUBMODULEChkController.List", "전체 SUBMODULE 보통점검 데이터 로드 실패");
                    return Json(new { success = false, message = "전체 SUBMODULE 보통점검 데이터 로드 실패" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("SUBMODULEChkController.List", $"GetTotalSUBMODULEListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}