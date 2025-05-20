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
    public partial class ARMREACTORGojangController : Controller
    {
        // GET: ARMREACTORGojang
        public ActionResult ARMREACTORGojangList(string ARMREACTOR_Code)
        {
            var basicInfo = armreactorBasicInfoRepository.GetARMREACTORBasicInfoByCode(ARMREACTOR_Code);
            ViewBag.ARMREACTORCode = ARMREACTOR_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Gojang/etc/AC Yard/ARMREACTOR/ARMREACTORGojangList.cshtml");
        }

        public ActionResult ARMREACTORGojangTotalList()
        {
            return View("~/Views/Gojang/Total/etc/AC Yard/ARMREACTORGojangTotalList.cshtml");
        }

        /// <summary>
        /// ARMREACTOR 고장 이력 데이터 가져오기
        /// </summary>
        [HttpPost]
        public JsonResult GetARMREACTORFHByARMREACTORCode(string armreactorCode)
        {
            Result res = new Result(true);
            List<ARMREACTORFailureHistory> armreactorFh = new List<ARMREACTORFailureHistory>();

            try
            {
                LogHelper.WriteLog("ARMREACTORFHController.cs", "GetARMREACTORFHByARMREACTORCode 실행");

                res = armreactorGojangRepository.GetARMREACTORFHByARMREACTORCode(armreactorCode, out armreactorFh);

                if (!res.IsSuccess)
                {
                    LogHelper.WriteLog("ARMREACTORFHController.cs", res.Message);
                    return Json(new { success = false, message = res.Message });
                }

                if (armreactorFh.Count == 0)
                {
                    LogHelper.WriteLog("ARMREACTORFHController.cs", "조회된 데이터가 없습니다.");
                    return Json(new { success = true, data = new List<ARMREACTORFailureHistory>() });
                }

                LogHelper.WriteLog("ARMREACTORFHController.cs", $"조회된 데이터: {armreactorFh.Count}건");

                return Json(new { success = true, data = armreactorFh });
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("ARMREACTORFHController.cs", "오류 발생: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalARMREACTORGojangListData()
        {
            try
            {
                LogHelper.WriteLog("TotalARMREACTORGojangController.List", "GetTotalARMREACTORGojangListData 실행");

                List<ARMREACTORFailureHistory> armreactorGojang;
                var repoResult = armreactorGojangRepository.GetTotalARMREACTORGojang(out armreactorGojang);
                if (!repoResult.IsSuccess)
                    return Json(new { success = false, message = repoResult.Message });

                armreactorBasicInfoRepository.GetAllARMREACTORBasicInfoRepo(out var basics);
                var basicMap = basics.ToDictionary(b => b.ARMREACTOR_Code, b => b);

                var formattedData = armreactorGojang.Select(item =>
                {
                    basicMap.TryGetValue(item.ARMREACTOR_Code, out var basic);
                    return new
                    {
                        item.Tbl_Idx,
                        item.ARMREACTOR_Code,
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

                LogHelper.WriteLog("ARMREACTORGojangController.List", $"조회된 데이터: {formattedData.Count}건");
                return Json(formattedData);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("ARMREACTORGojangController.List", $"GetTotalARMREACTORGojangListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}