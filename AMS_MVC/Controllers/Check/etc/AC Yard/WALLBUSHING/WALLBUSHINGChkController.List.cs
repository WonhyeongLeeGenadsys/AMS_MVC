using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class WALLBUSHINGChkController : Controller
    {
        public ActionResult WALLBUSHINGChkList(string WALLBUSHING_Code)
        {
            var basicInfo = wallbushingBasicInfoRepository.GetWALLBUSHINGBasicInfoByCode(WALLBUSHING_Code);
            ViewBag.WALLBUSHINGCode = WALLBUSHING_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Check/etc/AC Yard/WALLBUSHING/WALLBUSHINGChkList.cshtml");
        }

        public ActionResult WALLBUSHINGChkTotalList()
        {
            return View("~/Views/Check/Total/etc/AC Yard/WALLBUSHINGChkTotalList.cshtml");
        }

        [HttpPost]
        public ActionResult GetWALLBUSHINGChkListData(string wallbushingCode)
        {
            try
            {
                LogHelper.WriteLog("WALLBUSHINGChkController.List", "GetWALLBUSHINGChkListData 실행");
                LogHelper.WriteLog("wallbushingCode 조회 : ",$"{wallbushingCode}");

                List<WALLBUSHINGChk> wallbushingChks = new List<WALLBUSHINGChk>();
                var repoResult = wallbushingChkRepository.GetWALLBUSHINGChkByWALLBUSHINGCode(wallbushingCode, out wallbushingChks);
                if (repoResult.IsSuccess)
                {
                    var formattedData = wallbushingChks.Select(item => new
                    {
                        item.Tbl_Idx,
                        item.WALLBUSHING_Code,
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


                    LogHelper.WriteLog("WALLBUSHINGChkController.List", $"조회된 데이터: {wallbushingChks.Count}건");
                    return Json(formattedData);
                }
                else
                {
                    LogHelper.WriteLog("WALLBUSHINGChkController.List", "WALLBUSHING 보통점검 데이터 로드 실패");
                    return Json(new { success = false, message = "WALLBUSHING 보통점검 데이터 로드 실패" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("WALLBUSHINGChkController.List", $"GetWALLBUSHINGListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalWALLBUSHINGChkListData()
        {
            // 1) 전체 점검 데이터 조회
            var repoResult = wallbushingChkRepository.GetTotalWALLBUSHINGChk(out var wallbushingChks);
            if (!repoResult.IsSuccess)
                return Json(new { success = false, message = repoResult.Message });

            // 2) WALLBUSHING 기본정보 전체 조회 → 코드별 매핑용 딕셔너리 생성
            wallbushingBasicInfoRepository.GetAllWALLBUSHINGBasicInfoRepo(out var basics);
            var basicMap = basics.ToDictionary(b => b.WALLBUSHING_Code, b => b);

            // 3) JSON 응답용 객체에 Name, Serial_No 및 모든 CHK_* 필드를 한 번에 담기
            var formatted = wallbushingChks.Select(item =>
            {
                basicMap.TryGetValue(item.WALLBUSHING_Code, out var basic);
                return new
                {
                    item.Tbl_Idx,
                    item.WALLBUSHING_Code,

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