using System;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class SpareDashboardController : Controller
    {
        private readonly SPAREDashboardRepository spareDashboardRepository;

        public SpareDashboardController()
        {
            ViewBag.MenuType = "SPARE";
            spareDashboardRepository = new SPAREDashboardRepository();
        }

        [HttpGet]
        public ActionResult SPAREOverview()
        {
            return View("~/Views/SPARE/SPAREOverview.cshtml");
        }

        [HttpPost]
        public JsonResult GetSPAREOverviewData(int? assetTypeId = null, string criticality = null)
        {
            try
            {
                var result = spareDashboardRepository.GetSpareOverviewDataRepo(assetTypeId, criticality);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    detail = ex.StackTrace
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult SPAREInventoryStatus()
        {
            return View("~/Views/SPARE/Inventory/SPAREInventoryStatus.cshtml");
        }

        [HttpPost]
        public JsonResult GetSPAREInventoryStatusData(int? assetTypeId = null, string criticality = null)
        {
            try
            {
                var result = spareDashboardRepository.GetSpareInventoryStatusDataRepo(assetTypeId, criticality);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    detail = ex.StackTrace
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult SPAREProcurementStatus()
        {
            return View("~/Views/SPARE/Procurement/SPAREProcurementStatus.cshtml");
        }

        [HttpPost]
        public JsonResult GetSPAREProcurementStatusData(int? assetTypeId = null, string status = null)
        {
            try
            {
                var result = spareDashboardRepository.GetSpareProcurementStatusDataRepo(assetTypeId, status);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    detail = ex.StackTrace
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult SPARECostPlan()
        {
            return View("~/Views/SPARE/Cost/SPARECostPlan.cshtml");
        }

        [HttpPost]
        public JsonResult GetSPARECostPlanData(int? assetTypeId = null, int? fiscalYear = null)
        {
            try
            {
                var result = spareDashboardRepository.GetSpareCostPlanDataRepo(assetTypeId, fiscalYear);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    detail = ex.StackTrace
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult SPAREPolicy()
        {
            return View("~/Views/SPARE/Inventory/SPAREPolicy.cshtml");
        }

        [HttpPost]
        public JsonResult GetSPAREPolicyData(int? assetTypeId = null, string policyType = null)
        {
            try
            {
                var result = spareDashboardRepository.GetSparePolicyDataRepo(assetTypeId, policyType);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    detail = ex.StackTrace
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}