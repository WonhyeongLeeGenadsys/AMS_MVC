using AMS_MVC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers.Check
{
    public partial class PUMPChkController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly PUMPChkRepository pumpChkRepository;
        private readonly PUMPBasicInfoRepository pumpBasicInfoRepository;
        private RiskmatrixRepository riskMatrixRepository = new RiskmatrixRepository();

        public PUMPChkController()
        {
            ViewBag.MenuType = "Check";
            ViewBag.ActiveMenu = "PUMP";
            ViewBag.ActiveSubMenu = "PUMPRegular"; // 보통점검
            companyRepository = new CompanyRepository();
            pumpChkRepository = new PUMPChkRepository();
            pumpBasicInfoRepository = new PUMPBasicInfoRepository();            
        }
    }
}
