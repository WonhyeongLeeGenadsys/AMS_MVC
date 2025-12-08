using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class TANKGojangController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly GojangRepository gojangRepository;
        private readonly TANKGojangRepository tankGojangRepository;
        private readonly TANKBasicInfoRepository tankBasicInfoRepository;

        public TANKGojangController()
        {
            ViewBag.MenuType = "Gojang";
            companyRepository = new CompanyRepository();
            gojangRepository = new GojangRepository();
            tankGojangRepository = new TANKGojangRepository();
            tankBasicInfoRepository = new TANKBasicInfoRepository();
        }
    }
}