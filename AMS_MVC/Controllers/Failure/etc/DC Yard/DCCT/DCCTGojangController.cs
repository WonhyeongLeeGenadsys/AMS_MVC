using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class DCCTGojangController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly GojangRepository gojangRepository;
        private readonly DCCTGojangRepository dcctGojangRepository;
        private readonly DCCTBasicInfoRepository dcctBasicInfoRepository;

        public DCCTGojangController()
        {
            ViewBag.MenuType = "Gojang";
            companyRepository = new CompanyRepository();
            gojangRepository = new GojangRepository();
            dcctGojangRepository = new DCCTGojangRepository();
            dcctBasicInfoRepository = new DCCTBasicInfoRepository();
        }
    }
}