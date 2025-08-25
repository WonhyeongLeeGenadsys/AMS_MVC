using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class DSGojangController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly GojangRepository gojangRepository;
        private readonly DSGojangRepository dsGojangRepository;
        private readonly DSBasicInfoRepository dsBasicInfoRepository;

        public DSGojangController()
        {
            ViewBag.MenuType = "Gojang";
            companyRepository = new CompanyRepository();
            gojangRepository = new GojangRepository();
            dsGojangRepository = new DSGojangRepository();
            dsBasicInfoRepository = new DSBasicInfoRepository();
        }
    }
}