using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class SAGojangController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly GojangRepository gojangRepository;
        private readonly SAGojangRepository saGojangRepository;
        private readonly SABasicInfoRepository saBasicInfoRepository;

        public SAGojangController()
        {
            ViewBag.MenuType = "Gojang";
            companyRepository = new CompanyRepository();
            gojangRepository = new GojangRepository();
            saGojangRepository = new SAGojangRepository();
            saBasicInfoRepository = new SABasicInfoRepository();
        }
    }
}