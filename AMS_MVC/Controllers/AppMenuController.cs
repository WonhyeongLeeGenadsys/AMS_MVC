using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers
{
    public class AppMenuController : Controller
    {
        // GET: AppMenu
        public ActionResult Index()
        {
            ViewBag.MenuType = "AppMenu"; // LeftMenu에 DeviceInfoMenu 표시
            ViewData["Title"] = "등록";
            return View();
        }
    }
}