using AMS_MVC.Models;
using AMS_MVC.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace AMS_MVC.Controllers
{
    public class DCCABLEDeviceController : Controller
    {
        private readonly RiskmatrixRepository _riskmatrixRepo = new RiskmatrixRepository();
        private readonly PriorityInfoRepository _priorityRepo = new PriorityInfoRepository();
        private readonly MaintenanceRepository _maintenanceRepo = new MaintenanceRepository();
        private readonly GojangRepository _gojangRepo = new GojangRepository();

        // DCCABLEDeviceInfo 페이지
        public ActionResult Index()
        {
            ViewBag.MenuType = "DeviceInfo";
            return View("~/Views/Device/DCCABLE/DCCABLEDevice.cshtml");
        }

        /// <summary>
        /// Riskmatrix 데이터 가져오기
        /// </summary>
        public JsonResult GetRiskmatrixData(string prefix)
        {
            try
            {
                var riskData = _riskmatrixRepo.GetRiskMatrixPofCofByVCBCode(prefix);
                return Json(riskData);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Riskmatrix PoF, CoF 데이터 가져오기
        /// </summary>
        public JsonResult GetRiskMatrixPofCof(string prefix)
        {
            var PofCof = _riskmatrixRepo.GetRiskMatrixPofCof(prefix);
            return Json(PofCof);
        }

        /// <summary>
        /// 우선순위 데이터 가져오기
        /// </summary>
        public JsonResult GetPriorityDCCABLE()
        {
            try
            {
                var priorityData = _priorityRepo.GetPriority("DCCABLE_BASICINFO", "DCCABLE_CODE", "DCCABLE", "DCCABLE");
                return Json(priorityData);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        /// <summary>
        /// 유지보수 한 달 간격 데이터 가져오기
        /// </summary>
        public JsonResult GetMonthlyMaintenanceData()
        {
            try
            {
                var data = _maintenanceRepo.GetMonthlyMaintenanceCounts("DCCABLE_MAINTENANCE_HISTORY", "DCCABLE");
                return Json(data);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        /// <summary>
        /// DCCABLE 단독 - 고장 테이블 정보 가져오기
        /// </summary>
        public JsonResult GetGojangDCCABLEList()
        {
            try
            {
                var gojangData = _gojangRepo.GetGojangData("DCCABLE_FAILURE_HISTORY", "DCCABLE_BASICINFO", "DCCABLE_CODE", "DCCABLE", "DCCABLE");
                return Json(gojangData);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
    }
}
