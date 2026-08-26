using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public class SPAREInventoryController : Controller
    {
        private readonly SPAREBasicInfoRepository spareBasicRepository;
        private readonly SPAREDashboardRepository spareDashboardRepository;

        public SPAREInventoryController()
        {
            ViewBag.MenuType = "SPARE";
            spareBasicRepository = new SPAREBasicInfoRepository();
            spareDashboardRepository = new SPAREDashboardRepository();
        }

        [HttpGet]
        public ActionResult SPAREInventoryList()
        {
            return View("~/Views/SPARE/Inventory/SPAREInventoryList.cshtml");
        }

        [HttpPost]
        public ActionResult GetInventoryListData()
        {
            var warning = TryRefreshCalculatedInventoryPolicies();
            var result = spareBasicRepository.GetInventoryListRepo(out var rows);

            return Json(new
            {
                success = result.IsSuccess,
                data = rows,
                message = result.Message,
                warning
            });
        }

        [HttpGet]
        public ActionResult SPAREInventoryEdit(int spareId)
        {
            if (spareId <= 0)
                return RedirectToAction("SPAREInventoryList");

            var sparePart = spareBasicRepository.GetSPAREPartBySPAREIdRepo(spareId);
            if (sparePart == null)
                return HttpNotFound("예비품 정보를 찾을 수 없습니다.");

            ViewBag.InventoryWarning = TryRefreshCalculatedInventoryPolicies();
            spareBasicRepository.GetInventoryBySPAREIdRepo(spareId, out var inventory);

            ViewBag.SparePart = sparePart;
            return View("~/Views/SPARE/Inventory/SPAREInventoryEdit.cshtml", inventory ?? new InventoryInfo { SPARE_ID = spareId });
        }

        [HttpPost]
        public ActionResult SaveInventory(InventoryInfo model)
        {
            if (model.SPARE_ID <= 0)
                return Json(new { success = false, message = "올바른 SPARE_ID가 아닙니다." });

            try
            {
                var result = spareBasicRepository.SaveInventoryRepo(model);

                return Json(new
                {
                    success = result.IsSuccess,
                    message = result.Message
                });
            }
            catch (System.Exception ex)
            {
                LogHelper.WriteLog(
                    "SPAREInventoryController.SaveInventory",
                    ex.Message + " / " + ex.StackTrace);

                return Json(new
                {
                    success = false,
                    message = "재고 저장 중 서버 오류가 발생했습니다."
                });
            }
        }

        private string TryRefreshCalculatedInventoryPolicies()
        {
            try
            {
                RefreshCalculatedInventoryPolicies();
                return null;
            }
            catch (System.Exception ex)
            {
                const string warning = "재고 자동계산을 갱신하지 못해 기존 저장값으로 표시합니다.";
                LogHelper.WriteLog(
                    "SPAREInventoryController.RefreshCalculatedInventoryPolicies",
                    warning + " / " + ex.Message + " / " + ex.StackTrace);
                return warning;
            }
        }

        private void RefreshCalculatedInventoryPolicies()
        {
            var decisions = new DmDecisionService().GetDecisions();
            var forecast = new SpareDemandForecastService().Calculate(
                decisions,
                spareDashboardRepository.GetSpareDemandInputsRepo(null, null));
            var policies = new SpareInventoryPolicyService().Calculate(forecast.Rows);
            spareDashboardRepository.SaveCalculatedInventoryPoliciesRepo(policies);
        }
    }
}
