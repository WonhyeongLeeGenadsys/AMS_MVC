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

        [HttpGet]
        public ActionResult Recent()
        {
            ViewBag.MenuType = "TotalInfo";
            return View("~/Views/TotalInfo/Recent.cshtml");
        }

        [HttpGet]
        public JsonResult GetRecentActivity(string prefix = "")
        {
            var allHist = _riskRepo.GetRiskMatrixHistory(prefix)
                            .GroupBy(r => r.Code)
                            .ToDictionary(
                                g => g.Key,
                                g => g.OrderBy(r => r.LastTime).ToList()
                            );

            var withChanges = allHist
                .Select(kv =>
                {
                    var list = kv.Value;
                    var before = list.Count >= 2 ? list[list.Count - 2] : list[0];
                    var after = list[list.Count - 1];

                    var beforeObj = new { Cof = before.Cof, Pof = before.Pof, Hi = int.Parse(before.HI) };
                    var afterObj = new { Cof = after.Cof, Pof = after.Pof, Hi = int.Parse(after.HI) };

                    var hasChanged =
                        beforeObj.Hi != afterObj.Hi ||
                        beforeObj.Cof != afterObj.Cof ||
                        beforeObj.Pof != afterObj.Pof;

                    return new
                    {
                        Code = kv.Key,
                        Before = beforeObj,
                        After = afterObj,
                        BeforeTime = before.LastTime,
                        AfterTime = after.LastTime,
                        HasChanged = hasChanged
                    };
                })
                .Where(x => x.HasChanged)
                .OrderByDescending(x => x.AfterTime)
                .Select(x => new
                {
                    Code = x.Code,
                    Before = x.Before,
                    After = x.After,
                    BeforeTime = x.BeforeTime, 
            AfterTime = x.AfterTime
                })
                .ToList();

            return Json(withChanges, JsonRequestBehavior.AllowGet);
        }
    }
}