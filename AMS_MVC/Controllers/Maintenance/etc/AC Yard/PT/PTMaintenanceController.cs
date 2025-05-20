using AMS_MVC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers.Maintenance.PT
{
    public partial class PTMaintenanceController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly MaintenanceRepository maintenanceRepository;
        private readonly PTMaintenanceRepository ptMaintenanceRepository;
        private readonly PTBasicInfoRepository ptBasicInfoRepository;

        public PTMaintenanceController()
        {
            ViewBag.MenuType = "Maintenance";
            companyRepository = new CompanyRepository();
            maintenanceRepository = new MaintenanceRepository();
            ptMaintenanceRepository = new PTMaintenanceRepository();
            ptBasicInfoRepository = new PTBasicInfoRepository();
        }
    }
}