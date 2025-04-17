using AMS_MVC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers.Check
{
    public partial class DCCABLEChkController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly DCCABLEChkRepository dccableChkRepository;
        private readonly DCCABLEBasicInfoRepository dccableBasicInfoRepository;
        private RiskmatrixRepository riskMatrixRepository = new RiskmatrixRepository();

        public DCCABLEChkController()
        {
            ViewBag.MenuType = "Check";
            ViewBag.ActiveMenu = "DCCABLE";
            ViewBag.ActiveSubMenu = "DCCABLERegular"; // 보통점검
            companyRepository = new CompanyRepository();
            dccableChkRepository = new DCCABLEChkRepository();
            dccableBasicInfoRepository = new DCCABLEBasicInfoRepository();            
        }
    }
}
