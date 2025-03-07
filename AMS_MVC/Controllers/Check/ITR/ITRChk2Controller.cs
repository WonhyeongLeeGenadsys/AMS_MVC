using AMS_MVC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers.Check
{
    public partial class ITRChk2Controller : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly ITRChk2Repository itrChk2Repository;
        private readonly ITRBasicInfoRepository itrBasicInfoRepository;

        public ITRChk2Controller()
        {
            ViewBag.MenuType = "Check";
            ViewBag.ActiveMenu = "InterfaceTR";
            ViewBag.ActiveSubMenu = "ITRPrecision"; // 정밀점검
            companyRepository = new CompanyRepository();
            itrChk2Repository = new ITRChk2Repository();
            itrBasicInfoRepository = new ITRBasicInfoRepository();
        }
    }
}
