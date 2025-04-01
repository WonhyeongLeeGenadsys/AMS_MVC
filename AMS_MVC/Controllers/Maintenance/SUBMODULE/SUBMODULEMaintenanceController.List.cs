using AMS_MVC.Models;
using AMS_MVC.Repositories;
using AMS_MVC.Utlity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Maintenance.SUBMODULE
{
    public partial class SUBMODULEMaintenanceController : Controller
    {
        // GET: SUBMODULEMaintenance
        public ActionResult SUBMODULEMaintenanceList(string SUBMODULE_Code)
        {
            var basicInfo = submoduleBasicInfoRepository.GetSUBMODULEBasicInfoByCode(SUBMODULE_Code);
            ViewBag.SUBMODULECode = SUBMODULE_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Maintenance/SUBMODULE/SUBMODULEMaintenanceList.cshtml");
        }

        public ActionResult SUBMODULEMaintenanceTotalList()
        {
            return View("~/Views/Maintenance/Total/SUBMODULEMaintenanceTotalList.cshtml");
        }

        /// <summary>
        /// SUBMODULE 유지보수 데이터 가져오기
        /// </summary>
        [HttpPost]
        public JsonResult GetSUBMODULEMRBySUBMODULECode(string submoduleCode)
        {
            Result res = new Result(true);
            List<SUBMODULEMaintenanceHistory> submoduleMR = new List<SUBMODULEMaintenanceHistory>();

            try
            {
                LogHelper.WriteLog("SUBMODULEMRController.cs", "GetSUBMODULEMRBySUBMODULECode 실행");

                res = submoduleMaintenanceRepository.GetSUBMODULEMRBySUBMODULECode(submoduleCode, out submoduleMR);

                if (!res.IsSuccess)
                {
                    LogHelper.WriteLog("SUBMODULEMRController.cs", res.Message);
                    return Json(new { success = false, message = res.Message });
                }

                if (submoduleMR.Count == 0)
                {
                    LogHelper.WriteLog("SUBMODULEMRController.cs", "조회된 데이터가 없습니다.");
                    return Json(new { success = true, data = new List<SUBMODULEMaintenanceHistory>() });
                }

                LogHelper.WriteLog("SUBMODULEMRController.cs", $"조회된 데이터: {submoduleMR.Count}건");

                return Json(new { success = true, data = submoduleMR });
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("SUBMODULEMRController.cs", "오류 발생: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalSUBMODULEMaintenanceListData()
        {
            try
            {
                LogHelper.WriteLog("TotalSUBMODULEMaintenanceController.List", "GetTotalSUBMODULEMaintenanceListData 실행");

                List<SUBMODULEMaintenanceHistory> submoduleMaintenance = new List<SUBMODULEMaintenanceHistory>();
                var repoResult = submoduleMaintenanceRepository.GetTotalSUBMODULEMaintenance(out submoduleMaintenance);
                if (repoResult.IsSuccess)
                {
                    LogHelper.WriteLog("SUBMODULEMaintenanceController.List", $"조회된 데이터: {submoduleMaintenance.Count}건");
                    return Json(submoduleMaintenance);
                }
                else
                {
                    LogHelper.WriteLog("SUBMODULEMaintenanceController.List", "전체 SUBMODULE 유지보수 데이터 로드 실패");
                    return Json(new { success = false, message = "전체 SUBMODULE 유지보수 데이터 로드 실패" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("SUBMODULEMaintenanceController.List", $"GetTotalSUBMODULEMaintenanceListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}