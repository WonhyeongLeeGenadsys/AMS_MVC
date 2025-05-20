using AMS_MVC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers.Check
{
    public partial class HEATEXCHANGERChkController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly HEATEXCHANGERChkRepository heatexchangerChkRepository;
        private readonly HEATEXCHANGERBasicInfoRepository heatexchangerBasicInfoRepository;
        private RiskmatrixRepository riskMatrixRepository = new RiskmatrixRepository();

        public HEATEXCHANGERChkController()
        {
            ViewBag.MenuType = "Check";
            ViewBag.ActiveMenu = "HEATEXCHANGER";
            ViewBag.ActiveSubMenu = "HEATEXCHANGERRegular"; // 보통점검
            companyRepository = new CompanyRepository();
            heatexchangerChkRepository = new HEATEXCHANGERChkRepository();
            heatexchangerBasicInfoRepository = new HEATEXCHANGERBasicInfoRepository();            
        }
    }
}
