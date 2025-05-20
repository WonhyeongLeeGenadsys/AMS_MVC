using AMS_MVC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers.Maintenance.SA
{
    public partial class SAMaintenanceController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly MaintenanceRepository maintenanceRepository;
        private readonly SAMaintenanceRepository saMaintenanceRepository;
        private readonly SABasicInfoRepository saBasicInfoRepository;

        public SAMaintenanceController()
        {
            ViewBag.MenuType = "Maintenance";
            companyRepository = new CompanyRepository();
            maintenanceRepository = new MaintenanceRepository();
            saMaintenanceRepository = new SAMaintenanceRepository();
            saBasicInfoRepository = new SABasicInfoRepository();
        }
    }
}