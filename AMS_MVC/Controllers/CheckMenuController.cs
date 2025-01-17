using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers
{
    public class CheckMenuController : Controller
    {
        // GET: Check
        public ActionResult Index()
        {
            ViewBag.MenuType = "CheckMenu"; 
            ViewData["Title"] = "점검";
            return View();
        }
    }
}