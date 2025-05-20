using AMS_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Check
{
    public partial class CTChkController : Controller
    {
        public ActionResult CTChkList(string CT_Code)
        {
            var basicInfo = ctBasicInfoRepository.GetCTBasicInfoByCode(CT_Code);
            ViewBag.CTCode = CT_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Check/CT/CTChkList.cshtml");
        }

        public ActionResult CTChkTotalList()
        {
            return View("~/Views/Check/Total/etc/AC Yard/CTChkTotalList.cshtml");
        }

        [HttpPost]
        public ActionResult GetCTChkListData(string ctCode)
        {
            try
            {
                LogHelper.WriteLog("CTChkController.List", "GetCTChkListData 실행");
                LogHelper.WriteLog("ctCode 조회 : ",$"{ctCode}");

                List<CTChk> ctChks = new List<CTChk>();
                var repoResult = ctChkRepository.GetCTChkByCTCode(ctCode, out ctChks);
                if (repoResult.IsSuccess)
                {
                    var formattedData = ctChks.Select(item => new
                    {
                        item.Tbl_Idx,
                        item.CT_Code,
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


                    LogHelper.WriteLog("CTChkController.List", $"조회된 데이터: {ctChks.Count}건");
                    return Json(formattedData);
                }
                else
                {
                    LogHelper.WriteLog("CTChkController.List", "CT 보통점검 데이터 로드 실패");
                    return Json(new { success = false, message = "CT 보통점검 데이터 로드 실패" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("CTChkController.List", $"GetCTListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalCTChkListData()
        {
            // 1) 전체 점검 데이터 조회
            var repoResult = ctChkRepository.GetTotalCTChk(out var ctChks);
            if (!repoResult.IsSuccess)
                return Json(new { success = false, message = repoResult.Message });

            // 2) CT 기본정보 전체 조회 → 코드별 매핑용 딕셔너리 생성
            ctBasicInfoRepository.GetAllCTBasicInfoRepo(out var basics);
            var basicMap = basics.ToDictionary(b => b.CT_Code, b => b);

            // 3) JSON 응답용 객체에 Name, Serial_No 및 모든 CHK_* 필드를 한 번에 담기
            var formatted = ctChks.Select(item =>
            {
                basicMap.TryGetValue(item.CT_Code, out var basic);
                return new
                {
                    item.Tbl_Idx,
                    item.CT_Code,

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