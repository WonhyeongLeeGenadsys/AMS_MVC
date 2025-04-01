using AMS_MVC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers
{
    public class ITRDeviceController : Controller
    {
    
        private readonly RiskmatrixRepository _riskmatrixRepo = new RiskmatrixRepository();
        private readonly PriorityInfoRepository _priorityRepo = new PriorityInfoRepository();
        private readonly MaintenanceRepository _maintenanceRepo = new MaintenanceRepository();
        private readonly GojangRepository _gojangRepo = new GojangRepository();

        // VCBDeviceInfo 페이지
        public ActionResult Index()
        {
            ViewBag.MenuType = "DeviceInfo";
            return View("~/Views/Device/ITR/ITRDevice.cshtml");
        }

        /// <summary>
        /// Riskmatrix 데이터 가져오기
        /// </summary>
        public JsonResult GetRiskmatrixData(string prefix)
        {
            try
            {
                var riskData = _riskmatrixRepo.GetRiskMatrixPofCof(prefix);
                return Json(riskData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Riskmatrix PoF, CoF 데이터 가져오기
        /// </summary>
        public JsonResult GetRiskMatrixPofCof(string prefix)
        {
            var PofCof = _riskmatrixRepo.GetRiskMatrixPofCof(prefix);
            return Json(PofCof, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 우선순위 데이터 가져오기
        /// </summary>
        public JsonResult GetPriorityITR()
        {
            try
            {
                var priorityData = _priorityRepo.GetPriority("INTERFACETR_BASICINFO", "ITR_CODE", "Interface TR", "I");
                return Json(priorityData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 유지보수 한 달 간격 데이터 가져오기
        /// </summary>
        public JsonResult GetMonthlyMaintenanceData()
        {
            try
            {
                var data = _maintenanceRepo.GetMonthlyMaintenanceCounts("INTERFACETR_MAINTENANCE_HISTORY", "Interface TR");
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// VCB 단독 - 고장 테이블 정보 가져오기
        /// </summary>
        public JsonResult GetGojangITRList()
        {
            try
            {
                var gojangData = _gojangRepo.GetGojangData("INTERFACETR_FAILURE_HISTORY", "INTERFACETR_BASICINFO", "ITR_CODE", "I", "Interface TR");
                return Json(gojangData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}