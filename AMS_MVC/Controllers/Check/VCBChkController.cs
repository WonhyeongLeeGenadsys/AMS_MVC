using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers.Check
{
    public class VCBChkController : Controller
    {
        // GET: VCBChk
        public ActionResult VCBChkList()
        {
            ViewBag.MenuType = "Regist"; // TopMenu 등록 Event 활성화
            return View("~/Views/Check/VCB/VCBChkList.cshtml");
        }
    }
}