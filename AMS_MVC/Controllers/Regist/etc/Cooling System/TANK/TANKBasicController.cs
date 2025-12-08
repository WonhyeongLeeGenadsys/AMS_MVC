
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class TANKBasicController : Controller
    {
        private readonly TANKBasicInfoRepository tankBasicRepository;        
        private readonly CompanyRepository companyRepository;
              
        public TANKBasicController()
        {
            ViewBag.MenuType = "Regist"; // TopMenu 등록 Event 활성화
            tankBasicRepository = new TANKBasicInfoRepository();
            companyRepository = new CompanyRepository();
        }
    }
}