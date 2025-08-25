
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class SUBMODULEBasicController : Controller
    {
        private readonly SUBMODULEBasicInfoRepository submoduleBasicRepository;        
        private readonly CompanyRepository companyRepository;
              
        public SUBMODULEBasicController()
        {
            ViewBag.MenuType = "Regist"; // TopMenu 등록 Event 활성화
            submoduleBasicRepository = new SUBMODULEBasicInfoRepository();
            companyRepository = new CompanyRepository();
        }
    }
}