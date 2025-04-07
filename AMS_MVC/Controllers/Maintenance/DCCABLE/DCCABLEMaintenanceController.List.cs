using AMS_MVC.Models;
using AMS_MVC.Repositories;
using AMS_MVC.Utlity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Maintenance.DCCABLE
{
    public partial class DCCABLEMaintenanceController : Controller
    {
        // GET: DCCABLEMaintenance
        public ActionResult DCCABLEMaintenanceList(string DCCABLE_Code)
        {
            var basicInfo = dccableBasicInfoRepository.GetDCCABLEBasicInfoByCode(DCCABLE_Code);
            ViewBag.DCCABLECode = DCCABLE_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Maintenance/DCCABLE/DCCABLEMaintenanceList.cshtml");
        }

        public ActionResult DCCABLEMaintenanceTotalList()
        {
            return View("~/Views/Maintenance/Total/DCCABLEMaintenanceTotalList.cshtml");
        }

        /// <summary>
        /// DCCABLE 유지보수 데이터 가져오기
        /// </summary>
        [HttpPost]
        public JsonResult GetDCCABLEMRByDCCABLECode(string dccableCode)
        {
            Result res = new Result(true);
            List<DCCABLEMaintenanceHistory> dccableMR = new List<DCCABLEMaintenanceHistory>();

            try
            {
                LogHelper.WriteLog("DCCABLEMRController.cs", "GetDCCABLEMRByDCCABLECode 실행");

                res = dccableMaintenanceRepository.GetDCCABLEMRByDCCABLECode(dccableCode, out dccableMR);

                if (!res.IsSuccess)
                {
                    LogHelper.WriteLog("DCCABLEMRController.cs", res.Message);
                    return Json(new { success = false, message = res.Message });
                }

                if (dccableMR.Count == 0)
                {
                    LogHelper.WriteLog("DCCABLEMRController.cs", "조회된 데이터가 없습니다.");
                    return Json(new { success = true, data = new List<DCCABLEMaintenanceHistory>() });
                }

                LogHelper.WriteLog("DCCABLEMRController.cs", $"조회된 데이터: {dccableMR.Count}건");

                return Json(new { success = true, data = dccableMR });
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("DCCABLEMRController.cs", "오류 발생: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalDCCABLEMaintenanceListData()
        {
            try
            {
                LogHelper.WriteLog("TotalDCCABLEMaintenanceController.List", "GetTotalDCCABLEMaintenanceListData 실행");

                List<DCCABLEMaintenanceHistory> dccableMaintenance = new List<DCCABLEMaintenanceHistory>();
                var repoResult = dccableMaintenanceRepository.GetTotalDCCABLEMaintenance(out dccableMaintenance);
                if (repoResult.IsSuccess)
                {
                    var formattedData = dccableMaintenance.Select(item => new
                    {
                        item.Tbl_Idx,
                        item.DCCABLE_Code,
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

                    LogHelper.WriteLog("DCCABLEMaintenanceController.List", $"조회된 데이터: {dccableMaintenance.Count}건");
                    return Json(formattedData);
                }
                else
                {
                    LogHelper.WriteLog("DCCABLEMaintenanceController.List", "전체 DCCABLE 유지보수 데이터 로드 실패");
                    return Json(new { success = false, message = "전체 DCCABLE 유지보수 데이터 로드 실패" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("DCCABLEMaintenanceController.List", $"GetTotalDCCABLEMaintenanceListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}