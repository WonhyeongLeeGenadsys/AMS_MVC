using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class SPAREBasicController : Controller
    {
        public ActionResult SPAREBasicList()
        {
            return View("~/Views/SPARE/Basic/SPAREBasicList.cshtml");
        }

        [HttpPost]
        public ActionResult GetSPAREListData()
        {
            try
            {
                LogHelper.WriteLog("SPAREBasicController.List", "GetSPAREListData 실행");

                if (spareBasicRepository.GetAllSPAREBasicInfoRepo(out var spareParts).IsSuccess)
                {
                    // 부품별 연결 설비유형. 목록에 표시하기 위해 한 번에 조회한 뒤 사전으로 만든다.
                    // 한 부품이 여러 설비유형에 걸릴 수 있으므로 쉼표로 이어 붙인다.
                    spareBasicRepository.GetAllAssetMapsRepo(out var allAssetMaps);
                    var assetTypeNamesBySpareId = allAssetMaps
                        .GroupBy(m => m.SPARE_ID)
                        .ToDictionary(
                            g => g.Key,
                            g => string.Join(", ", g
                                .Select(m => m.ASSET_TYPE_ID)
                                .Distinct()
                                .OrderBy(id => id)
                                .Select(AssetTypeName)));

                    var list = new List<dynamic>();

                    foreach (var item in spareParts)
                    {
                        spareBasicRepository.GetInventoryBySPAREIdRepo(item.SPARE_ID, out var inventory);

                        string assetTypeNames;
                        if (!assetTypeNamesBySpareId.TryGetValue(item.SPARE_ID, out assetTypeNames))
                            assetTypeNames = "미지정";

                        list.Add(new
                        {
                            item.SPARE_ID,
                            item.PART_NUMBER,
                            item.PART_NAME,
                            ASSET_TYPE_NAMES = assetTypeNames,
                            item.CRITICALITY_GRADE,
                            item.UNIT_PRICE,
                            item.LEAD_TIME_DAYS,
                            item.SUPPLIER,
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
