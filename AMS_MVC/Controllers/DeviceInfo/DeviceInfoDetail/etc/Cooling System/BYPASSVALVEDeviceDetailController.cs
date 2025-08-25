
using System.Collections.Generic;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public class BYPASSVALVEDeviceDetailController : Controller
    {
        private readonly BYPASSVALVEBasicInfoRepository bypassvalveBasicInfoRepo = new BYPASSVALVEBasicInfoRepository();

        // URL 예: /BYPASSVALVEDeviceDetail/BYPASSVALVEDeviceDetail?bypassvalveCode=BYPASSVALVE0000001
        public ActionResult BYPASSVALVEDeviceDetail(string bypassvalveCode)
        {
            ViewBag.MenuType = "DeviceInfo";

            if (string.IsNullOrEmpty(bypassvalveCode))
            {
                return HttpNotFound("BYPASSVALVE 코드가 제공되지 않았습니다.");
            }

            // RiskMatrix 데이터 처리 (기존 코드)
            var riskMatrixRepo = new RiskmatrixRepository();

            var hiDict = riskMatrixRepo.GetRiskMatrixByCode(bypassvalveCode);

            ViewBag.HIDict = hiDict;

            // BYPASSVALVE 기본정보 조회
            var model = bypassvalveBasicInfoRepo.GetBYPASSVALVEBasicInfoByCode(bypassvalveCode);
            if (model == null)
            {
                return HttpNotFound("해당 BYPASSVALVE 정보를 찾을 수 없습니다.");
            }

            // 보통점검 데이터 조회
            var bypassvalveChkRepo = new BYPASSVALVEChkRepository();
            List<BYPASSVALVEChk> chkList;
            var result = bypassvalveChkRepo.GetBYPASSVALVEChkByBYPASSVALVECode(bypassvalveCode, out chkList);
            ViewBag.BYPASSVALVEChkList = chkList;

            return View("~/Views/Device/BYPASSVALVE/BYPASSVALVEDeviceDetail.cshtml", model);
        }

    }
}
