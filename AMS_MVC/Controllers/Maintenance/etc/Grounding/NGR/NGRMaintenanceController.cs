using AMS_MVC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers.Maintenance.NGR
{
    public partial class NGRMaintenanceController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly MaintenanceRepository maintenanceRepository;
        private readonly NGRMaintenanceRepository ngrMaintenanceRepository;
        private readonly NGRBasicInfoRepository ngrBasicInfoRepository;

        public NGRMaintenanceController()
        {
            ViewBag.MenuType = "Maintenance";
            companyRepository = new CompanyRepository();
            maintenanceRepository = new MaintenanceRepository();
            ngrMaintenanceRepository = new NGRMaintenanceRepository();
            ngrBasicInfoRepository = new NGRBasicInfoRepository();
        }
    }
}