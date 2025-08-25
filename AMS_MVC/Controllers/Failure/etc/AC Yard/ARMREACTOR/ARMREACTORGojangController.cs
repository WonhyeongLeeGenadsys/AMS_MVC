using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class ARMREACTORGojangController : Controller
    {
        private readonly CompanyRepository companyRepository;
        private readonly GojangRepository gojangRepository;
        private readonly ARMREACTORGojangRepository armreactorGojangRepository;
        private readonly ARMREACTORBasicInfoRepository armreactorBasicInfoRepository;

        public ARMREACTORGojangController()
        {
            ViewBag.MenuType = "Gojang";
            companyRepository = new CompanyRepository();
            gojangRepository = new GojangRepository();
            armreactorGojangRepository = new ARMREACTORGojangRepository();
            armreactorBasicInfoRepository = new ARMREACTORBasicInfoRepository();
        }
    }
}