using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class BYPASSVALVEMaintenanceController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly MaintenanceRepository maintenanceRepository;
        private readonly BYPASSVALVEMaintenanceRepository bypassvalveMaintenanceRepository;
        private readonly BYPASSVALVEBasicInfoRepository bypassvalveBasicInfoRepository;

        public BYPASSVALVEMaintenanceController()
        {
            ViewBag.MenuType = "Maintenance";
            companyRepository = new CompanyRepository();
            maintenanceRepository = new MaintenanceRepository();
            bypassvalveMaintenanceRepository = new BYPASSVALVEMaintenanceRepository();
            bypassvalveBasicInfoRepository = new BYPASSVALVEBasicInfoRepository();
        }
    }
}