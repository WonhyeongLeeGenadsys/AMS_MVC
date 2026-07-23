using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public class SPARECostController : Controller
    {
        private readonly SPAREBasicInfoRepository spareBasicRepository;

        public SPARECostController()
        {
            ViewBag.MenuType = "SPARE";
            spareBasicRepository = new SPAREBasicInfoRepository();
        }

        [HttpGet]
        public ActionResult SPARECostList()
        {
            return View("~/Views/SPARE/Cost/SPARECostList.cshtml");
        }

        [HttpPost]
        public ActionResult GetCostListData()
        {
            var result = spareBasicRepository.GetCostListRepo(out var rows);

            return Json(new
            {
                success = result.IsSuccess,
                data = rows,
                message = result.Message
            });
        }

        [HttpGet]
        public ActionResult SPARECostAdd(int spareId = 0)
        {
            if (spareId <= 0)
                return RedirectToAction("SPAREBasicList", "SPAREBasic");

            var sparePart = spareBasicRepository.GetSPAREPartBySPAREIdRepo(spareId);
            if (sparePart == null)
                return HttpNotFound("예비품 정보를 찾을 수 없습니다.");

            ViewBag.SparePart = sparePart;
            return View("~/Views/SPARE/Cost/SPARECostAdd.cshtml", new CostManagementInfo { SPARE_ID = spareId });
        }

        [HttpPost]
        public ActionResult SaveCost(CostManagementInfo model)
        {
            if (model.SPARE_ID <= 0)
                return Json(new { success = false, message = "올바른 SPARE_ID가 아닙니다." });

            var sparePart = spareBasicRepository.GetSPAREPartBySPAREIdRepo(model.SPARE_ID);
            if (sparePart == null)
                return Json(new { success = false, message = "비용계획을 등록할 예비품을 찾을 수 없습니다." });

            if (sparePart.IS_ACTIVE == false)
                return Json(new { success = false, message = "미사용 예비품에는 비용계획을 등록할 수 없습니다." });

            var result = spareBasicRepository.SaveCostRepo(model);

            return Json(new
            {
                success = result.IsSuccess,
                message = result.Message
            });
        }

        [HttpGet]
        public ActionResult SPARECostUpdate(int costId)
        {
            var model = spareBasicRepository.GetCostByIdRepo(costId);
            if (model == null)
                return HttpNotFound("비용계획 정보를 찾을 수 없습니다.");

            return View("~/Views/SPARE/Cost/SPARECostUpdate.cshtml", model);
        }

        [HttpPost]
        public ActionResult UpdateCost(CostManagementInfo model)
        {
            if (model.COST_ID <= 0)
                return Json(new { success = false, message = "올바른 COST_ID가 아닙니다." });

            var result = spareBasicRepository.UpdateCostRepo(model);

            return Json(new
            {
                success = result.IsSuccess,
                message = result.Message
            });
        }
    }
}
