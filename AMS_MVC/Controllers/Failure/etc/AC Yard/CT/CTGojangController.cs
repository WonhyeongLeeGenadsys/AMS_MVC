using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class CTGojangController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly GojangRepository gojangRepository;
        private readonly CTGojangRepository ctGojangRepository;
        private readonly CTBasicInfoRepository ctBasicInfoRepository;

        public CTGojangController()
        {
            ViewBag.MenuType = "Gojang";
            companyRepository = new CompanyRepository();
            gojangRepository = new GojangRepository();
            ctGojangRepository = new CTGojangRepository();
            ctBasicInfoRepository = new CTBasicInfoRepository();
        }
    }
}