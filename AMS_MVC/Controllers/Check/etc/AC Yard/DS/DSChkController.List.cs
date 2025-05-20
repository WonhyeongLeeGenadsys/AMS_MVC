using AMS_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Check
{
    public partial class DSChkController : Controller
    {
        public ActionResult DSChkList(string DS_Code)
        {
            var basicInfo = dsBasicInfoRepository.GetDSBasicInfoByCode(DS_Code);
            ViewBag.DSCode = DS_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Check/etc/AC Yard/DS/DSChkList.cshtml");
        }

        public ActionResult DSChkTotalList()
        {
            return View("~/Views/Check/Total/etc/AC Yard/DSChkTotalList.cshtml");
        }

        [HttpPost]
        public ActionResult GetDSChkListData(string dsCode)
        {
            try
            {
                LogHelper.WriteLog("DSChkController.List", "GetDSChkListData 실행");
                LogHelper.WriteLog("dsCode 조회 : ",$"{dsCode}");

                List<DSChk> dsChks = new List<DSChk>();
                var repoResult = dsChkRepository.GetDSChkByDSCode(dsCode, out dsChks);
                if (repoResult.IsSuccess)
                {
                    var formattedData = dsChks.Select(item => new
                    {
                        item.Tbl_Idx,
                        item.DS_Code,
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


                    LogHelper.WriteLog("DSChkController.List", $"조회된 데이터: {dsChks.Count}건");
                    return Json(formattedData);
                }
                else
                {
                    LogHelper.WriteLog("DSChkController.List", "DS 보통점검 데이터 로드 실패");
                    return Json(new { success = false, message = "DS 보통점검 데이터 로드 실패" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("DSChkController.List", $"GetDSListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalDSChkListData()
        {
            // 1) 전체 점검 데이터 조회
            var repoResult = dsChkRepository.GetTotalDSChk(out var dsChks);
            if (!repoResult.IsSuccess)
                return Json(new { success = false, message = repoResult.Message });

            // 2) DS 기본정보 전체 조회 → 코드별 매핑용 딕셔너리 생성
            dsBasicInfoRepository.GetAllDSBasicInfoRepo(out var basics);
            var basicMap = basics.ToDictionary(b => b.DS_Code, b => b);

            // 3) JSON 응답용 객체에 Name, Serial_No 및 모든 CHK_* 필드를 한 번에 담기
            var formatted = dsChks.Select(item =>
            {
                basicMap.TryGetValue(item.DS_Code, out var basic);
                return new
                {
                    item.Tbl_Idx,
                    item.DS_Code,

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