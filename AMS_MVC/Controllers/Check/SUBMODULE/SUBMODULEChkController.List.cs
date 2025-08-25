using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
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
                        item.CHK_Writer,
                        item.CHK_CE_Voltage,
                        item.CHK_G_Voltage,
                        item.CHK_On_Resistance,
                        item.CHK_Thermal_Resistance,
                        item.CHK_C_Current,
                        item.CHK_OnOff_Time,
                        item.CHK_Insulation_Resistance,
                        item.CHK_ESR,
                        item.CHK_Capacitance,
                        item.CHK_Temperature,
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
                // 1) 전체 점검 데이터 조회
                var repoResult = submoduleChkRepository.GetTotalSUBMODULEChk(out var submoduleChks);
                if (!repoResult.IsSuccess)
                    return Json(new { success = false, message = repoResult.Message });

                // 2) SUBMODULE 기본정보 전체 조회 → 코드별 딕셔너리 생성
                submoduleBasicInfoRepository.GetAllSUBMODULEBasicInfoRepo(out var basics);
                var basicMap = basics.ToDictionary(b => b.SUBMODULE_Code, b => b);

                // 3) JSON 응답용 객체에 Name, Serial_No 및 기존 필드 모두 포함
                var formatted = submoduleChks.Select(item =>
                {
                    basicMap.TryGetValue(item.SUBMODULE_Code, out var basic);
                    return new
                    {
                        item.Tbl_Idx,
                        item.SUBMODULE_Code,

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

                        item.CHK_CE_Voltage,
                        item.CHK_G_Voltage,
                        item.CHK_On_Resistance,
                        item.CHK_Thermal_Resistance,
                        item.CHK_C_Current,
                        item.CHK_OnOff_Time,
                        item.CHK_Insulation_Resistance,
                        item.CHK_ESR,
                        item.CHK_Capacitance,
                        item.CHK_Temperature,

                        CHK_Tbl_GetDate = item.CHK_Tbl_GetDate.ToString("yyyy-MM-dd HH:mm:ss")
                    };
                }).ToList();

                return Json(formatted);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("SUBMODULEChkController.List", $"GetTotalSUBMODULEListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}