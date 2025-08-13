using AMS_MVC.Models;
using AMS_MVC.Repositories;
using AMS_MVC.Services;
using System.Linq;
using System.Web.Mvc;

namespace AMS_MVC.Controllers
{
    public class SettingController : Controller
    {
        private EquipmentWeibullRepository weibullRepo = new EquipmentWeibullRepository();
        private readonly CoFRepository cofRepo = new CoFRepository();
        private readonly CoFCalculator calculator = new CoFCalculator();

        // GET: Setting/SubstationInfo
        public ActionResult SubstationInfo()
        {
            ViewBag.MenuType = "Setting";

            var eqList = weibullRepo.GetAll();

            var vcb = eqList.FirstOrDefault(x => x.EquipmentName.ToUpper() == "VCB");
            var itr = eqList.FirstOrDefault(x => x.EquipmentName.ToUpper() == "ITR");
            var submodule = eqList.FirstOrDefault(x => x.EquipmentName.ToUpper() == "SUBMODULE");
            var dccb = eqList.FirstOrDefault(x => x.EquipmentName.ToUpper() == "DCCB");
            var dccable = eqList.FirstOrDefault(x => x.EquipmentName.ToUpper() == "DCCABLE");

            ViewBag.VCB = vcb;        
            ViewBag.ITR = itr;
            ViewBag.SUBMODULE = submodule;
            ViewBag.DCCB = dccb;
            ViewBag.DCCABLE = dccable;

            return View("~/Views/Setting/SubstationInfo.cshtml");
        }

        public ActionResult MemberInfo()
        {
            ViewBag.MenuType = "Setting";
            return View("~/Views/Setting/MemberInfo.cshtml");
        }

        public ActionResult CofInfo(string code = "VCB")
        {
            ViewBag.MenuType = "Setting";

            // 모델 먼저 준비
            var model = cofRepo.GetLatest(code) ?? new COFModel { Code = code };

            // 드롭다운 아이템 + 현재 선택값 = model.Code
            var equipmentTypes = new[] { "VCB", "ITR", "DCCB", "DCCABLE", "SUBMODULE" };
            ViewBag.EquipmentTypes = new SelectList(equipmentTypes, model.Code);

            return View(model);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CofInfo(COFModel model)
        {
            ViewBag.MenuType = "Setting";
            if (!ModelState.IsValid) return View(model);

            calculator.Calculate(model);

            int affected = (model.Tbl_Idx == 0)
                ? cofRepo.Insert(model)           // 처음 저장
                : cofRepo.UpdateById(model);      // 기존 행 변경

            TempData["SaveInfo"] = (model.Tbl_Idx == 0) ? "저장 완료" : "변경 완료";
            return RedirectToAction(nameof(CofInfo), new { code = model.Code }); // PRG
        }


        [HttpGet]
        public JsonResult GetCofData(string code)
        {
            // 오늘자 최신 모델 또는 새 빈 모델
            var model = cofRepo.GetLatest(code) ?? new COFModel { Code = code };
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}
