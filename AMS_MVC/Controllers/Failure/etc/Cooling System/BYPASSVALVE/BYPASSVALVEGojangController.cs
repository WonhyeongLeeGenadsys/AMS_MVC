using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class BYPASSVALVEGojangController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly GojangRepository gojangRepository;
        private readonly BYPASSVALVEGojangRepository bypassvalveGojangRepository;
        private readonly BYPASSVALVEBasicInfoRepository bypassvalveBasicInfoRepository;

        public BYPASSVALVEGojangController()
        {
            ViewBag.MenuType = "Gojang";
            companyRepository = new CompanyRepository();
            gojangRepository = new GojangRepository();
            bypassvalveGojangRepository = new BYPASSVALVEGojangRepository();
            bypassvalveBasicInfoRepository = new BYPASSVALVEBasicInfoRepository();
        }
    }
}