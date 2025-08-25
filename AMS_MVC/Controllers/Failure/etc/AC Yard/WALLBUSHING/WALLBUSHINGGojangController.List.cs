
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class WALLBUSHINGGojangController : Controller
    {
        // GET: WALLBUSHINGGojang
        public ActionResult WALLBUSHINGGojangList(string WALLBUSHING_Code)
        {
            var basicInfo = wallbushingBasicInfoRepository.GetWALLBUSHINGBasicInfoByCode(WALLBUSHING_Code);
            ViewBag.WALLBUSHINGCode = WALLBUSHING_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Gojang/etc/AC Yard/WALLBUSHING/WALLBUSHINGGojangList.cshtml");
        }

        public ActionResult WALLBUSHINGGojangTotalList()
        {
            return View("~/Views/Gojang/Total/etc/AC Yard/WALLBUSHINGGojangTotalList.cshtml");
        }

        /// <summary>
        /// WALLBUSHING 고장 이력 데이터 가져오기
        /// </summary>
        [HttpPost]
        public JsonResult GetWALLBUSHINGFHByWALLBUSHINGCode(string wallbushingCode)
        {
            Result res = new Result(true);
            List<WALLBUSHINGFailureHistory> wallbushingFh = new List<WALLBUSHINGFailureHistory>();

            try
            {
                LogHelper.WriteLog("WALLBUSHINGFHController.cs", "GetWALLBUSHINGFHByWALLBUSHINGCode 실행");

                res = wallbushingGojangRepository.GetWALLBUSHINGFHByWALLBUSHINGCode(wallbushingCode, out wallbushingFh);

                if (!res.IsSuccess)
                {
                    LogHelper.WriteLog("WALLBUSHINGFHController.cs", res.Message);
                    return Json(new { success = false, message = res.Message });
                }

                if (wallbushingFh.Count == 0)
                {
                    LogHelper.WriteLog("WALLBUSHINGFHController.cs", "조회된 데이터가 없습니다.");
                    return Json(new { success = true, data = new List<WALLBUSHINGFailureHistory>() });
                }

                LogHelper.WriteLog("WALLBUSHINGFHController.cs", $"조회된 데이터: {wallbushingFh.Count}건");

                return Json(new { success = true, data = wallbushingFh });
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("WALLBUSHINGFHController.cs", "오류 발생: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalWALLBUSHINGGojangListData()
        {
            try
            {
                LogHelper.WriteLog("TotalWALLBUSHINGGojangController.List", "GetTotalWALLBUSHINGGojangListData 실행");

                List<WALLBUSHINGFailureHistory> wallbushingGojang;
                var repoResult = wallbushingGojangRepository.GetTotalWALLBUSHINGGojang(out wallbushingGojang);
                if (!repoResult.IsSuccess)
                    return Json(new { success = false, message = repoResult.Message });

                wallbushingBasicInfoRepository.GetAllWALLBUSHINGBasicInfoRepo(out var basics);
                var basicMap = basics.ToDictionary(b => b.WALLBUSHING_Code, b => b);

                var formattedData = wallbushingGojang.Select(item =>
                {
                    basicMap.TryGetValue(item.WALLBUSHING_Code, out var basic);
                    return new
                    {
                        item.Tbl_Idx,
                        item.WALLBUSHING_Code,
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

                LogHelper.WriteLog("WALLBUSHINGGojangController.List", $"조회된 데이터: {formattedData.Count}건");
                return Json(formattedData);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("WALLBUSHINGGojangController.List", $"GetTotalWALLBUSHINGGojangListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}