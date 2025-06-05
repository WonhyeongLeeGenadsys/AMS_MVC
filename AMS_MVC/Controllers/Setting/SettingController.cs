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

        public ActionResult CoFInfo()
        {
            ViewBag.MenuType = "Setting";
            return View("~/Views/Setting/CoFInfo.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CoFInfo(COFModel model)
        {
            ViewBag.MenuType = "Setting";

            if (!ModelState.IsValid)
            {
                // 유효성 검사 실패 시, 그냥 원래 View로 
                return View("~/Views/Setting/CoFInfo.cshtml", model);
            }

            // 1) 입력값에 대한 계산 수행
            calculator.Calculate(model);

            // 2) DB에 저장
            cofRepo.Insert(model);

            // 3) 다시 최신값(방금 저장된 값)을 받아서 View에 넘김
            var latest = cofRepo.GetLatest();
            return View("~/Views/Setting/CoFInfo.cshtml", latest);
        }
    }
}
