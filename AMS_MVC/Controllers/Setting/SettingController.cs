using AMS_MVC.Repositories;
using System.Linq;
using System.Web.Mvc;

namespace AMS_MVC.Controllers
{
    public class SettingController : Controller
    {
        private EquipmentWeibullRepository _weibullRepo = new EquipmentWeibullRepository();

        // GET: Setting/SubstationInfo
        public ActionResult SubstationInfo()
        {
            ViewBag.MenuType = "Setting";

            var eqList = _weibullRepo.GetAll();

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
    }
}
