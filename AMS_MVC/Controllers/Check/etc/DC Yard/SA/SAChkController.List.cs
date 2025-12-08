using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class SAChkController : Controller
    {
        public ActionResult SAChkList(string SA_Code)
        {
            var basicInfo = saBasicInfoRepository.GetSABasicInfoByCode(SA_Code);
            ViewBag.SACode = SA_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Check/etc/DC Yard/SA/SAChkList.cshtml");
        }

        public ActionResult SAChkTotalList()
        {
            return View("~/Views/Check/Total/etc/DC Yard/SAChkTotalList.cshtml");
        }

        [HttpPost]
        public ActionResult GetSAChkListData(string saCode)
        {
            try
            {
                LogHelper.WriteLog("SAChkController.List", "GetSAChkListData 실행");
                LogHelper.WriteLog("saCode 조회 : ",$"{saCode}");

                List<SAChk> saChks = new List<SAChk>();
                var repoResult = saChkRepository.GetSAChkBySACode(saCode, out saChks);
                if (repoResult.IsSuccess)
                {
                    var formattedData = saChks.Select(item => new
                    {
                        item.Tbl_Idx,
                        item.SA_Code,
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


                    LogHelper.WriteLog("SAChkController.List", $"조회된 데이터: {saChks.Count}건");
                    return Json(formattedData);
                }
                else
                {
                    LogHelper.WriteLog("SAChkController.List", "SA 보통점검 데이터 로드 실패");
                    return Json(new { success = false, message = "SA 보통점검 데이터 로드 실패" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("SAChkController.List", $"GetSAListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalSAChkListData()
        {
            // 1) 전체 점검 데이터 조회
            var repoResult = saChkRepository.GetTotalSAChk(out var saChks);
            if (!repoResult.IsSuccess)
                return Json(new { success = false, message = repoResult.Message });

            // 2) SA 기본정보 전체 조회 → 코드별 매핑용 딕셔너리 생성
            saBasicInfoRepository.GetAllSABasicInfoRepo(out var basics);
            var basicMap = basics.ToDictionary(b => b.SA_Code, b => b);

            // 3) JSON 응답용 객체에 Name, Serial_No 및 모든 CHK_* 필드를 한 번에 담기
            var formatted = saChks.Select(item =>
            {
                basicMap.TryGetValue(item.SA_Code, out var basic);
                return new
                {
                    item.Tbl_Idx,
                    item.SA_Code,

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

                    // DB 반영 일시
                    CHK_Tbl_GetDate = item.CHK_Tbl_GetDate.ToString("yyyy-MM-dd HH:mm:ss")
                };
            }).ToList();

            return Json(formatted);
        }

    }
}