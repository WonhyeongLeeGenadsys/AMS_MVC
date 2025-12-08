
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class HEATEXCHANGERGojangController : Controller
    {
        // GET: HEATEXCHANGERGojang
        public ActionResult HEATEXCHANGERGojangList(string HEATEXCHANGER_Code)
        {
            var basicInfo = heatexchangerBasicInfoRepository.GetHEATEXCHANGERBasicInfoByCode(HEATEXCHANGER_Code);
            ViewBag.HEATEXCHANGERCode = HEATEXCHANGER_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Gojang/etc/Cooling System/HEATEXCHANGER/HEATEXCHANGERGojangList.cshtml");
        }

        public ActionResult HEATEXCHANGERGojangTotalList()
        {
            return View("~/Views/Gojang/Total/etc/Cooling System/HEATEXCHANGERGojangTotalList.cshtml");
        }

        /// <summary>
        /// HEATEXCHANGER 고장 이력 데이터 가져오기
        /// </summary>
        [HttpPost]
        public JsonResult GetHEATEXCHANGERFHByHEATEXCHANGERCode(string heatexchangerCode)
        {
            Result res = new Result(true);
            List<HEATEXCHANGERFailureHistory> heatexchangerFh = new List<HEATEXCHANGERFailureHistory>();

            try
            {
                LogHelper.WriteLog("HEATEXCHANGERFHController.cs", "GetHEATEXCHANGERFHByHEATEXCHANGERCode 실행");

                res = heatexchangerGojangRepository.GetHEATEXCHANGERFHByHEATEXCHANGERCode(heatexchangerCode, out heatexchangerFh);

                if (!res.IsSuccess)
                {
                    LogHelper.WriteLog("HEATEXCHANGERFHController.cs", res.Message);
                    return Json(new { success = false, message = res.Message });
                }

                if (heatexchangerFh.Count == 0)
                {
                    LogHelper.WriteLog("HEATEXCHANGERFHController.cs", "조회된 데이터가 없습니다.");
                    return Json(new { success = true, data = new List<HEATEXCHANGERFailureHistory>() });
                }

                LogHelper.WriteLog("HEATEXCHANGERFHController.cs", $"조회된 데이터: {heatexchangerFh.Count}건");

                return Json(new { success = true, data = heatexchangerFh });
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("HEATEXCHANGERFHController.cs", "오류 발생: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalHEATEXCHANGERGojangListData()
        {
            try
            {
                LogHelper.WriteLog("TotalHEATEXCHANGERGojangController.List", "GetTotalHEATEXCHANGERGojangListData 실행");

                List<HEATEXCHANGERFailureHistory> heatexchangerGojang;
                var repoResult = heatexchangerGojangRepository.GetTotalHEATEXCHANGERGojang(out heatexchangerGojang);
                if (!repoResult.IsSuccess)
                    return Json(new { success = false, message = repoResult.Message });

                heatexchangerBasicInfoRepository.GetAllHEATEXCHANGERBasicInfoRepo(out var basics);
                var basicMap = basics.ToDictionary(b => b.HEATEXCHANGER_Code, b => b);

                var formattedData = heatexchangerGojang.Select(item =>
                {
                    basicMap.TryGetValue(item.HEATEXCHANGER_Code, out var basic);
                    return new
                    {
                        item.Tbl_Idx,
                        item.HEATEXCHANGER_Code,
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

                LogHelper.WriteLog("HEATEXCHANGERGojangController.List", $"조회된 데이터: {formattedData.Count}건");
                return Json(formattedData);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("HEATEXCHANGERGojangController.List", $"GetTotalHEATEXCHANGERGojangListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}