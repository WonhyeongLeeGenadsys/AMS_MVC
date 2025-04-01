using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers.DeviceInfo.DeviceInfoDetail
{
    public class DCCBDeviceDetailController : Controller
    {
        // GET: DCCBDeviceDetail
        public ActionResult DCCBDeviceDetail()
        {
            ViewBag.MenuType = "DeviceInfo";
            return View("~/Views/Device/DCCB/DCCBDeviceDetail.cshtml");
        }
    }
}