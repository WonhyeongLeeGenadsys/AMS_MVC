using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class ESGojangController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly GojangRepository gojangRepository;
        private readonly ESGojangRepository esGojangRepository;
        private readonly ESBasicInfoRepository esBasicInfoRepository;

        public ESGojangController()
        {
            ViewBag.MenuType = "Gojang";
            companyRepository = new CompanyRepository();
            gojangRepository = new GojangRepository();
            esGojangRepository = new ESGojangRepository();
            esBasicInfoRepository = new ESBasicInfoRepository();
        }
    }
}