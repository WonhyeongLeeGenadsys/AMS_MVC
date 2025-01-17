using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers
{
    public class OptionMenuController : Controller
    {
        // GET: OptionMenu
        public ActionResult Index()
        {
            ViewBag.MenuType = "OptionMenu"; 
            ViewData["Title"] = "환경설정";
            return View();
        }
    }
}