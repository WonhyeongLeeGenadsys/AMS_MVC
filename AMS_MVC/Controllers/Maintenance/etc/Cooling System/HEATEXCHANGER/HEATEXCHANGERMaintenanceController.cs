using AMS_MVC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers.Maintenance.HEATEXCHANGER
{
    public partial class HEATEXCHANGERMaintenanceController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly MaintenanceRepository maintenanceRepository;
        private readonly HEATEXCHANGERMaintenanceRepository heatexchangerMaintenanceRepository;
        private readonly HEATEXCHANGERBasicInfoRepository heatexchangerBasicInfoRepository;

        public HEATEXCHANGERMaintenanceController()
        {
            ViewBag.MenuType = "Maintenance";
            companyRepository = new CompanyRepository();
            maintenanceRepository = new MaintenanceRepository();
            heatexchangerMaintenanceRepository = new HEATEXCHANGERMaintenanceRepository();
            heatexchangerBasicInfoRepository = new HEATEXCHANGERBasicInfoRepository();
        }
    }
}