using AMS_MVC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers.Check
{
    public partial class PTChkController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly PTChkRepository ptChkRepository;
        private readonly PTBasicInfoRepository ptBasicInfoRepository;
        private RiskmatrixRepository riskMatrixRepository = new RiskmatrixRepository();

        public PTChkController()
        {
            ViewBag.MenuType = "Check";
            ViewBag.ActiveMenu = "PT";
            ViewBag.ActiveSubMenu = "PTRegular"; // 보통점검
            companyRepository = new CompanyRepository();
            ptChkRepository = new PTChkRepository();
            ptBasicInfoRepository = new PTBasicInfoRepository();            
        }
    }
}
