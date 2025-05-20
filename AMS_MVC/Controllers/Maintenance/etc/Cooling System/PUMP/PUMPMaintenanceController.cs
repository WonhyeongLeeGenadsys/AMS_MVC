using AMS_MVC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers.Maintenance.PUMP
{
    public partial class PUMPMaintenanceController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly MaintenanceRepository maintenanceRepository;
        private readonly PUMPMaintenanceRepository pumpMaintenanceRepository;
        private readonly PUMPBasicInfoRepository pumpBasicInfoRepository;

        public PUMPMaintenanceController()
        {
            ViewBag.MenuType = "Maintenance";
            companyRepository = new CompanyRepository();
            maintenanceRepository = new MaintenanceRepository();
            pumpMaintenanceRepository = new PUMPMaintenanceRepository();
            pumpBasicInfoRepository = new PUMPBasicInfoRepository();
        }
    }
}