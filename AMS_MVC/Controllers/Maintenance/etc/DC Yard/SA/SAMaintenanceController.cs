using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
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