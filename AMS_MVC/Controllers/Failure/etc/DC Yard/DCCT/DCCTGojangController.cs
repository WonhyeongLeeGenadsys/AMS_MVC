using AMS_MVC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers
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