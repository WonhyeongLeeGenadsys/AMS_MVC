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

        [HttpPost]
        public JsonResult GetSPAREDemandForecastData(
            int? assetTypeId = null,
            string criticality = null)
        {
            try
            {
                var decisions = new DmDecisionService().GetDecisions();
                var fullForecast = new SpareDemandForecastService().Calculate(
                    decisions,
                    spareDashboardRepository.GetSpareDemandInputsRepo(null, null));
                SaveCalculatedInventoryPolicies(fullForecast);

                var forecast = !assetTypeId.HasValue && string.IsNullOrWhiteSpace(criticality)
                    ? fullForecast
                    : new SpareDemandForecastService().Calculate(
                        decisions,
                        spareDashboardRepository.GetSpareDemandInputsRepo(
                            assetTypeId,
                            criticality));

                return Json(new
                {
                    success = true,
                    summary = new
                    {
                        forecast.TOTAL_PART_COUNT,
                        forecast.FORECAST_PART_COUNT,
                        forecast.SHORTAGE_PART_COUNT,
                        forecast.TOTAL_RECOMMENDED_QTY,
                        forecast.TOTAL_EXPECTED_DEMAND,
                        forecast.TOTAL_EXPECTED_COST
                    },
                    rows = forecast.Rows
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(
                    "SpareDashboardController",
                    "GetSPAREDemandForecastData Error: " + ex.Message);
                return Json(new
                {
                    success = false,
                    message = "예비품 수요예측 데이터를 계산하는 중 오류가 발생했습니다.",
                    detail = ex.Message
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
                RefreshCalculatedInventoryPolicies();
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
                int baseYear = fiscalYear.GetValueOrDefault(DateTime.Now.Year);
                var decisions = new DmDecisionService().GetDecisions();
                var demandInputs = spareDashboardRepository.GetSpareDemandInputsRepo(
                    assetTypeId,
                    null);
                var plan = new SpareProcurementPlanService().Calculate(
                    decisions,
                    demandInputs,
                    baseYear);
                var assetCostRows = spareDashboardRepository.GetSpareAssetCostDataRepo(
                    assetTypeId);

                return Json(new
                {
                    success = true,
                    assetCostRows,
                    yearlyBudgetRows = plan.YearlyBudgetRows,
                    procurementRows = plan.Rows,
                    summary = new
                    {
                        plan.BASE_YEAR,
                        plan.TOTAL_PART_COUNT,
                        plan.TOTAL_ORDER_QTY,
                        plan.TOTAL_ORDER_COST,
                        plan.EXCLUDED_ASSET_COUNT
                    }
                }, JsonRequestBehavior.AllowGet);
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
                RefreshCalculatedInventoryPolicies();
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

        private void RefreshCalculatedInventoryPolicies()
        {
            var decisions = new DmDecisionService().GetDecisions();
            var forecast = new SpareDemandForecastService().Calculate(
                decisions,
                spareDashboardRepository.GetSpareDemandInputsRepo(null, null));
            SaveCalculatedInventoryPolicies(forecast);
        }

        private void SaveCalculatedInventoryPolicies(SpareDemandForecastResult forecast)
        {
            var policies = new SpareInventoryPolicyService().Calculate(forecast.Rows);
            spareDashboardRepository.SaveCalculatedInventoryPoliciesRepo(policies);
        }
    }
}
