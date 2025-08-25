
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class ESBasicController : Controller
    {
        private readonly ESBasicInfoRepository esBasicRepository;        
        private readonly CompanyRepository companyRepository;
              
        public ESBasicController()
        {
            ViewBag.MenuType = "Regist"; // TopMenu 등록 Event 활성화
            esBasicRepository = new ESBasicInfoRepository();
            companyRepository = new CompanyRepository();
        }
    }
}