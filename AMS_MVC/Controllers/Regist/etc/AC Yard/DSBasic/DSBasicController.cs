using AMS_MVC.Models;
using AMS_MVC.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers
{
    public partial class DSBasicController : Controller
    {
        private readonly DSBasicInfoRepository dsBasicRepository;        
        private readonly CompanyRepository companyRepository;
              
        public DSBasicController()
        {
            ViewBag.MenuType = "Regist"; // TopMenu 등록 Event 활성화
            dsBasicRepository = new DSBasicInfoRepository();
            companyRepository = new CompanyRepository();
        }
    }
}