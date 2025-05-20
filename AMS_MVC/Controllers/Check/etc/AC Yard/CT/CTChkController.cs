using AMS_MVC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers.Check
{
    public partial class CTChkController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly CTChkRepository ctChkRepository;
        private readonly CTBasicInfoRepository ctBasicInfoRepository;
        private RiskmatrixRepository riskMatrixRepository = new RiskmatrixRepository();

        public CTChkController()
        {
            ViewBag.MenuType = "Check";
            ViewBag.ActiveMenu = "CT";
            ViewBag.ActiveSubMenu = "CTRegular"; // 보통점검
            companyRepository = new CompanyRepository();
            ctChkRepository = new CTChkRepository();
            ctBasicInfoRepository = new CTBasicInfoRepository();            
        }
    }
}
