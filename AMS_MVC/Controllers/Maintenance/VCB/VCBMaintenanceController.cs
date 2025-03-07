using AMS_MVC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers.Maintenance.VCB
{
    public partial class VCBMaintenanceController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly MaintenanceRepository maintenanceRepository;
        private readonly VCBMaintenanceRepository vcbMaintenanceRepository;
        private readonly VCBBasicInfoRepository vcbBasicInfoRepository;

        public VCBMaintenanceController()
        {
            ViewBag.MenuType = "Maintenance";
            companyRepository = new CompanyRepository();
            maintenanceRepository = new MaintenanceRepository();
            vcbMaintenanceRepository = new VCBMaintenanceRepository();
            vcbBasicInfoRepository = new VCBBasicInfoRepository();
        }
    }
}