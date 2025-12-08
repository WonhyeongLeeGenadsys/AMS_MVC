using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class DCCBChkController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly DCCBChkRepository dccbChkRepository;
        private readonly DCCBBasicInfoRepository dccbBasicInfoRepository;
        private RiskmatrixRepository riskMatrixRepository = new RiskmatrixRepository();
        private CoFRepository cofRepo = new CoFRepository();

        public DCCBChkController()
        {
            ViewBag.MenuType = "Check";
            ViewBag.ActiveMenu = "DCCB";
            ViewBag.ActiveSubMenu = "DCCBRegular"; // 보통점검
            companyRepository = new CompanyRepository();
            dccbChkRepository = new DCCBChkRepository();
            dccbBasicInfoRepository = new DCCBBasicInfoRepository();            
        }
    }
}
