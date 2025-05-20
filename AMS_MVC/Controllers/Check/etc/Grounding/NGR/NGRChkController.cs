using AMS_MVC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers.Check
{
    public partial class NGRChkController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly NGRChkRepository ngrChkRepository;
        private readonly NGRBasicInfoRepository ngrBasicInfoRepository;
        private RiskmatrixRepository riskMatrixRepository = new RiskmatrixRepository();

        public NGRChkController()
        {
            ViewBag.MenuType = "Check";
            ViewBag.ActiveMenu = "NGR";
            ViewBag.ActiveSubMenu = "NGRRegular"; // 보통점검
            companyRepository = new CompanyRepository();
            ngrChkRepository = new NGRChkRepository();
            ngrBasicInfoRepository = new NGRBasicInfoRepository();            
        }
    }
}
