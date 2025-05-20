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
    public partial class ZIGZAGTRGojangController : Controller
    {
        // GET: ZIGZAGTRGojang
        public ActionResult ZIGZAGTRGojangList(string ZIGZAGTR_Code)
        {
            var basicInfo = zigzagtrBasicInfoRepository.GetZIGZAGTRBasicInfoByCode(ZIGZAGTR_Code);
            ViewBag.ZIGZAGTRCode = ZIGZAGTR_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Gojang/etc/Grounding/ZIGZAGTR/ZIGZAGTRGojangList.cshtml");
        }

        public ActionResult ZIGZAGTRGojangTotalList()
        {
            return View("~/Views/Gojang/Total/etc/Grounding/ZIGZAGTRGojangTotalList.cshtml");
        }

        /// <summary>
        /// ZIGZAGTR 고장 이력 데이터 가져오기
        /// </summary>
        [HttpPost]
        public JsonResult GetZIGZAGTRFHByZIGZAGTRCode(string zigzagtrCode)
        {
            Result res = new Result(true);
            List<ZIGZAGTRFailureHistory> zigzagtrFh = new List<ZIGZAGTRFailureHistory>();

            try
            {
                LogHelper.WriteLog("ZIGZAGTRFHController.cs", "GetZIGZAGTRFHByZIGZAGTRCode 실행");

                res = zigzagtrGojangRepository.GetZIGZAGTRFHByZIGZAGTRCode(zigzagtrCode, out zigzagtrFh);

                if (!res.IsSuccess)
                {
                    LogHelper.WriteLog("ZIGZAGTRFHController.cs", res.Message);
                    return Json(new { success = false, message = res.Message });
                }

                if (zigzagtrFh.Count == 0)
                {
                    LogHelper.WriteLog("ZIGZAGTRFHController.cs", "조회된 데이터가 없습니다.");
                    return Json(new { success = true, data = new List<ZIGZAGTRFailureHistory>() });
                }

                LogHelper.WriteLog("ZIGZAGTRFHController.cs", $"조회된 데이터: {zigzagtrFh.Count}건");

                return Json(new { success = true, data = zigzagtrFh });
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("ZIGZAGTRFHController.cs", "오류 발생: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalZIGZAGTRGojangListData()
        {
            try
            {
                LogHelper.WriteLog("TotalZIGZAGTRGojangController.List", "GetTotalZIGZAGTRGojangListData 실행");

                List<ZIGZAGTRFailureHistory> zigzagtrGojang;
                var repoResult = zigzagtrGojangRepository.GetTotalZIGZAGTRGojang(out zigzagtrGojang);
                if (!repoResult.IsSuccess)
                    return Json(new { success = false, message = repoResult.Message });

                zigzagtrBasicInfoRepository.GetAllZIGZAGTRBasicInfoRepo(out var basics);
                var basicMap = basics.ToDictionary(b => b.ZIGZAGTR_Code, b => b);

                var formattedData = zigzagtrGojang.Select(item =>
                {
                    basicMap.TryGetValue(item.ZIGZAGTR_Code, out var basic);
                    return new
                    {
                        item.Tbl_Idx,
                        item.ZIGZAGTR_Code,
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

                LogHelper.WriteLog("ZIGZAGTRGojangController.List", $"조회된 데이터: {formattedData.Count}건");
                return Json(formattedData);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("ZIGZAGTRGojangController.List", $"GetTotalZIGZAGTRGojangListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}