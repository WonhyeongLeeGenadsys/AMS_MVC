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
        private EquipmentSearchRepository _searchRepo = new EquipmentSearchRepository();

        [HttpGet]
        public ActionResult Battery()
        {
            ViewBag.MenuType = "TotalInfo"; 
            return View("~/Views/TotalInfo/Battery.cshtml");
        }

        [HttpPost]
        public JsonResult SearchBetweenDates(string dateType, DateTime startDate, DateTime endDate)
        {
            if (string.IsNullOrEmpty(dateType))
            {
                return Json(new { success = false, error = "날짜 기준이 지정되지 않았습니다." });
            }

            try
            {
                var data = _searchRepo.SearchAllEquipments(dateType, startDate, endDate);


                var ranked = data
                    .Select(item => new
                    {
                        Category = item.Category,
                        Sort = item.Sort,
                        Code = item.Code,
                        Serial_No = item.Serial_No,
                        Name = item.Name,
                        Install_Date = item.Install_Date != null
                            ? ((DateTime)item.Install_Date).ToString("yy.MM.dd")
                            : "",
                        Operating_Date = item.Operating_Date != null
                            ? ((DateTime)item.Operating_Date).ToString("yy.MM.dd")
                            : "",
                        UsageYears = (item.Operating_Date != null)
                                        ? DateTime.Now.Year - ((DateTime)item.Operating_Date).Year
                                        : 0,
                        Price = item.Price,
                        Install_Place = item.Install_Place,
                        Make_Company = item.Make_Company,
                        CoF = ConvertToFloat(item.CoF),
                        PoF = ConvertToFloat(item.PoF),
                        HI = item.HI,
                        Rated_V = item.Rated_V,
                        Rated_A = item.Rated_A,
                        PriorityKey = ConvertToFloat(item.CoF) + ConvertToFloat(item.PoF)
                    })
                    .OrderByDescending(x => x.PriorityKey)
                    .ThenByDescending(x => x.UsageYears)
                    .Select((item, index) => new
                    {
                        Priority = index + 1,
                        item.Category,
                        item.Sort,
                        item.Code,
                        item.Serial_No,
                        item.Name,
                        item.Install_Date,
                        item.Operating_Date,
                        UsageYears = item.UsageYears,
                        item.Price,
                        item.Install_Place,
                        item.Make_Company,
                        CoF = item.CoF,
                        PoF = item.PoF,
                        HI = item.HI,
                        item.Rated_V,
                        item.Rated_A
                    })
                    .ToList();

                return Json(new { success = true, data = ranked });
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("TotalInfoController", $"SearchBetweenDates Error: {ex.Message}");
                return Json(new { success = false, error = ex.Message });
            }
        }

        private float ConvertToFloat(object val)
        {
            if (val == null)
                return 0f;
            float f;
            if (float.TryParse(val.ToString(), out f))
                return f;
            return 0f;
        }
    }
}