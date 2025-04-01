using AMS_MVC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers.Maintenance.SUBMODULE
{
    public partial class SUBMODULEMaintenanceController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly MaintenanceRepository maintenanceRepository;
        private readonly SUBMODULEMaintenanceRepository submoduleMaintenanceRepository;
        private readonly SUBMODULEBasicInfoRepository submoduleBasicInfoRepository;

        public SUBMODULEMaintenanceController()
        {
            ViewBag.MenuType = "Maintenance";
            companyRepository = new CompanyRepository();
            maintenanceRepository = new MaintenanceRepository();
            submoduleMaintenanceRepository = new SUBMODULEMaintenanceRepository();
            submoduleBasicInfoRepository = new SUBMODULEBasicInfoRepository();
        }
    }
}