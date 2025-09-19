
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class TotalInfoController : Controller
    {
        [HttpGet]
        public ActionResult PofTab()
        {
            ViewBag.MenuType = "TotalInfo";
            return View("~/Views/TotalInfo/PofTab.cshtml");
        }
    }
}