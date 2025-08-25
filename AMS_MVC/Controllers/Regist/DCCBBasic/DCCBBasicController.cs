
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class DCCBBasicController : Controller
    {
        private readonly DCCBBasicInfoRepository dccbBasicRepository;        
        private readonly CompanyRepository companyRepository;
              
        public DCCBBasicController()
        {
            ViewBag.MenuType = "Regist"; // TopMenu 등록 Event 활성화
            dccbBasicRepository = new DCCBBasicInfoRepository();
            companyRepository = new CompanyRepository();
        }
    }
}