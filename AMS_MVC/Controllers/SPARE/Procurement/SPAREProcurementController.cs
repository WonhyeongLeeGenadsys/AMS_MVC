using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public class SPAREProcurementController : Controller
    {
        private readonly SPAREBasicInfoRepository spareBasicRepository;

        public SPAREProcurementController()
        {
            ViewBag.MenuType = "SPARE";
            spareBasicRepository = new SPAREBasicInfoRepository();
        }

        [HttpGet]
        public ActionResult SPAREProcurementList()
        {
            return View("~/Views/SPARE/Procurement/SPAREProcurementList.cshtml");
        }

        [HttpPost]
        public ActionResult GetProcurementListData()
        {
            var result = spareBasicRepository.GetProcurementListRepo(out var rows);

            return Json(new
            {
                success = result.IsSuccess,
                data = rows,
                message = result.Message
            });
        }

        [HttpGet]
        public ActionResult SPAREProcurementAdd(int spareId = 0)
        {
            if (spareId <= 0)
                return RedirectToAction("SPAREBasicList", "SPAREBasic");

            var sparePart = spareBasicRepository.GetSPAREPartBySPAREIdRepo(spareId);
            if (sparePart == null)
                return HttpNotFound("예비품 정보를 찾을 수 없습니다.");

            ViewBag.SparePart = sparePart;
            return View("~/Views/SPARE/Procurement/SPAREProcurementAdd.cshtml", new ProcurementInfo { SPARE_ID = spareId });
        }

        [HttpPost]
        public ActionResult SaveProcurement(ProcurementInfo model)
        {
            if (model.SPARE_ID <= 0)
                return Json(new { success = false, message = "올바른 SPARE_ID가 아닙니다." });

            var sparePart = spareBasicRepository.GetSPAREPartBySPAREIdRepo(model.SPARE_ID);
            if (sparePart == null)
                return Json(new { success = false, message = "발주를 등록할 예비품을 찾을 수 없습니다." });

            if (sparePart.IS_ACTIVE == false)
                return Json(new { success = false, message = "미사용 예비품에는 발주를 등록할 수 없습니다." });

            var result = spareBasicRepository.SaveProcurementRepo(model);

            return Json(new
            {
                success = result.IsSuccess,
                message = result.Message
            });
        }

        [HttpGet]
        public ActionResult SPAREProcurementUpdate(int procId)
        {
            var model = spareBasicRepository.GetProcurementByIdRepo(procId);
            if (model == null)
                return HttpNotFound("발주 정보를 찾을 수 없습니다.");

            return View("~/Views/SPARE/Procurement/SPAREProcurementUpdate.cshtml", model);
        }

        [HttpPost]
        public ActionResult UpdateProcurement(ProcurementInfo model)
        {
            if (model.PROC_ID <= 0)
                return Json(new { success = false, message = "올바른 PROC_ID가 아닙니다." });

            var result = spareBasicRepository.UpdateProcurementRepo(model);

            return Json(new
            {
                success = result.IsSuccess,
                message = result.Message
            });
        }
    }
}
