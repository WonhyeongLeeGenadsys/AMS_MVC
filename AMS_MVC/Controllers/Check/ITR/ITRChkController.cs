// Controllers/Check/ITRChkController.cs
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class ITRChkController : Controller
    {
        protected readonly CompanyRepository _companyRepo;
        protected readonly ITRBasicInfoRepository _basicRepo;
        protected readonly ITRChk1Repository _chk1Repo;
        protected readonly ITRChk2Repository _chk2Repo;

        private readonly ITRChkScoreCalculator _scoreCalc = new ITRChkScoreCalculator();
        private readonly RiskmatrixRepository _riskRepo = new RiskmatrixRepository();
        private CoFRepository cofRepo = new CoFRepository();

        public ITRChkController()
        {
            ViewBag.MenuType = "Check";
            ViewBag.ActiveMenu = "InterfaceTR";

            _companyRepo = new CompanyRepository();
            _basicRepo = new ITRBasicInfoRepository();
            _chk1Repo = new ITRChk1Repository();
            _chk2Repo = new ITRChk2Repository();
            _scoreCalc = new ITRChkScoreCalculator();
            _riskRepo = new RiskmatrixRepository();
        }
    }
}
