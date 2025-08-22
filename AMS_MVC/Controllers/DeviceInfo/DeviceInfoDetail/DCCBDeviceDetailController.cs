using AMS_MVC.Models;
using AMS_MVC.Repositories;
using System.Collections.Generic;
using System.Web.Mvc;

namespace AMS_MVC.Controllers.DeviceInfo.DeviceInfoDetail
{
    public class DCCBDeviceDetailController : Controller
    {
        private readonly DCCBBasicInfoRepository dccbBasicInfoRepo = new DCCBBasicInfoRepository();

        // URL 예: /DCCBDeviceDetail/DCCBDeviceDetail?dccbCode=DCCB0000001
        public ActionResult DCCBDeviceDetail(string dccbCode)
        {
            ViewBag.MenuType = "DeviceInfo";

            if (string.IsNullOrEmpty(dccbCode))
            {
                return HttpNotFound("DCCB 코드가 제공되지 않았습니다.");
            }

            // RiskMatrix 데이터 처리 (기존 코드)
            var riskMatrixRepo = new RiskmatrixRepository();

            var hiDict = riskMatrixRepo.GetRiskMatrixByCode(dccbCode);

            ViewBag.HIDict = hiDict;

            // DCCB 기본정보 조회
            var model = dccbBasicInfoRepo.GetDCCBBasicInfoByCode(dccbCode);
            if (model == null)
            {
                return HttpNotFound("해당 DCCB 정보를 찾을 수 없습니다.");
            }

            // 보통점검 데이터 조회
            var dccbChkRepo = new DCCBChkRepository();
            List<DCCBChk> chkList;
            var result = dccbChkRepo.GetDCCBChkByDCCBCode(dccbCode, out chkList);
            ViewBag.DCCBChkList = chkList;

            return View("~/Views/Device/DCCB/DCCBDeviceDetail.cshtml", model);
        }

    }
}
