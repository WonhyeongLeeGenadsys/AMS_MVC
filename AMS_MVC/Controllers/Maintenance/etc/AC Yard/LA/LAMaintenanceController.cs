using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class LAMaintenanceController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly MaintenanceRepository maintenanceRepository;
        private readonly LAMaintenanceRepository laMaintenanceRepository;
        private readonly LABasicInfoRepository laBasicInfoRepository;

        public LAMaintenanceController()
        {
            ViewBag.MenuType = "Maintenance";
            companyRepository = new CompanyRepository();
            maintenanceRepository = new MaintenanceRepository();
            laMaintenanceRepository = new LAMaintenanceRepository();
            laBasicInfoRepository = new LABasicInfoRepository();
        }
    }
}