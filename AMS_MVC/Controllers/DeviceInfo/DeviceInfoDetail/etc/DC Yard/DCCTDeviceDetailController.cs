
using System.Collections.Generic;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public class DCCTDeviceDetailController : Controller
    {
        private readonly DCCTBasicInfoRepository dcctBasicInfoRepo = new DCCTBasicInfoRepository();

        // URL 예: /DCCTDeviceDetail/DCCTDeviceDetail?dcctCode=DCCT0000001
        public ActionResult DCCTDeviceDetail(string dcctCode)
        {
            ViewBag.MenuType = "DeviceInfo";

            if (string.IsNullOrEmpty(dcctCode))
            {
                return HttpNotFound("DCCT 코드가 제공되지 않았습니다.");
            }

            // RiskMatrix 데이터 처리 (기존 코드)
            var riskMatrixRepo = new RiskmatrixRepository();

            var hiDict = riskMatrixRepo.GetRiskMatrixByCode(dcctCode);

            ViewBag.HIDict = hiDict;

            // DCCT 기본정보 조회
            var model = dcctBasicInfoRepo.GetDCCTBasicInfoByCode(dcctCode);
            if (model == null)
            {
                return HttpNotFound("해당 DCCT 정보를 찾을 수 없습니다.");
            }

            // 보통점검 데이터 조회
            var dcctChkRepo = new DCCTChkRepository();
            List<DCCTChk> chkList;
            var result = dcctChkRepo.GetDCCTChkByDCCTCode(dcctCode, out chkList);
            ViewBag.DCCTChkList = chkList;

            return View("~/Views/Device/DCCT/DCCTDeviceDetail.cshtml", model);
        }

    }
}
