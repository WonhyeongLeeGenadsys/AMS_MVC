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
    public partial class SUBMODULEGojangController : Controller
    {
        // GET: SUBMODULEGojang
        public ActionResult SUBMODULEGojangList(string SUBMODULE_Code)
        {
            var basicInfo = submoduleBasicInfoRepository.GetSUBMODULEBasicInfoByCode(SUBMODULE_Code);
            ViewBag.SUBMODULECode = SUBMODULE_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Gojang/SUBMODULE/SUBMODULEGojangList.cshtml");
        }

        public ActionResult SUBMODULEGojangTotalList()
        {
            return View("~/Views/Gojang/Total/SUBMODULEGojangTotalList.cshtml");
        }

        /// <summary>
        /// SUBMODULE 고장 이력 데이터 가져오기
        /// </summary>
        [HttpPost]
        public JsonResult GetSUBMODULEFHBySUBMODULECode(string submoduleCode)
        {
            Result res = new Result(true);
            List<SUBMODULEFailureHistory> submoduleFh = new List<SUBMODULEFailureHistory>();

            try
            {
                LogHelper.WriteLog("SUBMODULEFHController.cs", "GetSUBMODULEFHBySUBMODULECode 실행");

                res = submoduleGojangRepository.GetSUBMODULEFHBySUBMODULECode(submoduleCode, out submoduleFh);

                if (!res.IsSuccess)
                {
                    LogHelper.WriteLog("SUBMODULEFHController.cs", res.Message);
                    return Json(new { success = false, message = res.Message });
                }

                if (submoduleFh.Count == 0)
                {
                    LogHelper.WriteLog("SUBMODULEFHController.cs", "조회된 데이터가 없습니다.");
                    return Json(new { success = true, data = new List<SUBMODULEFailureHistory>() });
                }

                LogHelper.WriteLog("SUBMODULEFHController.cs", $"조회된 데이터: {submoduleFh.Count}건");

                return Json(new { success = true, data = submoduleFh });
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("SUBMODULEFHController.cs", "오류 발생: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalSUBMODULEGojangListData()
        {
            try
            {
                LogHelper.WriteLog("TotalSUBMODULEGojangController.List", "GetTotalSUBMODULEGojangListData 실행");

                List<SUBMODULEFailureHistory> submoduleGojang = new List<SUBMODULEFailureHistory>();
                var repoResult = submoduleGojangRepository.GetTotalSUBMODULEGojang(out submoduleGojang);
                if (repoResult.IsSuccess)
                {
                    var formattedData = submoduleGojang.Select(item => new
                    {
                        item.Tbl_Idx,
                        item.SUBMODULE_Code,
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
                        item.Fail_Writer,

                    }).ToList();

                    LogHelper.WriteLog("SUBMODULEGojangController.List", $"조회된 데이터: {submoduleGojang.Count}건");
                    return Json(formattedData);
                }
                else
                {
                    LogHelper.WriteLog("SUBMODULEGojangController.List", "전체 SUBMODULE 고장이력 데이터 로드 실패");
                    return Json(new { success = false, message = "전체 SUBMODULE 고장이력 데이터 로드 실패" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("SUBMODULEGojangController.List", $"GetTotalSUBMODULEGojangListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}