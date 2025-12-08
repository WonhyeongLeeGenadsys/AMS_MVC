using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class ESMaintenanceController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly MaintenanceRepository maintenanceRepository;
        private readonly ESMaintenanceRepository esMaintenanceRepository;
        private readonly ESBasicInfoRepository esBasicInfoRepository;

        public ESMaintenanceController()
        {
            ViewBag.MenuType = "Maintenance";
            companyRepository = new CompanyRepository();
            maintenanceRepository = new MaintenanceRepository();
            esMaintenanceRepository = new ESMaintenanceRepository();
            esBasicInfoRepository = new ESBasicInfoRepository();
        }
    }
}