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
    public partial class LABasicController : Controller
    {
        private readonly LABasicInfoRepository laBasicRepository;        
        private readonly CompanyRepository companyRepository;
              
        public LABasicController()
        {
            ViewBag.MenuType = "Regist"; // TopMenu 등록 Event 활성화
            laBasicRepository = new LABasicInfoRepository();
            companyRepository = new CompanyRepository();
        }
    }
}