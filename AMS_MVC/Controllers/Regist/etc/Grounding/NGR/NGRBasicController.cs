
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class NGRBasicController : Controller
    {
        private readonly NGRBasicInfoRepository ngrBasicRepository;        
        private readonly CompanyRepository companyRepository;
              
        public NGRBasicController()
        {
            ViewBag.MenuType = "Regist"; // TopMenu 등록 Event 활성화
            ngrBasicRepository = new NGRBasicInfoRepository();
            companyRepository = new CompanyRepository();
        }
    }
}