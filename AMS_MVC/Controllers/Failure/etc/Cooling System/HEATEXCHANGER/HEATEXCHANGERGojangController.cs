using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class HEATEXCHANGERGojangController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly GojangRepository gojangRepository;
        private readonly HEATEXCHANGERGojangRepository heatexchangerGojangRepository;
        private readonly HEATEXCHANGERBasicInfoRepository heatexchangerBasicInfoRepository;

        public HEATEXCHANGERGojangController()
        {
            ViewBag.MenuType = "Gojang";
            companyRepository = new CompanyRepository();
            gojangRepository = new GojangRepository();
            heatexchangerGojangRepository = new HEATEXCHANGERGojangRepository();
            heatexchangerBasicInfoRepository = new HEATEXCHANGERBasicInfoRepository();
        }
    }
}