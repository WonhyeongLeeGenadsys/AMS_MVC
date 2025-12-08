using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class PTGojangController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly GojangRepository gojangRepository;
        private readonly PTGojangRepository ptGojangRepository;
        private readonly PTBasicInfoRepository ptBasicInfoRepository;

        public PTGojangController()
        {
            ViewBag.MenuType = "Gojang";
            companyRepository = new CompanyRepository();
            gojangRepository = new GojangRepository();
            ptGojangRepository = new PTGojangRepository();
            ptBasicInfoRepository = new PTBasicInfoRepository();
        }
    }
}