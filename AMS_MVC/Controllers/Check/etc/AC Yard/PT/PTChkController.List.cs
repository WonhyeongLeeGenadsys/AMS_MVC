using AMS_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Check
{
    public partial class PTChkController : Controller
    {
        public ActionResult PTChkList(string PT_Code)
        {
            var basicInfo = ptBasicInfoRepository.GetPTBasicInfoByCode(PT_Code);
            ViewBag.PTCode = PT_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Check/etc/AC Yard/PT/PTChkList.cshtml");
        }

        public ActionResult PTChkTotalList()
        {
            return View("~/Views/Check/Total/etc/AC Yard/PTChkTotalList.cshtml");
        }

        [HttpPost]
        public ActionResult GetPTChkListData(string ptCode)
        {
            try
            {
                LogHelper.WriteLog("PTChkController.List", "GetPTChkListData 실행");
                LogHelper.WriteLog("ptCode 조회 : ",$"{ptCode}");

                List<PTChk> ptChks = new List<PTChk>();
                var repoResult = ptChkRepository.GetPTChkByPTCode(ptCode, out ptChks);
                if (repoResult.IsSuccess)
                {
                    var formattedData = ptChks.Select(item => new
                    {
                        item.Tbl_Idx,
                        item.PT_Code,
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


                    LogHelper.WriteLog("PTChkController.List", $"조회된 데이터: {ptChks.Count}건");
                    return Json(formattedData);
                }
                else
                {
                    LogHelper.WriteLog("PTChkController.List", "PT 보통점검 데이터 로드 실패");
                    return Json(new { success = false, message = "PT 보통점검 데이터 로드 실패" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("PTChkController.List", $"GetPTListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalPTChkListData()
        {
            // 1) 전체 점검 데이터 조회
            var repoResult = ptChkRepository.GetTotalPTChk(out var ptChks);
            if (!repoResult.IsSuccess)
                return Json(new { success = false, message = repoResult.Message });

            // 2) PT 기본정보 전체 조회 → 코드별 매핑용 딕셔너리 생성
            ptBasicInfoRepository.GetAllPTBasicInfoRepo(out var basics);
            var basicMap = basics.ToDictionary(b => b.PT_Code, b => b);

            // 3) JSON 응답용 객체에 Name, Serial_No 및 모든 CHK_* 필드를 한 번에 담기
            var formatted = ptChks.Select(item =>
            {
                basicMap.TryGetValue(item.PT_Code, out var basic);
                return new
                {
                    item.Tbl_Idx,
                    item.PT_Code,

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