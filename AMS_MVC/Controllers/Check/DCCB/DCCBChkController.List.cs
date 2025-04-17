using AMS_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Check
{
    public partial class DCCBChkController : Controller
    {
        public ActionResult DCCBChkList(string DCCB_Code)
        {
            var basicInfo = dccbBasicInfoRepository.GetDCCBBasicInfoByCode(DCCB_Code);
            ViewBag.DCCBCode = DCCB_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Check/DCCB/DCCBChkList.cshtml");
        }

        public ActionResult DCCBChkTotalList()
        {
            return View("~/Views/Check/Total/DCCBChkTotalList.cshtml");
        }

        [HttpPost]
        public ActionResult GetDCCBChkListData(string dccbCode)
        {
            try
            {
                LogHelper.WriteLog("DCCBChkController.List", "GetDCCBChkListData 실행");
                LogHelper.WriteLog("dccbCode 조회 : ",$"{dccbCode}");

                List<DCCBChk> dccbChks = new List<DCCBChk>();
                var repoResult = dccbChkRepository.GetDCCBChkByDCCBCode(dccbCode, out dccbChks);
                if (repoResult.IsSuccess)
                {
                    var formattedData = dccbChks.Select(item => new
                    {
                        item.Tbl_Idx,
                        item.DCCB_Code,
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
                        item.CHK_DCCB_Count,
                        item.CHK_Cutoff_Count,
                        item.CHK_A_Rate,
                        item.CHK_Short_A_Rate,
                        item.CHK_Writer,
                    }).ToList();


                    LogHelper.WriteLog("DCCBChkController.List", $"조회된 데이터: {dccbChks.Count}건");
                    return Json(formattedData);
                }
                else
                {
                    LogHelper.WriteLog("DCCBChkController.List", "DCCB 보통점검 데이터 로드 실패");
                    return Json(new { success = false, message = "DCCB 보통점검 데이터 로드 실패" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("DCCBChkController.List", $"GetDCCBListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalDCCBChkListData()
        {
            try
            {
                LogHelper.WriteLog("TotalDCCBChkController.List", "GetTotalDCCBChkListData 실행");

                List<DCCBChk> dccbChks = new List<DCCBChk>();
                var repoResult = dccbChkRepository.GetTotalDCCBChk(out dccbChks);
                if (repoResult.IsSuccess)
                {
                    var formattedData = dccbChks.Select(item => new
                    {
                        item.Tbl_Idx,
                        item.DCCB_Code,
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
                        item.CHK_DCCB_Count,
                        item.CHK_Cutoff_Count,
                        item.CHK_A_Rate,
                        item.CHK_Short_A_Rate,
                        item.CHK_Writer,
                    }).ToList();

                    LogHelper.WriteLog("DCCBChkController.List", $"조회된 데이터: {dccbChks.Count}건");
                    return Json(formattedData);

                }
                else
                {
                    LogHelper.WriteLog("DCCBChkController.List", "전체 DCCB 보통점검 데이터 로드 실패");
                    return Json(new { success = false, message = "전체 DCCB 보통점검 데이터 로드 실패" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("DCCBChkController.List", $"GetTotalDCCBListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}