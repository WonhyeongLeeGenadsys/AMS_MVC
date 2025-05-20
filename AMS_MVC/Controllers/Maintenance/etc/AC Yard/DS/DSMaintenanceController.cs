using AMS_MVC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers.Maintenance.DS
{
    public partial class DSMaintenanceController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly MaintenanceRepository maintenanceRepository;
        private readonly DSMaintenanceRepository dsMaintenanceRepository;
        private readonly DSBasicInfoRepository dsBasicInfoRepository;

        public DSMaintenanceController()
        {
            ViewBag.MenuType = "Maintenance";
            companyRepository = new CompanyRepository();
            maintenanceRepository = new MaintenanceRepository();
            dsMaintenanceRepository = new DSMaintenanceRepository();
            dsBasicInfoRepository = new DSBasicInfoRepository();
        }
    }
}