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
            try
            {
                var repository = new RiskmatrixRepository();
                var riskData = repository.GetRiskMatrixPofCof(prefix);
                return Json(riskData);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
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
        public ActionResult Battery()
        {
            return View();
        }

        public ActionResult Check()
        {
            return View();
        }

        public ActionResult Connection()
        {
            return View();
        }
    }
}