
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class PUMPGojangController : Controller
    {
        // GET: PUMPGojang
        public ActionResult PUMPGojangList(string PUMP_Code)
        {
            var basicInfo = pumpBasicInfoRepository.GetPUMPBasicInfoByCode(PUMP_Code);
            ViewBag.PUMPCode = PUMP_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Gojang/etc/Cooling System/PUMP/PUMPGojangList.cshtml");
        }

        public ActionResult PUMPGojangTotalList()
        {
            return View("~/Views/Gojang/Total/etc/Cooling System/PUMPGojangTotalList.cshtml");
        }

        /// <summary>
        /// PUMP 고장 이력 데이터 가져오기
        /// </summary>
        [HttpPost]
        public JsonResult GetPUMPFHByPUMPCode(string pumpCode)
        {
            Result res = new Result(true);
            List<PUMPFailureHistory> pumpFh = new List<PUMPFailureHistory>();

            try
            {
                LogHelper.WriteLog("PUMPFHController.cs", "GetPUMPFHByPUMPCode 실행");

                res = pumpGojangRepository.GetPUMPFHByPUMPCode(pumpCode, out pumpFh);

                if (!res.IsSuccess)
                {
                    LogHelper.WriteLog("PUMPFHController.cs", res.Message);
                    return Json(new { success = false, message = res.Message });
                }

                if (pumpFh.Count == 0)
                {
                    LogHelper.WriteLog("PUMPFHController.cs", "조회된 데이터가 없습니다.");
                    return Json(new { success = true, data = new List<PUMPFailureHistory>() });
                }

                LogHelper.WriteLog("PUMPFHController.cs", $"조회된 데이터: {pumpFh.Count}건");

                return Json(new { success = true, data = pumpFh });
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("PUMPFHController.cs", "오류 발생: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalPUMPGojangListData()
        {
            try
            {
                LogHelper.WriteLog("TotalPUMPGojangController.List", "GetTotalPUMPGojangListData 실행");

                List<PUMPFailureHistory> pumpGojang;
                var repoResult = pumpGojangRepository.GetTotalPUMPGojang(out pumpGojang);
                if (!repoResult.IsSuccess)
                    return Json(new { success = false, message = repoResult.Message });

                pumpBasicInfoRepository.GetAllPUMPBasicInfoRepo(out var basics);
                var basicMap = basics.ToDictionary(b => b.PUMP_Code, b => b);

                var formattedData = pumpGojang.Select(item =>
                {
                    basicMap.TryGetValue(item.PUMP_Code, out var basic);
                    return new
                    {
                        item.Tbl_Idx,
                        item.PUMP_Code,
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

                LogHelper.WriteLog("PUMPGojangController.List", $"조회된 데이터: {formattedData.Count}건");
                return Json(formattedData);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("PUMPGojangController.List", $"GetTotalPUMPGojangListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}