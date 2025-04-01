using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers
{
    public class SettingController : Controller
    {
        // GET: Setting
        public ActionResult SubstationInfo()
        {
            ViewBag.MenuType = "Setting"; // TopMenu 등록 Event 활성화
            return View("~/Views/Setting/SubstationInfo.cshtml");
        }

        public ActionResult MemberInfo()
        {
            ViewBag.MenuType = "Setting"; // TopMenu 등록 Event 활성화
            return View("~/Views/Setting/MemberInfo.cshtml");
        }
    }
}