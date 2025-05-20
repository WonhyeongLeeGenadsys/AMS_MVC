using AMS_MVC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers.Check
{
    public partial class DCCTChkController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly DCCTChkRepository dcctChkRepository;
        private readonly DCCTBasicInfoRepository dcctBasicInfoRepository;
        private RiskmatrixRepository riskMatrixRepository = new RiskmatrixRepository();

        public DCCTChkController()
        {
            ViewBag.MenuType = "Check";
            ViewBag.ActiveMenu = "DCCT";
            ViewBag.ActiveSubMenu = "DCCTRegular"; // 보통점검
            companyRepository = new CompanyRepository();
            dcctChkRepository = new DCCTChkRepository();
            dcctBasicInfoRepository = new DCCTBasicInfoRepository();            
        }
    }
}
