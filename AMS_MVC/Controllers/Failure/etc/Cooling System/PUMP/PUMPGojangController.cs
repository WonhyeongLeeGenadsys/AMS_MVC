using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class PUMPGojangController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly GojangRepository gojangRepository;
        private readonly PUMPGojangRepository pumpGojangRepository;
        private readonly PUMPBasicInfoRepository pumpBasicInfoRepository;

        public PUMPGojangController()
        {
            ViewBag.MenuType = "Gojang";
            companyRepository = new CompanyRepository();
            gojangRepository = new GojangRepository();
            pumpGojangRepository = new PUMPGojangRepository();
            pumpBasicInfoRepository = new PUMPBasicInfoRepository();
        }
    }
}