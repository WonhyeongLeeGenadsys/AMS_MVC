using AMS_MVC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers.Maintenance.WALLBUSHING
{
    public partial class WALLBUSHINGMaintenanceController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly MaintenanceRepository maintenanceRepository;
        private readonly WALLBUSHINGMaintenanceRepository wallbushingMaintenanceRepository;
        private readonly WALLBUSHINGBasicInfoRepository wallbushingBasicInfoRepository;

        public WALLBUSHINGMaintenanceController()
        {
            ViewBag.MenuType = "Maintenance";
            companyRepository = new CompanyRepository();
            maintenanceRepository = new MaintenanceRepository();
            wallbushingMaintenanceRepository = new WALLBUSHINGMaintenanceRepository();
            wallbushingBasicInfoRepository = new WALLBUSHINGBasicInfoRepository();
        }
    }
}