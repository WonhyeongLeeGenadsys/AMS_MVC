
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class ESGojangController : Controller
    {
        // GET: ESGojang
        public ActionResult ESGojangList(string ES_Code)
        {
            var basicInfo = esBasicInfoRepository.GetESBasicInfoByCode(ES_Code);
            ViewBag.ESCode = ES_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Gojang/etc/Grounding/ES/ESGojangList.cshtml");
        }

        public ActionResult ESGojangTotalList()
        {
            return View("~/Views/Gojang/Total/etc/Grounding/ESGojangTotalList.cshtml");
        }

        /// <summary>
        /// ES 고장 이력 데이터 가져오기
        /// </summary>
        [HttpPost]
        public JsonResult GetESFHByESCode(string esCode)
        {
            Result res = new Result(true);
            List<ESFailureHistory> esFh = new List<ESFailureHistory>();

            try
            {
                LogHelper.WriteLog("ESFHController.cs", "GetESFHByESCode 실행");

                res = esGojangRepository.GetESFHByESCode(esCode, out esFh);

                if (!res.IsSuccess)
                {
                    LogHelper.WriteLog("ESFHController.cs", res.Message);
                    return Json(new { success = false, message = res.Message });
                }

                if (esFh.Count == 0)
                {
                    LogHelper.WriteLog("ESFHController.cs", "조회된 데이터가 없습니다.");
                    return Json(new { success = true, data = new List<ESFailureHistory>() });
                }

                LogHelper.WriteLog("ESFHController.cs", $"조회된 데이터: {esFh.Count}건");

                return Json(new { success = true, data = esFh });
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("ESFHController.cs", "오류 발생: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalESGojangListData()
        {
            try
            {
                LogHelper.WriteLog("TotalESGojangController.List", "GetTotalESGojangListData 실행");

                List<ESFailureHistory> esGojang;
                var repoResult = esGojangRepository.GetTotalESGojang(out esGojang);
                if (!repoResult.IsSuccess)
                    return Json(new { success = false, message = repoResult.Message });

                esBasicInfoRepository.GetAllESBasicInfoRepo(out var basics);
                var basicMap = basics.ToDictionary(b => b.ES_Code, b => b);

                var formattedData = esGojang.Select(item =>
                {
                    basicMap.TryGetValue(item.ES_Code, out var basic);
                    return new
                    {
                        item.Tbl_Idx,
                        item.ES_Code,
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

                LogHelper.WriteLog("ESGojangController.List", $"조회된 데이터: {formattedData.Count}건");
                return Json(formattedData);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("ESGojangController.List", $"GetTotalESGojangListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}