using AMS_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Check
{
    public partial class ARMREACTORChkController : Controller
    {
        public ActionResult ARMREACTORChkList(string ARMREACTOR_Code)
        {
            var basicInfo = armreactorBasicInfoRepository.GetARMREACTORBasicInfoByCode(ARMREACTOR_Code);
            ViewBag.ARMREACTORCode = ARMREACTOR_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Check/etc/AC Yard/ARMREACTOR/ARMREACTORChkList.cshtml");
        }

        public ActionResult ARMREACTORChkTotalList()
        {
            return View("~/Views/Check/Total/etc/AC Yard/ARMREACTORChkTotalList.cshtml");
        }

        [HttpPost]
        public ActionResult GetARMREACTORChkListData(string armreactorCode)
        {
            try
            {
                LogHelper.WriteLog("ARMREACTORChkController.List", "GetARMREACTORChkListData 실행");
                LogHelper.WriteLog("armreactorCode 조회 : ",$"{armreactorCode}");

                List<ARMREACTORChk> armreactorChks = new List<ARMREACTORChk>();
                var repoResult = armreactorChkRepository.GetARMREACTORChkByARMREACTORCode(armreactorCode, out armreactorChks);
                if (repoResult.IsSuccess)
                {
                    var formattedData = armreactorChks.Select(item => new
                    {
                        item.Tbl_Idx,
                        item.ARMREACTOR_Code,
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


                    LogHelper.WriteLog("ARMREACTORChkController.List", $"조회된 데이터: {armreactorChks.Count}건");
                    return Json(formattedData);
                }
                else
                {
                    LogHelper.WriteLog("ARMREACTORChkController.List", "ARMREACTOR 보통점검 데이터 로드 실패");
                    return Json(new { success = false, message = "ARMREACTOR 보통점검 데이터 로드 실패" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("ARMREACTORChkController.List", $"GetARMREACTORListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalARMREACTORChkListData()
        {
            // 1) 전체 점검 데이터 조회
            var repoResult = armreactorChkRepository.GetTotalARMREACTORChk(out var armreactorChks);
            if (!repoResult.IsSuccess)
                return Json(new { success = false, message = repoResult.Message });

            // 2) ARMREACTOR 기본정보 전체 조회 → 코드별 매핑용 딕셔너리 생성
            armreactorBasicInfoRepository.GetAllARMREACTORBasicInfoRepo(out var basics);
            var basicMap = basics.ToDictionary(b => b.ARMREACTOR_Code, b => b);

            // 3) JSON 응답용 객체에 Name, Serial_No 및 모든 CHK_* 필드를 한 번에 담기
            var formatted = armreactorChks.Select(item =>
            {
                basicMap.TryGetValue(item.ARMREACTOR_Code, out var basic);
                return new
                {
                    item.Tbl_Idx,
                    item.ARMREACTOR_Code,

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