using System.Collections.Generic;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class SPAREBasicController : Controller
    {
        [HttpGet]
        public ActionResult SPAREBasicDetail(int spareId)
        {
            if (spareId <= 0)
            {
                return RedirectToAction("SPAREBasicList");
            }

            var sparePart = spareBasicRepository.GetSPAREPartBySPAREIdRepo(spareId);
            if (sparePart == null)
            {
                return HttpNotFound("예비품 정보를 찾을 수 없습니다.");
            }

            spareBasicRepository.GetInventoryBySPAREIdRepo(spareId, out var inventory);
            spareBasicRepository.GetAssetTypeIdsBySPAREIdRepo(spareId, out var assetTypeIds);
            spareBasicRepository.GetProcurementBySPAREIdRepo(spareId, out var procurements);
            spareBasicRepository.GetCostManagementBySPAREIdRepo(spareId, out var costs);

            ViewBag.Inventory = inventory ?? new InventoryInfo();
            ViewBag.AssetTypeIds = assetTypeIds ?? new List<int>();
            ViewBag.Procurements = procurements ?? new List<ProcurementInfo>();
            ViewBag.Costs = costs ?? new List<CostManagementInfo>();

            return View("~/Views/SPARE/Basic/SPAREBasicDetail.cshtml", sparePart);
        }

        [HttpPost]
        public JsonResult DeleteSPAREBasicInfo(int spareId)
        {
            if (spareId <= 0)
            {
                return Json(new { success = false, message = "올바른 SPARE_ID가 전달되지 않았습니다." });
            }

            var result = spareBasicRepository.DeleteSPAREBasicInfoRepo(spareId);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "예비품이 미사용 처리되었습니다. 기존 재고·발주·비용 이력은 보존됩니다." });
            }

            return Json(new { success = false, message = "예비품 미사용 처리 실패: " + result.Message });
        }
    }
}
