using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class ARMREACTORMaintenanceController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly MaintenanceRepository maintenanceRepository;
        private readonly ARMREACTORMaintenanceRepository armreactorMaintenanceRepository;
        private readonly ARMREACTORBasicInfoRepository armreactorBasicInfoRepository;

        public ARMREACTORMaintenanceController()
        {
            ViewBag.MenuType = "Maintenance";
            companyRepository = new CompanyRepository();
            maintenanceRepository = new MaintenanceRepository();
            armreactorMaintenanceRepository = new ARMREACTORMaintenanceRepository();
            armreactorBasicInfoRepository = new ARMREACTORBasicInfoRepository();
        }
    }
}