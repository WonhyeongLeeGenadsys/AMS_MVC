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

        /// <summary>
        /// 전체 5대장비, AC(VCB, ITR), DC(DCCB, DCCABLE, SUBMODULE) 3가지 분류!
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetRiskmatrixData(string prefix)
        {
            // Determine which equipment prefixes to include
            string[] codePrefixes;
            if (string.IsNullOrEmpty(prefix))
                codePrefixes = new[] { "VCB", "ITR", "DCCB", "DCCABLE", "SUBMODULE" };
            else if (prefix == "AC")
                codePrefixes = new[] { "VCB", "ITR" };
            else if (prefix == "DC")
                codePrefixes = new[] { "DCCB", "DCCABLE", "SUBMODULE" };
            else
                codePrefixes = new[] { prefix };

            var hiData = _riskRepo.GetAggregatedHI(codePrefixes);
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

        /// <summary>
        /// riskmatrix 값 전체, AC, DC 구별
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetRiskMapPoints(string prefix = "all")
        {
            var raw = _riskRepo.GetLatestRiskPoints();
            IEnumerable<Riskmatrix> filtered;

            prefix = (prefix ?? "").ToLower();
            if (prefix == "ac")
            {
                filtered = raw.Where(r => 
                r.Code.StartsWith("VCB") || 
                r.Code.StartsWith("ITR"));
            }

            else if (prefix == "dc")
            {
                filtered = raw.Where(r =>
                    r.Code.StartsWith("DCCB") ||
                    r.Code.StartsWith("DCCABLE") ||
                    r.Code.StartsWith("SUBMODULE"));
            }

            else
            {
                filtered = raw;
            }

            var points = filtered.Select(r => new {
                x = r.Cof,
                y = r.Pof,
                name = r.Code,
                group = new string(r.Code.TakeWhile(c => !char.IsDigit(c)).ToArray())
            });

            return Json(points, JsonRequestBehavior.AllowGet);
        }



    }
}