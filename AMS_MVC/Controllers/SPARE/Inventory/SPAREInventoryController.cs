using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public class SPAREInventoryController : Controller
    {
        private readonly SPAREBasicInfoRepository spareBasicRepository;

        public SPAREInventoryController()
        {
            ViewBag.MenuType = "SPARE";
            spareBasicRepository = new SPAREBasicInfoRepository();
        }

        [HttpGet]
        public ActionResult SPAREInventoryList()
        {
            return View("~/Views/SPARE/Inventory/SPAREInventoryList.cshtml");
        }

        [HttpPost]
        public ActionResult GetInventoryListData()
        {
            var result = spareBasicRepository.GetInventoryListRepo(out var rows);

            return Json(new
            {
                success = result.IsSuccess,
                data = rows,
                message = result.Message
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

            spareBasicRepository.GetInventoryBySPAREIdRepo(spareId, out var inventory);

            ViewBag.SparePart = sparePart;
            return View("~/Views/SPARE/Inventory/SPAREInventoryEdit.cshtml", inventory ?? new InventoryInfo { SPARE_ID = spareId });
        }

        [HttpPost]
        public ActionResult SaveInventory(InventoryInfo model)
        {
            if (model.SPARE_ID <= 0)
                return Json(new { success = false, message = "올바른 SPARE_ID가 아닙니다." });

            var result = spareBasicRepository.SaveInventoryRepo(model);

            return Json(new
            {
                success = result.IsSuccess,
                message = result.Message
            });
        }
    }
}