using System.Collections.Generic;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class SPAREBasicController : Controller
    {
        [HttpGet]
        public ActionResult UpdateSPAREBasic(int spareId)
        {
            var sparePart = spareBasicRepository.GetSPAREPartBySPAREIdRepo(spareId);
            if (sparePart == null)
            {
                return HttpNotFound("예비품 정보를 찾을 수 없습니다.");
            }

            spareBasicRepository.GetInventoryBySPAREIdRepo(spareId, out var inventory);
            spareBasicRepository.GetAssetTypeIdsBySPAREIdRepo(spareId, out var assetTypeIds);

            ViewBag.Inventory = inventory ?? new InventoryInfo();
            ViewBag.AssetTypeIds = assetTypeIds ?? new List<int>();

            return View("~/Views/SPARE/SPAREBasicUpdate.cshtml", sparePart);
        }

        [HttpPost]
        public ActionResult UpdateSPAREBasicInfo(
            SPAREPartInfo model,
            int current_qty = 0,
            int safety_stock = 0,
            int? eoq = null,
            int? reorder_point = null,
            List<int> asset_type_ids = null)
        {
            if (model.SPARE_ID <= 0)
                return Json(new { success = false, message = "올바른 예비품 ID가 아닙니다." });

            if (string.IsNullOrWhiteSpace(model.PART_NUMBER))
                return Json(new { success = false, message = "부품번호를 입력하세요." });

            if (string.IsNullOrWhiteSpace(model.PART_NAME))
                return Json(new { success = false, message = "부품명을 입력하세요." });

            var inventory = new InventoryInfo
            {
                SPARE_ID = model.SPARE_ID,
                CURRENT_QTY = current_qty,
                SAFETY_STOCK = safety_stock,
                EOQ = eoq,
                REORDER_POINT = reorder_point
            };

            var result = spareBasicRepository.UpdateSPAREBasicInfoRepo(
                model,
                inventory,
                asset_type_ids ?? new List<int>());

            return Json(new
            {
                success = result.IsSuccess,
                message = result.Message
            });
        }
    }
}