using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class VCBGojangController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly GojangRepository gojangRepository;
        private readonly VCBGojangRepository vcbGojangRepository;
        private readonly VCBBasicInfoRepository vcbBasicInfoRepository;

        public VCBGojangController()
        {
            ViewBag.MenuType = "Gojang";
            companyRepository = new CompanyRepository();
            gojangRepository = new GojangRepository();
            vcbGojangRepository = new VCBGojangRepository();
            vcbBasicInfoRepository = new VCBBasicInfoRepository();
        }
    }
}