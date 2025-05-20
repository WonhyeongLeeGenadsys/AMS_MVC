using AMS_MVC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers.Check
{
    public partial class ARMREACTORChkController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly ARMREACTORChkRepository armreactorChkRepository;
        private readonly ARMREACTORBasicInfoRepository armreactorBasicInfoRepository;
        private RiskmatrixRepository riskMatrixRepository = new RiskmatrixRepository();

        public ARMREACTORChkController()
        {
            ViewBag.MenuType = "Check";
            ViewBag.ActiveMenu = "ARMREACTOR";
            ViewBag.ActiveSubMenu = "ARMREACTORRegular"; // 보통점검
            companyRepository = new CompanyRepository();
            armreactorChkRepository = new ARMREACTORChkRepository();
            armreactorBasicInfoRepository = new ARMREACTORBasicInfoRepository();            
        }
    }
}
