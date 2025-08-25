
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class DSGojangController : Controller
    {
        // GET: DSGojang
        public ActionResult DSGojangList(string DS_Code)
        {
            var basicInfo = dsBasicInfoRepository.GetDSBasicInfoByCode(DS_Code);
            ViewBag.DSCode = DS_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Gojang/etc/AC Yard/DS/DSGojangList.cshtml");
        }

        public ActionResult DSGojangTotalList()
        {
            return View("~/Views/Gojang/Total/etc/AC Yard/DSGojangTotalList.cshtml");
        }

        /// <summary>
        /// DS 고장 이력 데이터 가져오기
        /// </summary>
        [HttpPost]
        public JsonResult GetDSFHByDSCode(string dsCode)
        {
            Result res = new Result(true);
            List<DSFailureHistory> dsFh = new List<DSFailureHistory>();

            try
            {
                LogHelper.WriteLog("DSFHController.cs", "GetDSFHByDSCode 실행");

                res = dsGojangRepository.GetDSFHByDSCode(dsCode, out dsFh);

                if (!res.IsSuccess)
                {
                    LogHelper.WriteLog("DSFHController.cs", res.Message);
                    return Json(new { success = false, message = res.Message });
                }

                if (dsFh.Count == 0)
                {
                    LogHelper.WriteLog("DSFHController.cs", "조회된 데이터가 없습니다.");
                    return Json(new { success = true, data = new List<DSFailureHistory>() });
                }

                LogHelper.WriteLog("DSFHController.cs", $"조회된 데이터: {dsFh.Count}건");

                return Json(new { success = true, data = dsFh });
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("DSFHController.cs", "오류 발생: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalDSGojangListData()
        {
            try
            {
                LogHelper.WriteLog("TotalDSGojangController.List", "GetTotalDSGojangListData 실행");

                List<DSFailureHistory> dsGojang;
                var repoResult = dsGojangRepository.GetTotalDSGojang(out dsGojang);
                if (!repoResult.IsSuccess)
                    return Json(new { success = false, message = repoResult.Message });

                dsBasicInfoRepository.GetAllDSBasicInfoRepo(out var basics);
                var basicMap = basics.ToDictionary(b => b.DS_Code, b => b);

                var formattedData = dsGojang.Select(item =>
                {
                    basicMap.TryGetValue(item.DS_Code, out var basic);
                    return new
                    {
                        item.Tbl_Idx,
                        item.DS_Code,
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

                LogHelper.WriteLog("DSGojangController.List", $"조회된 데이터: {formattedData.Count}건");
                return Json(formattedData);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("DSGojangController.List", $"GetTotalDSGojangListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}