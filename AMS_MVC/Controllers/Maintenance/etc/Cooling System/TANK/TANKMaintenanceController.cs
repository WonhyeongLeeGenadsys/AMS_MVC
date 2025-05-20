using AMS_MVC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers.Maintenance.TANK
{
    public partial class TANKMaintenanceController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly MaintenanceRepository maintenanceRepository;
        private readonly TANKMaintenanceRepository tankMaintenanceRepository;
        private readonly TANKBasicInfoRepository tankBasicInfoRepository;

        public TANKMaintenanceController()
        {
            ViewBag.MenuType = "Maintenance";
            companyRepository = new CompanyRepository();
            maintenanceRepository = new MaintenanceRepository();
            tankMaintenanceRepository = new TANKMaintenanceRepository();
            tankBasicInfoRepository = new TANKBasicInfoRepository();
        }
    }
}