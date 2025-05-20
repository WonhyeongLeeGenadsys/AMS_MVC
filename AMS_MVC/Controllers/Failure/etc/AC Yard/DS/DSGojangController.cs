using AMS_MVC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers
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