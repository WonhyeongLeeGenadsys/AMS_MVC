using AMS_MVC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers
{
    public partial class DCCBGojangController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly GojangRepository gojangRepository;
        private readonly DCCBGojangRepository dccbGojangRepository;
        private readonly DCCBBasicInfoRepository dccbBasicInfoRepository;

        public DCCBGojangController()
        {
            ViewBag.MenuType = "Gojang";
            companyRepository = new CompanyRepository();
            gojangRepository = new GojangRepository();
            dccbGojangRepository = new DCCBGojangRepository();
            dccbBasicInfoRepository = new DCCBBasicInfoRepository();
        }
    }
}