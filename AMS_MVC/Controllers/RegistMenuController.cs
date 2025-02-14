using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers
{
    public class RegistMenuController : Controller
    {
        // GET: AppMenu
        public ActionResult Index()
        {
            ViewBag.MenuType = "RegistMenu"; // LeftMenu에 RegistMenu 표시
            ViewData["Title"] = "등록";
            return View();
        }
    }
}