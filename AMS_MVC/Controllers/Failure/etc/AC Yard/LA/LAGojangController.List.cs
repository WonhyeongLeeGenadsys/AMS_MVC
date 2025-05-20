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
    public partial class LAGojangController : Controller
    {
        // GET: LAGojang
        public ActionResult LAGojangList(string LA_Code)
        {
            var basicInfo = laBasicInfoRepository.GetLABasicInfoByCode(LA_Code);
            ViewBag.LACode = LA_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Gojang/etc/AC Yard/LA/LAGojangList.cshtml");
        }

        public ActionResult LAGojangTotalList()
        {
            return View("~/Views/Gojang/Total/etc/AC Yard/LAGojangTotalList.cshtml");
        }

        /// <summary>
        /// LA 고장 이력 데이터 가져오기
        /// </summary>
        [HttpPost]
        public JsonResult GetLAFHByLACode(string laCode)
        {
            Result res = new Result(true);
            List<LAFailureHistory> laFh = new List<LAFailureHistory>();

            try
            {
                LogHelper.WriteLog("LAFHController.cs", "GetLAFHByLACode 실행");

                res = laGojangRepository.GetLAFHByLACode(laCode, out laFh);

                if (!res.IsSuccess)
                {
                    LogHelper.WriteLog("LAFHController.cs", res.Message);
                    return Json(new { success = false, message = res.Message });
                }

                if (laFh.Count == 0)
                {
                    LogHelper.WriteLog("LAFHController.cs", "조회된 데이터가 없습니다.");
                    return Json(new { success = true, data = new List<LAFailureHistory>() });
                }

                LogHelper.WriteLog("LAFHController.cs", $"조회된 데이터: {laFh.Count}건");

                return Json(new { success = true, data = laFh });
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("LAFHController.cs", "오류 발생: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalLAGojangListData()
        {
            try
            {
                LogHelper.WriteLog("TotalLAGojangController.List", "GetTotalLAGojangListData 실행");

                List<LAFailureHistory> laGojang;
                var repoResult = laGojangRepository.GetTotalLAGojang(out laGojang);
                if (!repoResult.IsSuccess)
                    return Json(new { success = false, message = repoResult.Message });

                laBasicInfoRepository.GetAllLABasicInfoRepo(out var basics);
                var basicMap = basics.ToDictionary(b => b.LA_Code, b => b);

                var formattedData = laGojang.Select(item =>
                {
                    basicMap.TryGetValue(item.LA_Code, out var basic);
                    return new
                    {
                        item.Tbl_Idx,
                        item.LA_Code,
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

                LogHelper.WriteLog("LAGojangController.List", $"조회된 데이터: {formattedData.Count}건");
                return Json(formattedData);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("LAGojangController.List", $"GetTotalLAGojangListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}