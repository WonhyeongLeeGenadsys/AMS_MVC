using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class LAChkController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly LAChkRepository laChkRepository;
        private readonly LABasicInfoRepository laBasicInfoRepository;
        private RiskmatrixRepository riskMatrixRepository = new RiskmatrixRepository();

        public LAChkController()
        {
            ViewBag.MenuType = "Check";
            ViewBag.ActiveMenu = "LA";
            ViewBag.ActiveSubMenu = "LARegular"; // 보통점검
            companyRepository = new CompanyRepository();
            laChkRepository = new LAChkRepository();
            laBasicInfoRepository = new LABasicInfoRepository();            
        }
    }
}
