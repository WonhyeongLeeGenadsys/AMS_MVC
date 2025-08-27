using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AMS_MVC
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
            // 모든 이력 가져옴 (Code 별 그룹화)
            var historyByCode = _riskRepo.GetRiskMatrixHistory(prefix)
                .GroupBy(r => r.Code)
                .ToDictionary(g => g.Key, g => g.OrderBy(r => r.LastTime).ToList());

            var result = new List<object>();

            //  Code 별로 Before/After 비교
            foreach (var kv in historyByCode)
            {
                var code = kv.Key;
                var records = kv.Value;

                if (records.Count == 0)
                    continue;

                var before = records.Count >= 2 ? records[records.Count - 2] : records[0];
                var after = records[records.Count - 1];

                //wjs
                var beforeObj = new
                {
                    Cof = before.Cof,
                    Pof = before.Pof,
                    Hi = int.Parse(before.HI)
                };

                //후
                var afterObj = new
                {
                    Cof = after.Cof,
                    Pof = after.Pof,
                    Hi = int.Parse(after.HI)
                };

                // 변화가 있는 경우만 저장
                bool hasChanged = beforeObj.Cof != afterObj.Cof ||
                                  beforeObj.Pof != afterObj.Pof ||
                                  beforeObj.Hi != afterObj.Hi;

                if (hasChanged)
                {
                    result.Add(new
                    {
                        Code = code,
                        Before = beforeObj,
                        After = afterObj,
                        BeforeTime = before.LastTime,
                        AfterTime = after.LastTime
                    });
                }
            }

            // 최신순 정렬
            var sorted = result
                .OrderByDescending(x => ((dynamic)x).AfterTime)
                .ToList();

            return Json(sorted, JsonRequestBehavior.AllowGet);
        }
    }
}
