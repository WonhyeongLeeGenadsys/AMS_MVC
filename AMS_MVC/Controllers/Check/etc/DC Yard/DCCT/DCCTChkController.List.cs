using AMS_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Check
{
    public partial class DCCTChkController : Controller
    {
        public ActionResult DCCTChkList(string DCCT_Code)
        {
            var basicInfo = dcctBasicInfoRepository.GetDCCTBasicInfoByCode(DCCT_Code);
            ViewBag.DCCTCode = DCCT_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Check/etc/DC Yard/DCCT/DCCTChkList.cshtml");
        }

        public ActionResult DCCTChkTotalList()
        {
            return View("~/Views/Check/Total/etc/DC Yard/DCCTChkTotalList.cshtml");
        }

        [HttpPost]
        public ActionResult GetDCCTChkListData(string dcctCode)
        {
            try
            {
                LogHelper.WriteLog("DCCTChkController.List", "GetDCCTChkListData 실행");
                LogHelper.WriteLog("dcctCode 조회 : ",$"{dcctCode}");

                List<DCCTChk> dcctChks = new List<DCCTChk>();
                var repoResult = dcctChkRepository.GetDCCTChkByDCCTCode(dcctCode, out dcctChks);
                if (repoResult.IsSuccess)
                {
                    var formattedData = dcctChks.Select(item => new
                    {
                        item.Tbl_Idx,
                        item.DCCT_Code,
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


                    LogHelper.WriteLog("DCCTChkController.List", $"조회된 데이터: {dcctChks.Count}건");
                    return Json(formattedData);
                }
                else
                {
                    LogHelper.WriteLog("DCCTChkController.List", "DCCT 보통점검 데이터 로드 실패");
                    return Json(new { success = false, message = "DCCT 보통점검 데이터 로드 실패" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("DCCTChkController.List", $"GetDCCTListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalDCCTChkListData()
        {
            // 1) 전체 점검 데이터 조회
            var repoResult = dcctChkRepository.GetTotalDCCTChk(out var dcctChks);
            if (!repoResult.IsSuccess)
                return Json(new { success = false, message = repoResult.Message });

            // 2) DCCT 기본정보 전체 조회 → 코드별 매핑용 딕셔너리 생성
            dcctBasicInfoRepository.GetAllDCCTBasicInfoRepo(out var basics);
            var basicMap = basics.ToDictionary(b => b.DCCT_Code, b => b);

            // 3) JSON 응답용 객체에 Name, Serial_No 및 모든 CHK_* 필드를 한 번에 담기
            var formatted = dcctChks.Select(item =>
            {
                basicMap.TryGetValue(item.DCCT_Code, out var basic);
                return new
                {
                    item.Tbl_Idx,
                    item.DCCT_Code,

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