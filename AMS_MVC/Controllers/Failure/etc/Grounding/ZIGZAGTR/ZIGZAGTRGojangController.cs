using AMS_MVC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers
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