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
