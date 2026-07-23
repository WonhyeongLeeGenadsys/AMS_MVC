using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
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
                        item.CHK_Writer,
                        item.CHK_MainCircuit_InsulationStrength,
                        item.CHK_MainCircuit_PD,
                        item.CHK_Machine_Part_Operation_Time,
                        item.CHK_Mechanical_Vibration_acceleration,
                        item.CHK_Relay_Auxiliary_Contact_Resistance,
                        item.CHK_CE_Voltage,
                        item.CHK_G_Voltage,
                        item.CHK_C_Current,
                        item.CHK_OnOff_Time,
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

                var repoResult = dccbChkRepository.GetTotalDCCBChk(out var dccbChks);
                if (!repoResult.IsSuccess)
                    return Json(new { success = false, message = repoResult.Message });

                dccbBasicInfoRepository.GetAllDCCBBasicInfoRepo(out var basics);
                var basicMap = basics.ToDictionary(b => b.DCCB_Code, b => b);

                var formatted = dccbChks.Select(item =>
                {
                    basicMap.TryGetValue(item.DCCB_Code, out var basic);
                    return new
                    {
                        item.Tbl_Idx,
                        item.DCCB_Code,
                        Name = basic?.Name ?? "",
                        Serial_No = basic?.Serial_No ?? "",
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
                        item.CHK_Writer,
                        item.CHK_MainCircuit_InsulationStrength,
                        item.CHK_MainCircuit_PD,
                        item.CHK_Machine_Part_Operation_Time,
                        item.CHK_Mechanical_Vibration_acceleration,
                        item.CHK_Relay_Auxiliary_Contact_Resistance,
                        item.CHK_CE_Voltage,
                        item.CHK_G_Voltage,
                        item.CHK_C_Current,
                        item.CHK_OnOff_Time
                    };
                }).ToList();

                return Json(formatted);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("DCCBChkController.List", $"GetTotalDCCBListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}
