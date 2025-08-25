using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class BYPASSVALVEChkController : Controller
    {
        public ActionResult BYPASSVALVEChkList(string BYPASSVALVE_Code)
        {
            var basicInfo = bypassvalveBasicInfoRepository.GetBYPASSVALVEBasicInfoByCode(BYPASSVALVE_Code);
            ViewBag.BYPASSVALVECode = BYPASSVALVE_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Check/etc/Cooling System/BYPASSVALVE/BYPASSVALVEChkList.cshtml");
        }

        public ActionResult BYPASSVALVEChkTotalList()
        {
            return View("~/Views/Check/Total/etc/Cooling System/BYPASSVALVEChkTotalList.cshtml");
        }

        [HttpPost]
        public ActionResult GetBYPASSVALVEChkListData(string bypassvalveCode)
        {
            try
            {
                LogHelper.WriteLog("BYPASSVALVEChkController.List", "GetBYPASSVALVEChkListData 실행");
                LogHelper.WriteLog("bypassvalveCode 조회 : ",$"{bypassvalveCode}");

                List<BYPASSVALVEChk> bypassvalveChks = new List<BYPASSVALVEChk>();
                var repoResult = bypassvalveChkRepository.GetBYPASSVALVEChkByBYPASSVALVECode(bypassvalveCode, out bypassvalveChks);
                if (repoResult.IsSuccess)
                {
                    var formattedData = bypassvalveChks.Select(item => new
                    {
                        item.Tbl_Idx,
                        item.BYPASSVALVE_Code,
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


                    LogHelper.WriteLog("BYPASSVALVEChkController.List", $"조회된 데이터: {bypassvalveChks.Count}건");
                    return Json(formattedData);
                }
                else
                {
                    LogHelper.WriteLog("BYPASSVALVEChkController.List", "BYPASSVALVE 보통점검 데이터 로드 실패");
                    return Json(new { success = false, message = "BYPASSVALVE 보통점검 데이터 로드 실패" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("BYPASSVALVEChkController.List", $"GetBYPASSVALVEListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalBYPASSVALVEChkListData()
        {
            // 1) 전체 점검 데이터 조회
            var repoResult = bypassvalveChkRepository.GetTotalBYPASSVALVEChk(out var bypassvalveChks);
            if (!repoResult.IsSuccess)
                return Json(new { success = false, message = repoResult.Message });

            // 2) BYPASSVALVE 기본정보 전체 조회 → 코드별 매핑용 딕셔너리 생성
            bypassvalveBasicInfoRepository.GetAllBYPASSVALVEBasicInfoRepo(out var basics);
            var basicMap = basics.ToDictionary(b => b.BYPASSVALVE_Code, b => b);

            // 3) JSON 응답용 객체에 Name, Serial_No 및 모든 CHK_* 필드를 한 번에 담기
            var formatted = bypassvalveChks.Select(item =>
            {
                basicMap.TryGetValue(item.BYPASSVALVE_Code, out var basic);
                return new
                {
                    item.Tbl_Idx,
                    item.BYPASSVALVE_Code,

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