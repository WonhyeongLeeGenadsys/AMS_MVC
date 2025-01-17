using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers
{
    public class DeviceInfoController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.MenuType = "DeviceInfo"; 
            ViewData["Title"] = "정보";
            return View();
        }
    }
}