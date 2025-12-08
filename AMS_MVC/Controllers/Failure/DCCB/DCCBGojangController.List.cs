
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class DCCBGojangController : Controller
    {
        // GET: DCCBGojang
        public ActionResult DCCBGojangList(string DCCB_Code)
        {
            var basicInfo = dccbBasicInfoRepository.GetDCCBBasicInfoByCode(DCCB_Code);
            ViewBag.DCCBCode = DCCB_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Gojang/DCCB/DCCBGojangList.cshtml");
        }

        public ActionResult DCCBGojangTotalList()
        {
            return View("~/Views/Gojang/Total/DCCBGojangTotalList.cshtml");
        }

        /// <summary>
        /// DCCB 고장 이력 데이터 가져오기
        /// </summary>
        [HttpPost]
        public JsonResult GetDCCBFHByDCCBCode(string dccbCode)
        {
            Result res = new Result(true);
            List<DCCBFailureHistory> dccbFh = new List<DCCBFailureHistory>();

            try
            {
                LogHelper.WriteLog("DCCBFHController.cs", "GetDCCBFHByDCCBCode 실행");

                res = dccbGojangRepository.GetDCCBFHByDCCBCode(dccbCode, out dccbFh);

                if (!res.IsSuccess)
                {
                    LogHelper.WriteLog("DCCBFHController.cs", res.Message);
                    return Json(new { success = false, message = res.Message });
                }

                if (dccbFh.Count == 0)
                {
                    LogHelper.WriteLog("DCCBFHController.cs", "조회된 데이터가 없습니다.");
                    return Json(new { success = true, data = new List<DCCBFailureHistory>() });
                }

                LogHelper.WriteLog("DCCBFHController.cs", $"조회된 데이터: {dccbFh.Count}건");

                return Json(new { success = true, data = dccbFh });
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("DCCBFHController.cs", "오류 발생: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalDCCBGojangListData()
        {
            try
            {
                LogHelper.WriteLog("TotalDCCBGojangController.List", "GetTotalDCCBGojangListData 실행");

                List<DCCBFailureHistory> dccbGojang;
                var repoResult = dccbGojangRepository.GetTotalDCCBGojang(out dccbGojang);
                if (!repoResult.IsSuccess)
                    return Json(new { success = false, message = repoResult.Message });

                dccbBasicInfoRepository.GetAllDCCBBasicInfoRepo(out var basics);
                var basicMap = basics.ToDictionary(b => b.DCCB_Code, b => b);

                var formattedData = dccbGojang.Select(item =>
                {
                    basicMap.TryGetValue(item.DCCB_Code, out var basic);
                    return new
                    {
                        item.Tbl_Idx,
                        item.DCCB_Code,
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

                LogHelper.WriteLog("DCCBGojangController.List", $"조회된 데이터: {formattedData.Count}건");
                return Json(formattedData);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("DCCBGojangController.List", $"GetTotalDCCBGojangListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}