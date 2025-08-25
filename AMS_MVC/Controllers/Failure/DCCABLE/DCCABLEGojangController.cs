using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class DCCABLEGojangController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly GojangRepository gojangRepository;
        private readonly DCCABLEGojangRepository dccableGojangRepository;
        private readonly DCCABLEBasicInfoRepository dccableBasicInfoRepository;

        public DCCABLEGojangController()
        {
            ViewBag.MenuType = "Gojang";
            companyRepository = new CompanyRepository();
            gojangRepository = new GojangRepository();
            dccableGojangRepository = new DCCABLEGojangRepository();
            dccableBasicInfoRepository = new DCCABLEBasicInfoRepository();
        }
    }
}