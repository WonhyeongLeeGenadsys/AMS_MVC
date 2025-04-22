using AMS_MVC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers.Check
{
    public partial class ITRChk1Controller : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly ITRChk1Repository itrChk1Repository;
        private readonly ITRBasicInfoRepository itrBasicInfoRepository;

        public ITRChk1Controller()
        {
            ViewBag.MenuType = "Check";
            ViewBag.ActiveMenu = "InterfaceTR";
            ViewBag.ActiveSubMenu = "ITRRegular"; // 보통점검
            companyRepository = new CompanyRepository();
            itrChk1Repository = new ITRChk1Repository();
            itrBasicInfoRepository = new ITRBasicInfoRepository();
        }
    }
}