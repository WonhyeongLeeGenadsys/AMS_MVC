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
    public partial class PUMPBasicController : Controller
    {
        private readonly PUMPBasicInfoRepository pumpBasicRepository;        
        private readonly CompanyRepository companyRepository;
              
        public PUMPBasicController()
        {
            ViewBag.MenuType = "Regist"; // TopMenu 등록 Event 활성화
            pumpBasicRepository = new PUMPBasicInfoRepository();
            companyRepository = new CompanyRepository();
        }
    }
}