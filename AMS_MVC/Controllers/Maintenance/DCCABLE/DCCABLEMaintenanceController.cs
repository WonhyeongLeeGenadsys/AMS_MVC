using AMS_MVC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers.Maintenance.DCCABLE
{
    public partial class DCCABLEMaintenanceController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly MaintenanceRepository maintenanceRepository;
        private readonly DCCABLEMaintenanceRepository dccableMaintenanceRepository;
        private readonly DCCABLEBasicInfoRepository dccableBasicInfoRepository;

        public DCCABLEMaintenanceController()
        {
            ViewBag.MenuType = "Maintenance";
            companyRepository = new CompanyRepository();
            maintenanceRepository = new MaintenanceRepository();
            dccableMaintenanceRepository = new DCCABLEMaintenanceRepository();
            dccableBasicInfoRepository = new DCCABLEBasicInfoRepository();
        }
    }
}