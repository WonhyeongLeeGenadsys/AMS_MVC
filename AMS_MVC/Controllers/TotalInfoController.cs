using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers
{
    public class TotalInfoController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "종합정보";
            return View();
        }

        public ActionResult Battery()
        {
            return View();
        }

        public ActionResult Connection()
        {
            return View();
        }

        public ActionResult Maintenance()
        {
            return View();
        }
    }
}