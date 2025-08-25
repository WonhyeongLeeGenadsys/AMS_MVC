
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class SAGojangController : Controller
    {
        // GET: SAGojang
        public ActionResult SAGojangList(string SA_Code)
        {
            var basicInfo = saBasicInfoRepository.GetSABasicInfoByCode(SA_Code);
            ViewBag.SACode = SA_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Gojang/etc/DC Yard/SA/SAGojangList.cshtml");
        }

        public ActionResult SAGojangTotalList()
        {
            return View("~/Views/Gojang/Total/etc/DC Yard/SAGojangTotalList.cshtml");
        }

        /// <summary>
        /// SA 고장 이력 데이터 가져오기
        /// </summary>
        [HttpPost]
        public JsonResult GetSAFHBySACode(string saCode)
        {
            Result res = new Result(true);
            List<SAFailureHistory> saFh = new List<SAFailureHistory>();

            try
            {
                LogHelper.WriteLog("SAFHController.cs", "GetSAFHBySACode 실행");

                res = saGojangRepository.GetSAFHBySACode(saCode, out saFh);

                if (!res.IsSuccess)
                {
                    LogHelper.WriteLog("SAFHController.cs", res.Message);
                    return Json(new { success = false, message = res.Message });
                }

                if (saFh.Count == 0)
                {
                    LogHelper.WriteLog("SAFHController.cs", "조회된 데이터가 없습니다.");
                    return Json(new { success = true, data = new List<SAFailureHistory>() });
                }

                LogHelper.WriteLog("SAFHController.cs", $"조회된 데이터: {saFh.Count}건");

                return Json(new { success = true, data = saFh });
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("SAFHController.cs", "오류 발생: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalSAGojangListData()
        {
            try
            {
                LogHelper.WriteLog("TotalSAGojangController.List", "GetTotalSAGojangListData 실행");

                List<SAFailureHistory> saGojang;
                var repoResult = saGojangRepository.GetTotalSAGojang(out saGojang);
                if (!repoResult.IsSuccess)
                    return Json(new { success = false, message = repoResult.Message });

                saBasicInfoRepository.GetAllSABasicInfoRepo(out var basics);
                var basicMap = basics.ToDictionary(b => b.SA_Code, b => b);

                var formattedData = saGojang.Select(item =>
                {
                    basicMap.TryGetValue(item.SA_Code, out var basic);
                    return new
                    {
                        item.Tbl_Idx,
                        item.SA_Code,
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

                LogHelper.WriteLog("SAGojangController.List", $"조회된 데이터: {formattedData.Count}건");
                return Json(formattedData);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("SAGojangController.List", $"GetTotalSAGojangListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}