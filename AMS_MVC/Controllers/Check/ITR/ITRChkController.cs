// Controllers/Check/ITRChkController.cs
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AMS_MVC.Models;
using AMS_MVC.Repositories;
using Web.Common.Log;
using AMS_MVC.Utlity;

namespace AMS_MVC.Controllers.Check
{
    public partial class ITRChkController : Controller
    {
        protected readonly CompanyRepository _companyRepo;
        protected readonly ITRBasicInfoRepository _basicRepo;
        protected readonly ITRChk1Repository _chk1Repo;
        protected readonly ITRChk2Repository _chk2Repo;

        public ITRChkController()
        {
            ViewBag.MenuType = "Check";
            ViewBag.ActiveMenu = "InterfaceTR";
            _companyRepo = new CompanyRepository();
            _basicRepo = new ITRBasicInfoRepository();
            _chk1Repo = new ITRChk1Repository();
            _chk2Repo = new ITRChk2Repository();
        }
    }
}
