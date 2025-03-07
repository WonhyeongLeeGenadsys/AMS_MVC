using AMS_MVC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers.Maintenance.ITR
{
    public partial class ITRMaintenanceController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly MaintenanceRepository maintenanceRepository;
        private readonly ITRMaintenanceRepository itrMaintenanceRepository;
        private readonly ITRBasicInfoRepository itrBasicInfoRepository;

        public ITRMaintenanceController()
        {
            ViewBag.MenuType = "Maintenance";
            companyRepository = new CompanyRepository();
            maintenanceRepository = new MaintenanceRepository();
            itrMaintenanceRepository = new ITRMaintenanceRepository();
            itrBasicInfoRepository = new ITRBasicInfoRepository();
        }
    }
}