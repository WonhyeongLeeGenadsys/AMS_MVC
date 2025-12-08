using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class WALLBUSHINGChkController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly WALLBUSHINGChkRepository wallbushingChkRepository;
        private readonly WALLBUSHINGBasicInfoRepository wallbushingBasicInfoRepository;
        private RiskmatrixRepository riskMatrixRepository = new RiskmatrixRepository();

        public WALLBUSHINGChkController()
        {
            ViewBag.MenuType = "Check";
            ViewBag.ActiveMenu = "WALLBUSHING";
            ViewBag.ActiveSubMenu = "WALLBUSHINGRegular"; // 보통점검
            companyRepository = new CompanyRepository();
            wallbushingChkRepository = new WALLBUSHINGChkRepository();
            wallbushingBasicInfoRepository = new WALLBUSHINGBasicInfoRepository();            
        }
    }
}
