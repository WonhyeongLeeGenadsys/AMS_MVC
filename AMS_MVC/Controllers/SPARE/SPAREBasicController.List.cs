using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class SPAREBasicController : Controller
    {
        public ActionResult SPAREBasicList()
        {
            return View("~/Views/SPARE/SPAREBasicList.cshtml");
        }

        [HttpPost]
        public ActionResult GetSPAREListData()
        {
            try
            {
                LogHelper.WriteLog("SPAREBasicController.List", "GetSPAREListData 실행");

                if (spareBasicRepository.GetAllSPAREBasicInfoRepo(out var spareParts).IsSuccess)
                {
                    var list = new List<dynamic>();

                    foreach (var item in spareParts)
                    {
                        spareBasicRepository.GetInventoryBySPAREIdRepo(item.SPARE_ID, out var inventory);

                        list.Add(new
                        {
                            item.SPARE_ID,
                            item.PART_NUMBER,
                            item.PART_NAME,
                            item.CRITICALITY_GRADE,
                            item.UNIT_PRICE,
                            item.LEAD_TIME_DAYS,
                            CURRENT_QTY = inventory != null ? inventory.CURRENT_QTY : 0,
                            IS_ACTIVE = item.IS_ACTIVE.HasValue && item.IS_ACTIVE.Value ? "사용" : "미사용",
                            CREATED_AT = item.CREATED_AT.HasValue ? item.CREATED_AT.Value.ToString("yy.MM.dd") : ""
                        });
                    }

                    return Json(new { success = true, data = list });
                }

                return Json(new { success = false, message = "예비품 데이터 로드 실패" });
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("SPAREBasicController.List", $"GetSPAREListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}