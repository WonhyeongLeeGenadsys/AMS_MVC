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

        // GET: Setting/CoFInfo
        public ActionResult CofInfo(string code = "VCB")
        {
            ViewBag.MenuType = "Setting";

            // 1) 장비 리스트
            var equipmentTypes = new[] { "VCB", "ITR", "DCCB", "DCCABLE", "SUBMODULE" };
            ViewBag.EquipmentTypes = new SelectList(equipmentTypes, code);

            // 2) 선택된 코드에 대한 최신값 가져오기
            var model = cofRepo.GetLatest(code);
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CofInfo(COFModel model)
        {
            ViewBag.MenuType = "Setting";

            // 1) 장비 리스트, 선택된 model.Code 가 역시 드롭다운에 걸리도록
            var equipmentTypes = new[] { "VCB", "ITR", "DCCB", "DCCABLE", "SUBMODULE" };
            ViewBag.EquipmentTypes = new SelectList(equipmentTypes, model.Code);

            if (!ModelState.IsValid)
                return View(model);

            // 2) CoF 계산
            calculator.Calculate(model);

            // 3) 하루에 한 번만 UPDATE, 아니면 INSERT
            cofRepo.SaveOrUpdate(model);

            var riskRepo = new RiskmatrixRepository();
            riskRepo.UpdateCoFByPrefix(model.Code, model.Total_Cof);

            ModelState.Clear();

            // 4) 다시 최신값 조회 (오늘 업데이트된 or 새로 INSERT 된)
            var latest = cofRepo.GetLatest(model.Code);
            return View(latest);
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
