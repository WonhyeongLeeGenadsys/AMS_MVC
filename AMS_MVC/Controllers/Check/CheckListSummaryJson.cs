using System.Collections.Generic;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    /// <summary>
    /// 장비별 기존 JSON 필드명을 유지하면서 경량 점검 목록을 반환한다.
    /// </summary>
    internal static class CheckListSummaryJson
    {
        public static JsonResult Create(
            string equipmentType,
            string equipmentCode,
            string codeProperty,
            string fieldPrefix = "CHK")
        {
            var repository = new CheckListSummaryRepository();
            var result = repository.GetList(equipmentType, equipmentCode, out var items);

            if (!result.IsSuccess)
            {
                return Json(new { success = false, message = result.Message });
            }

            var data = new List<Dictionary<string, object>>(items.Count);
            foreach (var item in items)
            {
                data.Add(new Dictionary<string, object>
                {
                    ["Tbl_Idx"] = item.Tbl_Idx,
                    [codeProperty] = item.EquipmentCode,
                    ["Name"] = item.Name ?? string.Empty,
                    ["Serial_No"] = item.Serial_No ?? string.Empty,
                    [$"{fieldPrefix}_Gongsa_Name"] = item.GongsaName,
                    [$"{fieldPrefix}_Weather"] = item.Weather,
                    [$"{fieldPrefix}_Temp"] = item.Temp,
                    [$"{fieldPrefix}_Hum"] = item.Hum,
                    [$"{fieldPrefix}_Company"] = item.Company,
                    [$"{fieldPrefix}_Worker"] = item.Worker,
                    [$"{fieldPrefix}_Manager"] = item.Manager,
                    [$"{fieldPrefix}_Urgent_No"] = item.UrgentNo,
                    [$"{fieldPrefix}_Type"] = item.CheckType,
                    [$"{fieldPrefix}_Start_Date"] = item.StartDate?.ToString("yy.MM.dd"),
                    [$"{fieldPrefix}_End_Date"] = item.EndDate?.ToString("yy.MM.dd")
                });
            }

            return Json(data);
        }

        private static JsonResult Json(object data)
        {
            return new JsonResult
            {
                Data = data,
                JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                MaxJsonLength = int.MaxValue
            };
        }
    }
}
