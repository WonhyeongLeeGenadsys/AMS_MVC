
using System.Collections.Generic;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public class DSDeviceDetailController : Controller
    {
        private readonly DSBasicInfoRepository dsBasicInfoRepo = new DSBasicInfoRepository();

        // URL 예: /DSDeviceDetail/DSDeviceDetail?dsCode=DS0000001
        public ActionResult DSDeviceDetail(string dsCode)
        {
            ViewBag.MenuType = "DeviceInfo";

            if (string.IsNullOrEmpty(dsCode))
            {
                return HttpNotFound("DS 코드가 제공되지 않았습니다.");
            }

            // RiskMatrix 데이터 처리 (기존 코드)
            var riskMatrixRepo = new RiskmatrixRepository();

            var hiDict = riskMatrixRepo.GetLatestRiskMatrixByCode(dsCode);

            ViewBag.HIDict = hiDict;

            // DS 기본정보 조회
            var model = dsBasicInfoRepo.GetDSBasicInfoByCode(dsCode);
            if (model == null)
            {
                return HttpNotFound("해당 DS 정보를 찾을 수 없습니다.");
            }

            // 보통점검 데이터 조회
            var dsChkRepo = new DSChkRepository();
            List<DSChk> chkList;
            var result = dsChkRepo.GetDSChkByDSCode(dsCode, out chkList);
            ViewBag.DSChkList = chkList;

            return View("~/Views/Device/DS/DSDeviceDetail.cshtml", model);
        }

    }
}
