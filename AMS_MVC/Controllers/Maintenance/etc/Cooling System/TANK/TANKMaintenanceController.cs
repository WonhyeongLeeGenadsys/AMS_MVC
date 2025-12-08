using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class TANKMaintenanceController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly MaintenanceRepository maintenanceRepository;
        private readonly TANKMaintenanceRepository tankMaintenanceRepository;
        private readonly TANKBasicInfoRepository tankBasicInfoRepository;

        public TANKMaintenanceController()
        {
            ViewBag.MenuType = "Maintenance";
            companyRepository = new CompanyRepository();
            maintenanceRepository = new MaintenanceRepository();
            tankMaintenanceRepository = new TANKMaintenanceRepository();
            tankBasicInfoRepository = new TANKBasicInfoRepository();
        }
    }
}