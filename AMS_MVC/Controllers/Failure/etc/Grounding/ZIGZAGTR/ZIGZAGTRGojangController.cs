using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class ZIGZAGTRGojangController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly GojangRepository gojangRepository;
        private readonly ZIGZAGTRGojangRepository zigzagtrGojangRepository;
        private readonly ZIGZAGTRBasicInfoRepository zigzagtrBasicInfoRepository;

        public ZIGZAGTRGojangController()
        {
            ViewBag.MenuType = "Gojang";
            companyRepository = new CompanyRepository();
            gojangRepository = new GojangRepository();
            zigzagtrGojangRepository = new ZIGZAGTRGojangRepository();
            zigzagtrBasicInfoRepository = new ZIGZAGTRBasicInfoRepository();
        }
    }
}