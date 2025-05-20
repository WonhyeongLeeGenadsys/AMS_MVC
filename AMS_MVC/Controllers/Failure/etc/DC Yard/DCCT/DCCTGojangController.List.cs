using AMS_MVC.Models;
using AMS_MVC.Utlity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers
{
    public partial class DCCTGojangController : Controller
    {
        // GET: DCCTGojang
        public ActionResult DCCTGojangList(string DCCT_Code)
        {
            var basicInfo = dcctBasicInfoRepository.GetDCCTBasicInfoByCode(DCCT_Code);
            ViewBag.DCCTCode = DCCT_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Gojang/etc/DC Yard/DCCT/DCCTGojangList.cshtml");
        }

        public ActionResult DCCTGojangTotalList()
        {
            return View("~/Views/Gojang/Total/etc/DC Yard/DCCTGojangTotalList.cshtml");
        }

        /// <summary>
        /// DCCT 고장 이력 데이터 가져오기
        /// </summary>
        [HttpPost]
        public JsonResult GetDCCTFHByDCCTCode(string dcctCode)
        {
            Result res = new Result(true);
            List<DCCTFailureHistory> dcctFh = new List<DCCTFailureHistory>();

            try
            {
                LogHelper.WriteLog("DCCTFHController.cs", "GetDCCTFHByDCCTCode 실행");

                res = dcctGojangRepository.GetDCCTFHByDCCTCode(dcctCode, out dcctFh);

                if (!res.IsSuccess)
                {
                    LogHelper.WriteLog("DCCTFHController.cs", res.Message);
                    return Json(new { success = false, message = res.Message });
                }

                if (dcctFh.Count == 0)
                {
                    LogHelper.WriteLog("DCCTFHController.cs", "조회된 데이터가 없습니다.");
                    return Json(new { success = true, data = new List<DCCTFailureHistory>() });
                }

                LogHelper.WriteLog("DCCTFHController.cs", $"조회된 데이터: {dcctFh.Count}건");

                return Json(new { success = true, data = dcctFh });
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("DCCTFHController.cs", "오류 발생: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalDCCTGojangListData()
        {
            try
            {
                LogHelper.WriteLog("TotalDCCTGojangController.List", "GetTotalDCCTGojangListData 실행");

                List<DCCTFailureHistory> dcctGojang;
                var repoResult = dcctGojangRepository.GetTotalDCCTGojang(out dcctGojang);
                if (!repoResult.IsSuccess)
                    return Json(new { success = false, message = repoResult.Message });

                dcctBasicInfoRepository.GetAllDCCTBasicInfoRepo(out var basics);
                var basicMap = basics.ToDictionary(b => b.DCCT_Code, b => b);

                var formattedData = dcctGojang.Select(item =>
                {
                    basicMap.TryGetValue(item.DCCT_Code, out var basic);
                    return new
                    {
                        item.Tbl_Idx,
                        item.DCCT_Code,
                        Name = basic?.Name ?? "",
                        Serial_No = basic?.Serial_No ?? "",
                        item.Fail_Gojang_Name,
                        item.Fail_Weather,
                        item.Fail_Temp,
                        item.Fail_Hum,
                        item.Fail_Cause,
                        item.Fail_Reason,
                        item.Fail_Status,
                        item.Fail_Part,
                        item.Fail_Period,
                        item.Fail_Finder,
                        item.Fail_Repairer,
                        item.Fail_Supervisor,
                        Fail_Repair_Date = item.Fail_Repair_Date?.ToString("yy.MM.dd"),
                        item.Fail_Writer
                    };
                }).ToList();

                LogHelper.WriteLog("DCCTGojangController.List", $"조회된 데이터: {formattedData.Count}건");
                return Json(formattedData);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("DCCTGojangController.List", $"GetTotalDCCTGojangListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}