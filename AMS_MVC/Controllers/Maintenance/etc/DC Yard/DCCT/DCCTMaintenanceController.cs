using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class DCCTMaintenanceController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly MaintenanceRepository maintenanceRepository;
        private readonly DCCTMaintenanceRepository dcctMaintenanceRepository;
        private readonly DCCTBasicInfoRepository dcctBasicInfoRepository;

        public DCCTMaintenanceController()
        {
            ViewBag.MenuType = "Maintenance";
            companyRepository = new CompanyRepository();
            maintenanceRepository = new MaintenanceRepository();
            dcctMaintenanceRepository = new DCCTMaintenanceRepository();
            dcctBasicInfoRepository = new DCCTBasicInfoRepository();
        }
    }
}