using System.Collections.Generic;
using System.Web.Mvc;
using Web.Common;
using System.Linq;

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
            spareBasicRepository.GetAssetMapsBySPAREIdRepo(spareId, out var assetMaps);

            ViewBag.Inventory = inventory ?? new InventoryInfo();
            ViewBag.AssetMaps = assetMaps ?? new List<SpareAssetMapInfo>();

            return View("~/Views/SPARE/Basic/SPAREBasicUpdate.cshtml", sparePart);
        }

        [HttpPost]
        public ActionResult UpdateSPAREBasicInfo(
            SPAREPartInfo model,
            List<int> asset_type_ids = null,
            List<int> required_qtys = null)
        {
            if (model == null)
                return Json(new { success = false, message = "수정할 예비품 정보가 없습니다." });

            if (model.SPARE_ID <= 0)
                return Json(new { success = false, message = "올바른 예비품 ID가 아닙니다." });

            if (string.IsNullOrWhiteSpace(model.PART_NUMBER))
                return Json(new { success = false, message = "부품번호를 입력하세요." });

            if (string.IsNullOrWhiteSpace(model.PART_NAME))
                return Json(new { success = false, message = "부품명을 입력하세요." });

            if (string.IsNullOrWhiteSpace(model.CRITICALITY_GRADE) ||
                !new[] { "CRITICAL", "HIGH", "MEDIUM", "LOW" }.Contains(model.CRITICALITY_GRADE.Trim().ToUpperInvariant()))
                return Json(new { success = false, message = "중요도는 CRITICAL/HIGH/MEDIUM/LOW 중에서 선택하세요." });

            if (!model.UNIT_PRICE.HasValue || model.UNIT_PRICE.Value < 0)
                return Json(new { success = false, message = "단가는 0 이상으로 입력하세요." });

            if (!model.LEAD_TIME_DAYS.HasValue || model.LEAD_TIME_DAYS.Value < 0)
                return Json(new { success = false, message = "납기일은 0일 이상으로 입력하세요." });

            if (!TryBuildAssetMaps(
                asset_type_ids,
                required_qtys,
                out var assetMaps,
                out var assetMapError))
                return Json(new { success = false, message = assetMapError });

            model.PART_NUMBER = model.PART_NUMBER.Trim();
            model.PART_NAME = model.PART_NAME.Trim();
            model.CRITICALITY_GRADE = model.CRITICALITY_GRADE.Trim().ToUpperInvariant();
            model.SUPPLIER = string.IsNullOrWhiteSpace(model.SUPPLIER) ? null : model.SUPPLIER.Trim();
            model.NOTES = string.IsNullOrWhiteSpace(model.NOTES) ? null : model.NOTES.Trim();
            model.IS_ACTIVE = model.IS_ACTIVE ?? true;

            var result = spareBasicRepository.UpdateSPAREBasicInfoRepo(
                model,
                assetMaps);

            return Json(new
            {
                success = result.IsSuccess,
                message = result.Message
            });
        }
    }
}
