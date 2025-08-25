using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class LAChkController : Controller
    {
        public ActionResult LAChkList(string LA_Code)
        {
            var basicInfo = laBasicInfoRepository.GetLABasicInfoByCode(LA_Code);
            ViewBag.LACode = LA_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Check/etc/AC Yard/LA/LAChkList.cshtml");
        }

        public ActionResult LAChkTotalList()
        {
            return View("~/Views/Check/Total/etc/AC Yard/LAChkTotalList.cshtml");
        }

        [HttpPost]
        public ActionResult GetLAChkListData(string laCode)
        {
            try
            {
                LogHelper.WriteLog("LAChkController.List", "GetLAChkListData 실행");
                LogHelper.WriteLog("laCode 조회 : ",$"{laCode}");

                List<LAChk> laChks = new List<LAChk>();
                var repoResult = laChkRepository.GetLAChkByLACode(laCode, out laChks);
                if (repoResult.IsSuccess)
                {
                    var formattedData = laChks.Select(item => new
                    {
                        item.Tbl_Idx,
                        item.LA_Code,
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


                    LogHelper.WriteLog("LAChkController.List", $"조회된 데이터: {laChks.Count}건");
                    return Json(formattedData);
                }
                else
                {
                    LogHelper.WriteLog("LAChkController.List", "LA 보통점검 데이터 로드 실패");
                    return Json(new { success = false, message = "LA 보통점검 데이터 로드 실패" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("LAChkController.List", $"GetLAListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalLAChkListData()
        {
            // 1) 전체 점검 데이터 조회
            var repoResult = laChkRepository.GetTotalLAChk(out var laChks);
            if (!repoResult.IsSuccess)
                return Json(new { success = false, message = repoResult.Message });

            // 2) LA 기본정보 전체 조회 → 코드별 매핑용 딕셔너리 생성
            laBasicInfoRepository.GetAllLABasicInfoRepo(out var basics);
            var basicMap = basics.ToDictionary(b => b.LA_Code, b => b);

            // 3) JSON 응답용 객체에 Name, Serial_No 및 모든 CHK_* 필드를 한 번에 담기
            var formatted = laChks.Select(item =>
            {
                basicMap.TryGetValue(item.LA_Code, out var basic);
                return new
                {
                    item.Tbl_Idx,
                    item.LA_Code,

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