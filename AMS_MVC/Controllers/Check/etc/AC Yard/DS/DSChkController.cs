using AMS_MVC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers.Check
{
    public partial class DSChkController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly DSChkRepository dsChkRepository;
        private readonly DSBasicInfoRepository dsBasicInfoRepository;
        private RiskmatrixRepository riskMatrixRepository = new RiskmatrixRepository();

        public DSChkController()
        {
            ViewBag.MenuType = "Check";
            ViewBag.ActiveMenu = "DS";
            ViewBag.ActiveSubMenu = "DSRegular"; // 보통점검
            companyRepository = new CompanyRepository();
            dsChkRepository = new DSChkRepository();
            dsBasicInfoRepository = new DSBasicInfoRepository();            
        }
    }
}
