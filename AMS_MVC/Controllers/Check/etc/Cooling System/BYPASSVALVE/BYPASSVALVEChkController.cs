using AMS_MVC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers.Check
{
    public partial class BYPASSVALVEChkController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly BYPASSVALVEChkRepository bypassvalveChkRepository;
        private readonly BYPASSVALVEBasicInfoRepository bypassvalveBasicInfoRepository;
        private RiskmatrixRepository riskMatrixRepository = new RiskmatrixRepository();

        public BYPASSVALVEChkController()
        {
            ViewBag.MenuType = "Check";
            ViewBag.ActiveMenu = "BYPASSVALVE";
            ViewBag.ActiveSubMenu = "BYPASSVALVERegular"; // 보통점검
            companyRepository = new CompanyRepository();
            bypassvalveChkRepository = new BYPASSVALVEChkRepository();
            bypassvalveBasicInfoRepository = new BYPASSVALVEBasicInfoRepository();            
        }
    }
}
