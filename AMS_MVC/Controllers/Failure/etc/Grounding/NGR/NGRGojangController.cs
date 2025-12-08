using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class NGRGojangController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly GojangRepository gojangRepository;
        private readonly NGRGojangRepository ngrGojangRepository;
        private readonly NGRBasicInfoRepository ngrBasicInfoRepository;

        public NGRGojangController()
        {
            ViewBag.MenuType = "Gojang";
            companyRepository = new CompanyRepository();
            gojangRepository = new GojangRepository();
            ngrGojangRepository = new NGRGojangRepository();
            ngrBasicInfoRepository = new NGRBasicInfoRepository();
        }
    }
}