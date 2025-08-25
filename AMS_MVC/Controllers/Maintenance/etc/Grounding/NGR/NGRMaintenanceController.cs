using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
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