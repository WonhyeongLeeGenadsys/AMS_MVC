
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class PTBasicController : Controller
    {
        private readonly PTBasicInfoRepository ptBasicRepository;        
        private readonly CompanyRepository companyRepository;
              
        public PTBasicController()
        {
            ViewBag.MenuType = "Regist"; // TopMenu 등록 Event 활성화
            ptBasicRepository = new PTBasicInfoRepository();
            companyRepository = new CompanyRepository();
        }
    }
}