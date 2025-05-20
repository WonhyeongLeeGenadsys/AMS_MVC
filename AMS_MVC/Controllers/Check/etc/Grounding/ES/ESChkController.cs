using AMS_MVC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers.Check
{
    public partial class ESChkController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly ESChkRepository esChkRepository;
        private readonly ESBasicInfoRepository esBasicInfoRepository;
        private RiskmatrixRepository riskMatrixRepository = new RiskmatrixRepository();

        public ESChkController()
        {
            ViewBag.MenuType = "Check";
            ViewBag.ActiveMenu = "ES";
            ViewBag.ActiveSubMenu = "ESRegular"; // 보통점검
            companyRepository = new CompanyRepository();
            esChkRepository = new ESChkRepository();
            esBasicInfoRepository = new ESBasicInfoRepository();            
        }
    }
}
