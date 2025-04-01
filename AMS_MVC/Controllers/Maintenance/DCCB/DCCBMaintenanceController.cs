using AMS_MVC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers.Maintenance.DCCB
{
    public partial class DCCBMaintenanceController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly MaintenanceRepository maintenanceRepository;
        private readonly DCCBMaintenanceRepository dccbMaintenanceRepository;
        private readonly DCCBBasicInfoRepository dccbBasicInfoRepository;

        public DCCBMaintenanceController()
        {
            ViewBag.MenuType = "Maintenance";
            companyRepository = new CompanyRepository();
            maintenanceRepository = new MaintenanceRepository();
            dccbMaintenanceRepository = new DCCBMaintenanceRepository();
            dccbBasicInfoRepository = new DCCBBasicInfoRepository();
        }
    }
}