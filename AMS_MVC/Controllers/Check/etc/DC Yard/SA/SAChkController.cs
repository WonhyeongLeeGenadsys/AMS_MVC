using AMS_MVC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers.Check
{
    public partial class SAChkController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly SAChkRepository saChkRepository;
        private readonly SABasicInfoRepository saBasicInfoRepository;
        private RiskmatrixRepository riskMatrixRepository = new RiskmatrixRepository();

        public SAChkController()
        {
            ViewBag.MenuType = "Check";
            ViewBag.ActiveMenu = "SA";
            ViewBag.ActiveSubMenu = "SARegular"; // 보통점검
            companyRepository = new CompanyRepository();
            saChkRepository = new SAChkRepository();
            saBasicInfoRepository = new SABasicInfoRepository();            
        }
    }
}
