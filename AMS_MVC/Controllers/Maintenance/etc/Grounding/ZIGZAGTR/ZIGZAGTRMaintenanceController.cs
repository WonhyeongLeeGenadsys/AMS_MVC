using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class ZIGZAGTRMaintenanceController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly MaintenanceRepository maintenanceRepository;
        private readonly ZIGZAGTRMaintenanceRepository zigzagtrMaintenanceRepository;
        private readonly ZIGZAGTRBasicInfoRepository zigzagtrBasicInfoRepository;

        public ZIGZAGTRMaintenanceController()
        {
            ViewBag.MenuType = "Maintenance";
            companyRepository = new CompanyRepository();
            maintenanceRepository = new MaintenanceRepository();
            zigzagtrMaintenanceRepository = new ZIGZAGTRMaintenanceRepository();
            zigzagtrBasicInfoRepository = new ZIGZAGTRBasicInfoRepository();
        }
    }
}