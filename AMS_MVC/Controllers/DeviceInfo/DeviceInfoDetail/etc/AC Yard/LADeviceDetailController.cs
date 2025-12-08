
using System.Collections.Generic;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public class LADeviceDetailController : Controller
    {
        private readonly LABasicInfoRepository laBasicInfoRepo = new LABasicInfoRepository();

        // URL 예: /LADeviceDetail/LADeviceDetail?laCode=LA0000001
        public ActionResult LADeviceDetail(string laCode)
        {
            ViewBag.MenuType = "DeviceInfo";

            if (string.IsNullOrEmpty(laCode))
            {
                return HttpNotFound("LA 코드가 제공되지 않았습니다.");
            }

            // RiskMatrix 데이터 처리 (기존 코드)
            var riskMatrixRepo = new RiskmatrixRepository();

            var hiDict = riskMatrixRepo.GetRiskMatrixByCode(laCode);

            ViewBag.HIDict = hiDict;

            // LA 기본정보 조회
            var model = laBasicInfoRepo.GetLABasicInfoByCode(laCode);
            if (model == null)
            {
                return HttpNotFound("해당 LA 정보를 찾을 수 없습니다.");
            }

            // 보통점검 데이터 조회
            var laChkRepo = new LAChkRepository();
            List<LAChk> chkList;
            var result = laChkRepo.GetLAChkByLACode(laCode, out chkList);
            ViewBag.LAChkList = chkList;

            return View("~/Views/Device/LA/LADeviceDetail.cshtml", model);
        }

    }
}
