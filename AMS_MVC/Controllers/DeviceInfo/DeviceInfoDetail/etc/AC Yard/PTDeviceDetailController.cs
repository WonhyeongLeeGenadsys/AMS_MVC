
using System.Collections.Generic;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public class PTDeviceDetailController : Controller
    {
        private readonly PTBasicInfoRepository ptBasicInfoRepo = new PTBasicInfoRepository();

        // URL 예: /PTDeviceDetail/PTDeviceDetail?ptCode=PT0000001
        public ActionResult PTDeviceDetail(string ptCode)
        {
            ViewBag.MenuType = "DeviceInfo";

            if (string.IsNullOrEmpty(ptCode))
            {
                return HttpNotFound("PT 코드가 제공되지 않았습니다.");
            }

            // RiskMatrix 데이터 처리 (기존 코드)
            var riskMatrixRepo = new RiskmatrixRepository();

            var hiDict = riskMatrixRepo.GetRiskMatrixByCode(ptCode);

            ViewBag.HIDict = hiDict;

            // PT 기본정보 조회
            var model = ptBasicInfoRepo.GetPTBasicInfoByCode(ptCode);
            if (model == null)
            {
                return HttpNotFound("해당 PT 정보를 찾을 수 없습니다.");
            }

            // 보통점검 데이터 조회
            var ptChkRepo = new PTChkRepository();
            List<PTChk> chkList;
            var result = ptChkRepo.GetPTChkByPTCode(ptCode, out chkList);
            ViewBag.PTChkList = chkList;

            return View("~/Views/Device/PT/PTDeviceDetail.cshtml", model);
        }

    }
}
