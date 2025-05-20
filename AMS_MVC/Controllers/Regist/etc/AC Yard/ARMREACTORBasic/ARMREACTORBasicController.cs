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
    public partial class ARMREACTORBasicController : Controller
    {
        private readonly ARMREACTORBasicInfoRepository armreactorBasicRepository;        
        private readonly CompanyRepository companyRepository;
              
        public ARMREACTORBasicController()
        {
            ViewBag.MenuType = "Regist"; // TopMenu 등록 Event 활성화
            armreactorBasicRepository = new ARMREACTORBasicInfoRepository();
            companyRepository = new CompanyRepository();
        }
    }
}