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
    public partial class DCCABLEBasicController : Controller
    {
        private readonly DCCABLEBasicInfoRepository dccableBasicRepository;        
        private readonly CompanyRepository companyRepository;
              
        public DCCABLEBasicController()
        {
            ViewBag.MenuType = "Regist"; // TopMenu 등록 Event 활성화
            dccableBasicRepository = new DCCABLEBasicInfoRepository();
            companyRepository = new CompanyRepository();
        }
    }
}