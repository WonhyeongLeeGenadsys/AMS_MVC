using AMS_MVC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers
{
    public partial class ITRGojangController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly GojangRepository GojangRepository;
        private readonly ITRGojangRepository itrGojangRepository;
        private readonly ITRBasicInfoRepository itrBasicInfoRepository;

        public ITRGojangController()
        {
            ViewBag.MenuType = "Gojang";
            companyRepository = new CompanyRepository();
            GojangRepository = new GojangRepository();
            itrGojangRepository = new ITRGojangRepository();
            itrBasicInfoRepository = new ITRBasicInfoRepository();
        }
    }
}
