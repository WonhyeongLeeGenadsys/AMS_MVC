using AMS_MVC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers
{
    public partial class WALLBUSHINGGojangController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly GojangRepository gojangRepository;
        private readonly WALLBUSHINGGojangRepository wallbushingGojangRepository;
        private readonly WALLBUSHINGBasicInfoRepository wallbushingBasicInfoRepository;

        public WALLBUSHINGGojangController()
        {
            ViewBag.MenuType = "Gojang";
            companyRepository = new CompanyRepository();
            gojangRepository = new GojangRepository();
            wallbushingGojangRepository = new WALLBUSHINGGojangRepository();
            wallbushingBasicInfoRepository = new WALLBUSHINGBasicInfoRepository();
        }
    }
}