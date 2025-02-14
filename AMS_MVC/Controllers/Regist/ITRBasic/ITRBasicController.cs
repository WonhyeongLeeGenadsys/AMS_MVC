using AMS_MVC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers
{
    public partial class ITRBasicController : Controller
    {
        private readonly ITRBasicInfoRepository itrBasicRepository;
        private readonly CompanyRepository companyRepository;

        public ITRBasicController()
        {
            ViewBag.MenuType = "Regist"; // TopMenu 등록 Event 활성화
            itrBasicRepository = new ITRBasicInfoRepository();
            companyRepository = new CompanyRepository();
        }
    }
}