using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class ESChkController : Controller
    {
        public ActionResult ESChkList(string ES_Code)
        {
            var basicInfo = esBasicInfoRepository.GetESBasicInfoByCode(ES_Code);
            ViewBag.ESCode = ES_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Check/etc/Grounding/ES/ESChkList.cshtml");
        }

        public ActionResult ESChkTotalList()
        {
            return View("~/Views/Check/Total/etc/Grounding/ESChkTotalList.cshtml");
        }

        [HttpPost]
        public ActionResult GetESChkListData(string esCode)
        {
            try
            {
                LogHelper.WriteLog("ESChkController.List", "GetESChkListData 실행");
                LogHelper.WriteLog("esCode 조회 : ",$"{esCode}");

                List<ESChk> esChks = new List<ESChk>();
                var repoResult = esChkRepository.GetESChkByESCode(esCode, out esChks);
                if (repoResult.IsSuccess)
                {
                    var formattedData = esChks.Select(item => new
                    {
                        item.Tbl_Idx,
                        item.ES_Code,
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


                    LogHelper.WriteLog("ESChkController.List", $"조회된 데이터: {esChks.Count}건");
                    return Json(formattedData);
                }
                else
                {
                    LogHelper.WriteLog("ESChkController.List", "ES 보통점검 데이터 로드 실패");
                    return Json(new { success = false, message = "ES 보통점검 데이터 로드 실패" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("ESChkController.List", $"GetESListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalESChkListData()
        {
            // 1) 전체 점검 데이터 조회
            var repoResult = esChkRepository.GetTotalESChk(out var esChks);
            if (!repoResult.IsSuccess)
                return Json(new { success = false, message = repoResult.Message });

            // 2) ES 기본정보 전체 조회 → 코드별 매핑용 딕셔너리 생성
            esBasicInfoRepository.GetAllESBasicInfoRepo(out var basics);
            var basicMap = basics.ToDictionary(b => b.ES_Code, b => b);

            // 3) JSON 응답용 객체에 Name, Serial_No 및 모든 CHK_* 필드를 한 번에 담기
            var formatted = esChks.Select(item =>
            {
                basicMap.TryGetValue(item.ES_Code, out var basic);
                return new
                {
                    item.Tbl_Idx,
                    item.ES_Code,

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