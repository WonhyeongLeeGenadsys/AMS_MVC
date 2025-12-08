
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class NGRGojangController : Controller
    {
        // GET: NGRGojang
        public ActionResult NGRGojangList(string NGR_Code)
        {
            var basicInfo = ngrBasicInfoRepository.GetNGRBasicInfoByCode(NGR_Code);
            ViewBag.NGRCode = NGR_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Gojang/etc/Grounding/NGR/NGRGojangList.cshtml");
        }

        public ActionResult NGRGojangTotalList()
        {
            return View("~/Views/Gojang/Total/etc/Grounding/NGRGojangTotalList.cshtml");
        }

        /// <summary>
        /// NGR 고장 이력 데이터 가져오기
        /// </summary>
        [HttpPost]
        public JsonResult GetNGRFHByNGRCode(string ngrCode)
        {
            Result res = new Result(true);
            List<NGRFailureHistory> ngrFh = new List<NGRFailureHistory>();

            try
            {
                LogHelper.WriteLog("NGRFHController.cs", "GetNGRFHByNGRCode 실행");

                res = ngrGojangRepository.GetNGRFHByNGRCode(ngrCode, out ngrFh);

                if (!res.IsSuccess)
                {
                    LogHelper.WriteLog("NGRFHController.cs", res.Message);
                    return Json(new { success = false, message = res.Message });
                }

                if (ngrFh.Count == 0)
                {
                    LogHelper.WriteLog("NGRFHController.cs", "조회된 데이터가 없습니다.");
                    return Json(new { success = true, data = new List<NGRFailureHistory>() });
                }

                LogHelper.WriteLog("NGRFHController.cs", $"조회된 데이터: {ngrFh.Count}건");

                return Json(new { success = true, data = ngrFh });
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("NGRFHController.cs", "오류 발생: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalNGRGojangListData()
        {
            try
            {
                LogHelper.WriteLog("TotalNGRGojangController.List", "GetTotalNGRGojangListData 실행");

                List<NGRFailureHistory> ngrGojang;
                var repoResult = ngrGojangRepository.GetTotalNGRGojang(out ngrGojang);
                if (!repoResult.IsSuccess)
                    return Json(new { success = false, message = repoResult.Message });

                ngrBasicInfoRepository.GetAllNGRBasicInfoRepo(out var basics);
                var basicMap = basics.ToDictionary(b => b.NGR_Code, b => b);

                var formattedData = ngrGojang.Select(item =>
                {
                    basicMap.TryGetValue(item.NGR_Code, out var basic);
                    return new
                    {
                        item.Tbl_Idx,
                        item.NGR_Code,
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

                LogHelper.WriteLog("NGRGojangController.List", $"조회된 데이터: {formattedData.Count}건");
                return Json(formattedData);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("NGRGojangController.List", $"GetTotalNGRGojangListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}