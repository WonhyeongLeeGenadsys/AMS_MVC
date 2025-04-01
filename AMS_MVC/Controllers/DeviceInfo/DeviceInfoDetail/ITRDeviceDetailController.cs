using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers.DeviceInfo.DeviceInfoDetail
{
    public class ITRDeviceDetailController : Controller
    {
        // GET: ITRDeviceDetail
        public ActionResult ITRDeviceDetail()
        {
            ViewBag.MenuType = "DeviceInfo";
            return View("~/Views/Device/ITR/ITRDeviceDetail.cshtml");
        }
    }
}