using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class TANKChkController : Controller
    {
        public ActionResult TANKChkList(string TANK_Code)
        {
            var basicInfo = vcbBasicInfoRepository.GetTANKBasicInfoByCode(TANK_Code);
            ViewBag.TANKCode = TANK_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Check/etc/DC Yard/TANK/TANKChkList.cshtml");
        }

        public ActionResult TANKChkTotalList()
        {
            return View("~/Views/Check/Total/etc/Cooling System/TANKChkTotalList.cshtml");
        }

        [HttpPost]
        public ActionResult GetTANKChkListData(string vcbCode)
        {
            try
            {
                LogHelper.WriteLog("TANKChkController.List", "GetTANKChkListData 실행");
                LogHelper.WriteLog("vcbCode 조회 : ",$"{vcbCode}");

                List<TANKChk> vcbChks = new List<TANKChk>();
                var repoResult = vcbChkRepository.GetTANKChkByTANKCode(vcbCode, out vcbChks);
                if (repoResult.IsSuccess)
                {
                    var formattedData = vcbChks.Select(item => new
                    {
                        item.Tbl_Idx,
                        item.TANK_Code,
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


                    LogHelper.WriteLog("TANKChkController.List", $"조회된 데이터: {vcbChks.Count}건");
                    return Json(formattedData);
                }
                else
                {
                    LogHelper.WriteLog("TANKChkController.List", "TANK 보통점검 데이터 로드 실패");
                    return Json(new { success = false, message = "TANK 보통점검 데이터 로드 실패" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("TANKChkController.List", $"GetTANKListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalTANKChkListData()
        {
            // 1) 전체 점검 데이터 조회
            var repoResult = vcbChkRepository.GetTotalTANKChk(out var vcbChks);
            if (!repoResult.IsSuccess)
                return Json(new { success = false, message = repoResult.Message });

            // 2) TANK 기본정보 전체 조회 → 코드별 매핑용 딕셔너리 생성
            vcbBasicInfoRepository.GetAllTANKBasicInfoRepo(out var basics);
            var basicMap = basics.ToDictionary(b => b.TANK_Code, b => b);

            // 3) JSON 응답용 객체에 Name, Serial_No 및 모든 CHK_* 필드를 한 번에 담기
            var formatted = vcbChks.Select(item =>
            {
                basicMap.TryGetValue(item.TANK_Code, out var basic);
                return new
                {
                    item.Tbl_Idx,
                    item.TANK_Code,

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