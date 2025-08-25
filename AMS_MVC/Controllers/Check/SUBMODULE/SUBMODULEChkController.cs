using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class SUBMODULEChkController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly SUBMODULEChkRepository submoduleChkRepository;
        private readonly SUBMODULEBasicInfoRepository submoduleBasicInfoRepository;
        private RiskmatrixRepository riskMatrixRepository = new RiskmatrixRepository();
        private CoFRepository cofRepo = new CoFRepository();

        public SUBMODULEChkController()
        {
            ViewBag.MenuType = "Check";
            ViewBag.ActiveMenu = "SUBMODULE";
            ViewBag.ActiveSubMenu = "SUBMODULERegular"; // 보통점검
            companyRepository = new CompanyRepository();
            submoduleChkRepository = new SUBMODULEChkRepository();
            submoduleBasicInfoRepository = new SUBMODULEBasicInfoRepository();            
        }
    }
}
