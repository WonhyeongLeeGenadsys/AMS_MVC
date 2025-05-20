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
    public partial class BYPASSVALVEGojangController : Controller
    {
        // GET: BYPASSVALVEGojang
        public ActionResult BYPASSVALVEGojangList(string BYPASSVALVE_Code)
        {
            var basicInfo = bypassvalveBasicInfoRepository.GetBYPASSVALVEBasicInfoByCode(BYPASSVALVE_Code);
            ViewBag.BYPASSVALVECode = BYPASSVALVE_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Gojang/etc/Cooling System/BYPASSVALVE/BYPASSVALVEGojangList.cshtml");
        }

        public ActionResult BYPASSVALVEGojangTotalList()
        {
            return View("~/Views/Gojang/Total/etc/Cooling System/BYPASSVALVEGojangTotalList.cshtml");
        }

        /// <summary>
        /// BYPASSVALVE 고장 이력 데이터 가져오기
        /// </summary>
        [HttpPost]
        public JsonResult GetBYPASSVALVEFHByBYPASSVALVECode(string bypassvalveCode)
        {
            Result res = new Result(true);
            List<BYPASSVALVEFailureHistory> bypassvalveFh = new List<BYPASSVALVEFailureHistory>();

            try
            {
                LogHelper.WriteLog("BYPASSVALVEFHController.cs", "GetBYPASSVALVEFHByBYPASSVALVECode 실행");

                res = bypassvalveGojangRepository.GetBYPASSVALVEFHByBYPASSVALVECode(bypassvalveCode, out bypassvalveFh);

                if (!res.IsSuccess)
                {
                    LogHelper.WriteLog("BYPASSVALVEFHController.cs", res.Message);
                    return Json(new { success = false, message = res.Message });
                }

                if (bypassvalveFh.Count == 0)
                {
                    LogHelper.WriteLog("BYPASSVALVEFHController.cs", "조회된 데이터가 없습니다.");
                    return Json(new { success = true, data = new List<BYPASSVALVEFailureHistory>() });
                }

                LogHelper.WriteLog("BYPASSVALVEFHController.cs", $"조회된 데이터: {bypassvalveFh.Count}건");

                return Json(new { success = true, data = bypassvalveFh });
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("BYPASSVALVEFHController.cs", "오류 발생: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalBYPASSVALVEGojangListData()
        {
            try
            {
                LogHelper.WriteLog("TotalBYPASSVALVEGojangController.List", "GetTotalBYPASSVALVEGojangListData 실행");

                List<BYPASSVALVEFailureHistory> bypassvalveGojang;
                var repoResult = bypassvalveGojangRepository.GetTotalBYPASSVALVEGojang(out bypassvalveGojang);
                if (!repoResult.IsSuccess)
                    return Json(new { success = false, message = repoResult.Message });

                bypassvalveBasicInfoRepository.GetAllBYPASSVALVEBasicInfoRepo(out var basics);
                var basicMap = basics.ToDictionary(b => b.BYPASSVALVE_Code, b => b);

                var formattedData = bypassvalveGojang.Select(item =>
                {
                    basicMap.TryGetValue(item.BYPASSVALVE_Code, out var basic);
                    return new
                    {
                        item.Tbl_Idx,
                        item.BYPASSVALVE_Code,
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

                LogHelper.WriteLog("BYPASSVALVEGojangController.List", $"조회된 데이터: {formattedData.Count}건");
                return Json(formattedData);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("BYPASSVALVEGojangController.List", $"GetTotalBYPASSVALVEGojangListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}