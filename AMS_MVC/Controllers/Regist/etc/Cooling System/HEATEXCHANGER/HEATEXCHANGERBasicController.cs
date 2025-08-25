
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class HEATEXCHANGERBasicController : Controller
    {
        private readonly HEATEXCHANGERBasicInfoRepository heatexchangerBasicRepository;        
        private readonly CompanyRepository companyRepository;
              
        public HEATEXCHANGERBasicController()
        {
            ViewBag.MenuType = "Regist"; // TopMenu 등록 Event 활성화
            heatexchangerBasicRepository = new HEATEXCHANGERBasicInfoRepository();
            companyRepository = new CompanyRepository();
        }
    }
}