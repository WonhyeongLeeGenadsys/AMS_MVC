using System.Web.Mvc;
using Web.Common;

using System.Collections.Generic;
using System.Linq;

namespace AMS_MVC
{
    public partial class SPAREBasicController : Controller
    {
        private readonly SPAREBasicInfoRepository spareBasicRepository;

        public SPAREBasicController()
        {
            ViewBag.MenuType = "SPARE";
            spareBasicRepository = new SPAREBasicInfoRepository();
        }

        // ASSET_TYPE_ID(1~5) → 화면 표기명.
        // 대시보드 SQL의 CASE 매핑과 동일한 순서를 쓴다.
        // ASSET_TYPE_ID → 화면 표기명.
        // 설비유형은 13종이다. SPAREBasicDetail.cshtml의 매핑과 동일하게 유지할 것.
        public static string AssetTypeName(int assetTypeId)
        {
            switch (assetTypeId)
            {
                case 1: return "VCB";
                case 2: return "DCCB";
                case 3: return "Sub Module";
                case 4: return "DC Cable";
                case 5: return "Interface TR";
                case 6: return "Converter";
                case 7: return "Circuit Breaker";
                case 8: return "Cable";
                case 9: return "Switchgear";
                case 10: return "Protection Relay";
                case 11: return "Cooling System";
                case 12: return "Energy Storage";
                case 13: return "SCADA";
                default: return "미지정";
            }
        }

        private static bool TryBuildAssetMaps(
            List<int> assetTypeIds,
            List<int> requiredQtys,
            out List<SpareAssetMapInfo> assetMaps,
            out string errorMessage)
        {
            assetMaps = new List<SpareAssetMapInfo>();
            errorMessage = null;

            if (assetTypeIds == null || assetTypeIds.Count == 0)
            {
                errorMessage = "연결 설비유형을 한 개 이상 선택하세요.";
                return false;
            }

            if (requiredQtys == null || requiredQtys.Count != assetTypeIds.Count)
            {
                errorMessage = "선택한 설비유형의 필요수량을 입력하세요.";
                return false;
            }

            if (assetTypeIds.Any(x => x < 1 || x > 5) ||
                assetTypeIds.Distinct().Count() != assetTypeIds.Count)
            {
                errorMessage = "연결 설비유형 정보가 올바르지 않습니다.";
                return false;
            }

            if (requiredQtys.Any(x => x < 1))
            {
                errorMessage = "필요수량은 1개 이상으로 입력하세요.";
                return false;
            }

            assetMaps = assetTypeIds
                .Select((assetTypeId, index) => new SpareAssetMapInfo
                {
                    ASSET_TYPE_ID = assetTypeId,
                    REQUIRED_QTY = requiredQtys[index]
                })
                .ToList();

            return true;
        }
    }
}
