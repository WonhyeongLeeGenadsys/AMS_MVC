using AMS_MVC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers.Check
{
    public partial class VCBChkController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly VCBChkRepository vcbChkRepository;
        private readonly VCBBasicInfoRepository vcbBasicInfoRepository;
        private RiskmatrixRepository riskMatrixRepository = new RiskmatrixRepository();

        public VCBChkController()
        {
            ViewBag.MenuType = "Check";
            ViewBag.ActiveMenu = "VCB";
            ViewBag.ActiveSubMenu = "VCBRegular"; // 보통점검
            companyRepository = new CompanyRepository();
            vcbChkRepository = new VCBChkRepository();
            vcbBasicInfoRepository = new VCBBasicInfoRepository();            
        }
    }
}
