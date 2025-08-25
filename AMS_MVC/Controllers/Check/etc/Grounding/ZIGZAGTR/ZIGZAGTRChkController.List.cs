using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class ZIGZAGTRChkController : Controller
    {
        public ActionResult ZIGZAGTRChkList(string ZIGZAGTR_Code)
        {
            var basicInfo = zigzagtrBasicInfoRepository.GetZIGZAGTRBasicInfoByCode(ZIGZAGTR_Code);
            ViewBag.ZIGZAGTRCode = ZIGZAGTR_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Check/etc/Grounding/ZIGZAGTR/ZIGZAGTRChkList.cshtml");
        }

        public ActionResult ZIGZAGTRChkTotalList()
        {
            return View("~/Views/Check/Total/etc/Grounding/ZIGZAGTRChkTotalList.cshtml");
        }

        [HttpPost]
        public ActionResult GetZIGZAGTRChkListData(string zigzagtrCode)
        {
            try
            {
                LogHelper.WriteLog("ZIGZAGTRChkController.List", "GetZIGZAGTRChkListData 실행");
                LogHelper.WriteLog("zigzagtrCode 조회 : ",$"{zigzagtrCode}");

                List<ZIGZAGTRChk> zigzagtrChks = new List<ZIGZAGTRChk>();
                var repoResult = zigzagtrChkRepository.GetZIGZAGTRChkByZIGZAGTRCode(zigzagtrCode, out zigzagtrChks);
                if (repoResult.IsSuccess)
                {
                    var formattedData = zigzagtrChks.Select(item => new
                    {
                        item.Tbl_Idx,
                        item.ZIGZAGTR_Code,
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


                    LogHelper.WriteLog("ZIGZAGTRChkController.List", $"조회된 데이터: {zigzagtrChks.Count}건");
                    return Json(formattedData);
                }
                else
                {
                    LogHelper.WriteLog("ZIGZAGTRChkController.List", "ZIGZAGTR 보통점검 데이터 로드 실패");
                    return Json(new { success = false, message = "ZIGZAGTR 보통점검 데이터 로드 실패" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("ZIGZAGTRChkController.List", $"GetZIGZAGTRListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalZIGZAGTRChkListData()
        {
            // 1) 전체 점검 데이터 조회
            var repoResult = zigzagtrChkRepository.GetTotalZIGZAGTRChk(out var zigzagtrChks);
            if (!repoResult.IsSuccess)
                return Json(new { success = false, message = repoResult.Message });

            // 2) ZIGZAGTR 기본정보 전체 조회 → 코드별 매핑용 딕셔너리 생성
            zigzagtrBasicInfoRepository.GetAllZIGZAGTRBasicInfoRepo(out var basics);
            var basicMap = basics.ToDictionary(b => b.ZIGZAGTR_Code, b => b);

            // 3) JSON 응답용 객체에 Name, Serial_No 및 모든 CHK_* 필드를 한 번에 담기
            var formatted = zigzagtrChks.Select(item =>
            {
                basicMap.TryGetValue(item.ZIGZAGTR_Code, out var basic);
                return new
                {
                    item.Tbl_Idx,
                    item.ZIGZAGTR_Code,

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