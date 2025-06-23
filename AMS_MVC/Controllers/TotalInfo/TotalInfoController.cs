using AMS_MVC.Models;
using AMS_MVC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers
{
    public partial class TotalInfoController : Controller
    {
        private RiskmatrixRepository _riskRepo = new RiskmatrixRepository();

        // GET: TotalInfo/Index
        public ActionResult Index()
        {
            ViewBag.MenuType = "TotalInfo"; // TopMenu 등록 Event 활성화
            ViewBag.Title = "종합정보";
            return View();
        }
        [HttpPost]
        public JsonResult GetRiskmatrixData(string prefix)
        {
            var repository = new RiskmatrixRepository();
            // 올바른 호출: HI 등급별 건수 집계
            var hiData = repository.GetAggregatedHI(prefix);
            return Json(hiData);
        }
        [HttpPost]
        public JsonResult GetHIList(string prefix)
        {
            // 각 장비의 HI 값(정수)만 순서대로 리스트로 내려줍니다
            var list = _riskRepo.GetHIList(prefix);
            return Json(list);
        }
        [HttpPost]
        public JsonResult GetRiskMatrixPofCof()
        {
            try
            {
                var repository = new RiskmatrixRepository();
                var pofCof = repository.GetRiskMatrixPofCof();
                return Json(pofCof);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetPriorityInfo()
        {
            try
            {
                var priorityRepo = new PriorityInfoRepository();
                var priorityData = priorityRepo.GetPriorityInfo();

                var formattedData = priorityData.Select(item => new
                {
                    item.Priority,
                    item.Sort,
                    item.Code,
                    item.Serial_No,
                    item.Name,
                    Install_Date = item.Install_Date.ToString("yy.MM.dd"), 
                    Operating_Date = item.Operating_Date.ToString("yy.MM.dd"), 
                    item.UsagePeriod,
                    item.Price,
                    item.Rated_V,
                    item.Rated_A,
                    item.Make_Company,
                    item.Writer,
                    item.CoF,
                    item.PoF,
                    item.HI
                }).ToList();

                return Json(formattedData);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetMonthlyMaintenanceData()
        {
            try
            {
                var repo = new MaintenanceRepository();
                var data = repo.GetMonthlyMaintenanceCounts();


                return Json(data);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("TotalInfoController", $"GetMonthlyMaintenanceData Error: {ex.Message}");
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetGojangList()
        {
            try
            {
                var gojangRepo = new GojangRepository();
                var gojangData = gojangRepo.GetGojangAll();

                return Json(gojangData);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("TotalInfoController", $"GetGojangList Error: {ex.Message}");
                return Json(new { error = "데이터를 가져오는 중 오류 발생", details = ex.Message });
            }
        }

        [HttpGet]
        public ActionResult SearchByDateRange(string dateType, DateTime startDate, DateTime endDate)
        {
            if (string.IsNullOrEmpty(dateType))
            {
                return Json(new { error = "dateType이 비어있습니다." }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                // Repository 호출
                var data = _riskRepo.GetDevicesByDateRange(dateType, startDate, endDate);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = "검색 중 오류 발생", details = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Check()
        {
            return View();
        }

        public ActionResult Connection()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetRiskMapPoints()
        {
            var raw = _riskRepo.GetLatestRiskPoints();

            var points = raw.Select(r => {
                string code = r.Code;

                int idx = 0;
                while (idx < code.Length && !char.IsDigit(code[idx]))
                    idx++;
                string prefix = code.Substring(0, idx);

                decimal cofValue = r.Cof;
                decimal pofPercent = r.Pof;

                return new
                {
                    x = cofValue,        
                    y = pofPercent,      
                    name = code,
                    group = prefix
                };
            });

            return Json(points, JsonRequestBehavior.AllowGet);
        }
    }
}