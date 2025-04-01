using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers.DeviceInfo.DeviceInfoDetail
{
    public class DCCABLEDeviceDetailController : Controller
    {
        // GET: DCCABLEDeviceDetail
        public ActionResult DCCABLEDeviceDetail()
        {
            ViewBag.MenuType = "DeviceInfo";
            return View("~/Views/Device/DCCABLE/DCCABLEDeviceDetail.cshtml");
        }
    }
}