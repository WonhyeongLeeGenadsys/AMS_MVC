using AMS_MVC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers.Maintenance.CT
{
    public partial class CTMaintenanceController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly MaintenanceRepository maintenanceRepository;
        private readonly CTMaintenanceRepository ctMaintenanceRepository;
        private readonly CTBasicInfoRepository ctBasicInfoRepository;

        public CTMaintenanceController()
        {
            ViewBag.MenuType = "Maintenance";
            companyRepository = new CompanyRepository();
            maintenanceRepository = new MaintenanceRepository();
            ctMaintenanceRepository = new CTMaintenanceRepository();
            ctBasicInfoRepository = new CTBasicInfoRepository();
        }
    }
}