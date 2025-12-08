using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class HEATEXCHANGERChkController : Controller
    {
        public ActionResult HEATEXCHANGERChkList(string HEATEXCHANGER_Code)
        {
            var basicInfo = heatexchangerBasicInfoRepository.GetHEATEXCHANGERBasicInfoByCode(HEATEXCHANGER_Code);
            ViewBag.HEATEXCHANGERCode = HEATEXCHANGER_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Check/etc/Cooling System/HEATEXCHANGER/HEATEXCHANGERChkList.cshtml");
        }

        public ActionResult HEATEXCHANGERChkTotalList()
        {
            return View("~/Views/Check/Total/etc/AC Yard/HEATEXCHANGERChkTotalList.cshtml");
        }

        [HttpPost]
        public ActionResult GetHEATEXCHANGERChkListData(string heatexchangerCode)
        {
            try
            {
                LogHelper.WriteLog("HEATEXCHANGERChkController.List", "GetHEATEXCHANGERChkListData 실행");
                LogHelper.WriteLog("heatexchangerCode 조회 : ",$"{heatexchangerCode}");

                List<HEATEXCHANGERChk> heatexchangerChks = new List<HEATEXCHANGERChk>();
                var repoResult = heatexchangerChkRepository.GetHEATEXCHANGERChkByHEATEXCHANGERCode(heatexchangerCode, out heatexchangerChks);
                if (repoResult.IsSuccess)
                {
                    var formattedData = heatexchangerChks.Select(item => new
                    {
                        item.Tbl_Idx,
                        item.HEATEXCHANGER_Code,
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
                    }).ToList();


                    LogHelper.WriteLog("HEATEXCHANGERChkController.List", $"조회된 데이터: {heatexchangerChks.Count}건");
                    return Json(formattedData);
                }
                else
                {
                    LogHelper.WriteLog("HEATEXCHANGERChkController.List", "HEATEXCHANGER 보통점검 데이터 로드 실패");
                    return Json(new { success = false, message = "HEATEXCHANGER 보통점검 데이터 로드 실패" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("HEATEXCHANGERChkController.List", $"GetHEATEXCHANGERListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalHEATEXCHANGERChkListData()
        {
            // 1) 전체 점검 데이터 조회
            var repoResult = heatexchangerChkRepository.GetTotalHEATEXCHANGERChk(out var heatexchangerChks);
            if (!repoResult.IsSuccess)
                return Json(new { success = false, message = repoResult.Message });

            // 2) HEATEXCHANGER 기본정보 전체 조회 → 코드별 매핑용 딕셔너리 생성
            heatexchangerBasicInfoRepository.GetAllHEATEXCHANGERBasicInfoRepo(out var basics);
            var basicMap = basics.ToDictionary(b => b.HEATEXCHANGER_Code, b => b);

            // 3) JSON 응답용 객체에 Name, Serial_No 및 모든 CHK_* 필드를 한 번에 담기
            var formatted = heatexchangerChks.Select(item =>
            {
                basicMap.TryGetValue(item.HEATEXCHANGER_Code, out var basic);
                return new
                {
                    item.Tbl_Idx,
                    item.HEATEXCHANGER_Code,

                    // 추가한 기본정보
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

                    // DB 반영 일시
                    CHK_Tbl_GetDate = item.CHK_Tbl_GetDate.ToString("yyyy-MM-dd HH:mm:ss")
                };
            }).ToList();

            return Json(formatted);
        }

    }
}