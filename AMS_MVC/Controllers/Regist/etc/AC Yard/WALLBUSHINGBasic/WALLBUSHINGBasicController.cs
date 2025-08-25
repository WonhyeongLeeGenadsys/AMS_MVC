
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class WALLBUSHINGBasicController : Controller
    {
        private readonly WALLBUSHINGBasicInfoRepository wallbushingBasicRepository;        
        private readonly CompanyRepository companyRepository;
              
        public WALLBUSHINGBasicController()
        {
            ViewBag.MenuType = "Regist"; // TopMenu 등록 Event 활성화
            wallbushingBasicRepository = new WALLBUSHINGBasicInfoRepository();
            companyRepository = new CompanyRepository();
        }
    }
}