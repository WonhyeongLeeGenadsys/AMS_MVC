using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers.DeviceInfo.DeviceInfoDetail
{
    public class SUBMODULEDeviceDetailController : Controller
    {
        // GET: SUBMODULEDeviceDetail
        public ActionResult SUBMODULEDeviceDetail()
        {
            ViewBag.MenuType = "DeviceInfo";
            return View("~/Views/Device/SUBMODULE/SUBMODULEDeviceDetail.cshtml");
        }
    }
}