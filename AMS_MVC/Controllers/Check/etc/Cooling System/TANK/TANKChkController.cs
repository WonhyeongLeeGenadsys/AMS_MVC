using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class TANKChkController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly TANKChkRepository vcbChkRepository;
        private readonly TANKBasicInfoRepository vcbBasicInfoRepository;
        private RiskmatrixRepository riskMatrixRepository = new RiskmatrixRepository();

        public TANKChkController()
        {
            ViewBag.MenuType = "Check";
            ViewBag.ActiveMenu = "TANK";
            ViewBag.ActiveSubMenu = "TANKRegular"; // 보통점검
            companyRepository = new CompanyRepository();
            vcbChkRepository = new TANKChkRepository();
            vcbBasicInfoRepository = new TANKBasicInfoRepository();            
        }
    }
}
