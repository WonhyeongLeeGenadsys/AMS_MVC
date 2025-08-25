using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class ZIGZAGTRChkController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly ZIGZAGTRChkRepository zigzagtrChkRepository;
        private readonly ZIGZAGTRBasicInfoRepository zigzagtrBasicInfoRepository;
        private RiskmatrixRepository riskMatrixRepository = new RiskmatrixRepository();

        public ZIGZAGTRChkController()
        {
            ViewBag.MenuType = "Check";
            ViewBag.ActiveMenu = "ZIGZAGTR";
            ViewBag.ActiveSubMenu = "ZIGZAGTRRegular"; // 보통점검
            companyRepository = new CompanyRepository();
            zigzagtrChkRepository = new ZIGZAGTRChkRepository();
            zigzagtrBasicInfoRepository = new ZIGZAGTRBasicInfoRepository();            
        }
    }
}
