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
    public partial class ITRGojangController : Controller
    {
        // GET: ITRGojang
        public ActionResult ITRGojangList(string ITR_Code)
        {
            var basicInfo = itrBasicInfoRepository.GetITRBasicInfoByITRCode(ITR_Code);
            ViewBag.ITRCode = ITR_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Gojang/ITR/ITRGojangList.cshtml");
        }

        public ActionResult ITRGojangTotalList()
        {
            return View("~/Views/Gojang/Total/ITRGojangTotalList.cshtml");
        }

        /// <summary>
        /// VCB 고장 이력 데이터 가져오기
        /// </summary>
        [HttpPost]
        public JsonResult GetITRFHByITRCode(string itrCode)
        {
            Result res = new Result(true);
            List<ITRFailureHistory> itrGojang = new List<ITRFailureHistory>();

            try
            {
                LogHelper.WriteLog("ITRFHController.cs", "GetITRFHByITRCode 실행");

                res = itrGojangRepository.GetITRFHByITRCode(itrCode, out itrGojang);

                if (!res.IsSuccess)
                {
                    LogHelper.WriteLog("ITRFHController.cs", res.Message);
                    return Json(new { success = false, message = res.Message });
                }

                if (itrGojang.Count == 0)
                {
                    LogHelper.WriteLog("ITRFHController.cs", "조회된 데이터가 없습니다.");
                    return Json(new { success = true, data = new List<ITRFailureHistory>() });
                }

                LogHelper.WriteLog("ITRFHController.cs", $"조회된 데이터: {itrGojang.Count}건");

                return Json(new { success = true, data = itrGojang });
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("ITRFHController.cs", "오류 발생: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalITRGojangListData()
        {
            try
            {
                LogHelper.WriteLog("TotalITRGojangController.List", "GetTotalITRGojangListData 실행");

                List<ITRFailureHistory> itrGojang;
                var repoResult = itrGojangRepository.GetTotalITRGojang(out itrGojang);
                if (!repoResult.IsSuccess)
                    return Json(new { success = false, message = repoResult.Message });

                itrBasicInfoRepository.GetAllITRBasicInfoRepo(out var basics);
                var basicMap = basics.ToDictionary(b => b.ITR_Code, b => b);

                var formattedData = itrGojang.Select(item =>
                {
                    basicMap.TryGetValue(item.ITR_Code, out var basic);
                    return new
                    {
                        item.Tbl_Idx,
                        item.ITR_Code,
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

                LogHelper.WriteLog("ITRGojangController.List", $"조회된 데이터: {formattedData.Count}건");
                return Json(formattedData);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("ITRGojangController.List", $"GetTotalITRGojangListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}