using AMS_MVC.Models;
using AMS_MVC.Repositories;
using AMS_MVC.Utlity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Maintenance.DCCB
{
    public partial class DCCBMaintenanceController : Controller
    {
        // GET: DCCBMaintenance
        public ActionResult DCCBMaintenanceList(string DCCB_Code)
        {
            var basicInfo = dccbBasicInfoRepository.GetDCCBBasicInfoByCode(DCCB_Code);
            ViewBag.DCCBCode = DCCB_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Maintenance/DCCB/DCCBMaintenanceList.cshtml");
        }

        public ActionResult DCCBMaintenanceTotalList()
        {
            return View("~/Views/Maintenance/Total/DCCBMaintenanceTotalList.cshtml");
        }

        /// <summary>
        /// DCCB 유지보수 데이터 가져오기
        /// </summary>
        [HttpPost]
        public JsonResult GetDCCBMRByDCCBCode(string dccbCode)
        {
            Result res = new Result(true);
            List<DCCBMaintenanceHistory> dccbMR = new List<DCCBMaintenanceHistory>();

            try
            {
                LogHelper.WriteLog("DCCBMRController.cs", "GetDCCBMRByDCCBCode 실행");

                res = dccbMaintenanceRepository.GetDCCBMRByDCCBCode(dccbCode, out dccbMR);

                if (!res.IsSuccess)
                {
                    LogHelper.WriteLog("DCCBMRController.cs", res.Message);
                    return Json(new { success = false, message = res.Message });
                }

                if (dccbMR.Count == 0)
                {
                    LogHelper.WriteLog("DCCBMRController.cs", "조회된 데이터가 없습니다.");
                    return Json(new { success = true, data = new List<DCCBMaintenanceHistory>() });
                }

                LogHelper.WriteLog("DCCBMRController.cs", $"조회된 데이터: {dccbMR.Count}건");

                return Json(new { success = true, data = dccbMR });
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("DCCBMRController.cs", "오류 발생: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalDCCBMaintenanceListData()
        {
            try
            {
                LogHelper.WriteLog("TotalDCCBMaintenanceController.List", "GetTotalDCCBMaintenanceListData 실행");

                List<DCCBMaintenanceHistory> dccbMaintenance = new List<DCCBMaintenanceHistory>();
                var repoResult = dccbMaintenanceRepository.GetTotalDCCBMaintenance(out dccbMaintenance);
                if (repoResult.IsSuccess)
                {
                    var formattedData = dccbMaintenance.Select(item => new
                    {
                        item.Tbl_Idx,
                        item.DCCB_Code,
                        item.MR_Bosu_Name,
                        item.MR_Weather,
                        item.MR_Temp,
                        item.MR_Hum,
                        item.MR_Content,
                        item.MR_Status,
                        item.MR_Part,
                        item.MR_Worker,
                        MR_Date = item.MR_Date?.ToString("yy.MM.dd"),
                        item.MR_Writer,
                    }).ToList();


                    LogHelper.WriteLog("DCCBMaintenanceController.List", $"조회된 데이터: {dccbMaintenance.Count}건");
                    return Json(formattedData);
                }
                else
                {
                    LogHelper.WriteLog("DCCBMaintenanceController.List", "전체 DCCB 유지보수 데이터 로드 실패");
                    return Json(new { success = false, message = "전체 DCCB 유지보수 데이터 로드 실패" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("DCCBMaintenanceController.List", $"GetTotalDCCBMaintenanceListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}