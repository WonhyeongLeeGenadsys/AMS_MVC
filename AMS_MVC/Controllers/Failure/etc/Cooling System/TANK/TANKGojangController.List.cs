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
    public partial class TANKGojangController : Controller
    {
        // GET: TANKGojang
        public ActionResult TANKGojangList(string TANK_Code)
        {
            var basicInfo = tankBasicInfoRepository.GetTANKBasicInfoByCode(TANK_Code);
            ViewBag.TANKCode = TANK_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Gojang/etc/Cooling System/TANK/TANKGojangList.cshtml");
        }

        public ActionResult TANKGojangTotalList()
        {
            return View("~/Views/Gojang/Total/etc/Cooling System/TANKGojangTotalList.cshtml");
        }

        /// <summary>
        /// TANK 고장 이력 데이터 가져오기
        /// </summary>
        [HttpPost]
        public JsonResult GetTANKFHByTANKCode(string tankCode)
        {
            Result res = new Result(true);
            List<TANKFailureHistory> tankFh = new List<TANKFailureHistory>();

            try
            {
                LogHelper.WriteLog("TANKFHController.cs", "GetTANKFHByTANKCode 실행");

                res = tankGojangRepository.GetTANKFHByTANKCode(tankCode, out tankFh);

                if (!res.IsSuccess)
                {
                    LogHelper.WriteLog("TANKFHController.cs", res.Message);
                    return Json(new { success = false, message = res.Message });
                }

                if (tankFh.Count == 0)
                {
                    LogHelper.WriteLog("TANKFHController.cs", "조회된 데이터가 없습니다.");
                    return Json(new { success = true, data = new List<TANKFailureHistory>() });
                }

                LogHelper.WriteLog("TANKFHController.cs", $"조회된 데이터: {tankFh.Count}건");

                return Json(new { success = true, data = tankFh });
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("TANKFHController.cs", "오류 발생: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalTANKGojangListData()
        {
            try
            {
                LogHelper.WriteLog("TotalTANKGojangController.List", "GetTotalTANKGojangListData 실행");

                List<TANKFailureHistory> tankGojang;
                var repoResult = tankGojangRepository.GetTotalTANKGojang(out tankGojang);
                if (!repoResult.IsSuccess)
                    return Json(new { success = false, message = repoResult.Message });

                tankBasicInfoRepository.GetAllTANKBasicInfoRepo(out var basics);
                var basicMap = basics.ToDictionary(b => b.TANK_Code, b => b);

                var formattedData = tankGojang.Select(item =>
                {
                    basicMap.TryGetValue(item.TANK_Code, out var basic);
                    return new
                    {
                        item.Tbl_Idx,
                        item.TANK_Code,
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

                LogHelper.WriteLog("TANKGojangController.List", $"조회된 데이터: {formattedData.Count}건");
                return Json(formattedData);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("TANKGojangController.List", $"GetTotalTANKGojangListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}