using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class SPAREBasicController : Controller
    {
        [HttpGet]
        public ActionResult SPAREBasicAdd()
        {
            return View("~/Views/SPARE/SPAREBasicAdd.cshtml");
        }

        [HttpPost]
        public ActionResult SPAREBasicAdd(
            SPAREPartInfo model,
            int current_qty = 0,
            int safety_stock = 0,
            int? eoq = null,
            int? reorder_point = null,
            List<int> asset_type_ids = null,
            int? order_qty = null,
            decimal? unit_cost = null,
            DateTime? order_date = null,
            string status = null,
            string supplier = null,
            int? fiscal_year = null,
            decimal? budget_amount = null,
            decimal? actual_amount = null)
        {
            if (string.IsNullOrWhiteSpace(model.PART_NUMBER))
                return Json(new { success = false, message = "부품번호를 입력하세요." });

            if (string.IsNullOrWhiteSpace(model.PART_NAME))
                return Json(new { success = false, message = "부품명을 입력하세요." });

            var inventory = new InventoryInfo
            {
                CURRENT_QTY = current_qty,
                SAFETY_STOCK = safety_stock,
                EOQ = eoq,
                REORDER_POINT = reorder_point
            };

            var procurement = new ProcurementInfo
            {
                ORDER_QTY = order_qty,
                UNIT_COST = unit_cost,
                ORDER_DATE = order_date,
                STATUS = status,
                SUPPLIER = supplier
            };

            var cost = new CostManagementInfo
            {
                FISCAL_YEAR = fiscal_year,
                BUDGET_AMOUNT = budget_amount,
                ACTUAL_AMOUNT = actual_amount
            };

            var result = spareBasicRepository.CreateSPAREBasicInfoRepo(
                model,
                inventory,
                asset_type_ids ?? new List<int>(),
                procurement,
                cost);

            return Json(new
            {
                success = result.IsSuccess,
                message = result.Message
            });
        }
    }
}