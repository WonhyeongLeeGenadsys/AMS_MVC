using AMS_MVC.Models;
using AMS_MVC.Repositories;
using System.Collections.Generic;
using System.Web.Mvc;

namespace AMS_MVC.Controllers.DeviceInfo.DeviceInfoDetail
{
    public class PUMPDeviceDetailController : Controller
    {
        private readonly PUMPBasicInfoRepository pumpBasicInfoRepo = new PUMPBasicInfoRepository();

        // URL 예: /PUMPDeviceDetail/PUMPDeviceDetail?pumpCode=PUMP0000001
        public ActionResult PUMPDeviceDetail(string pumpCode)
        {
            ViewBag.MenuType = "DeviceInfo";

            if (string.IsNullOrEmpty(pumpCode))
            {
                return HttpNotFound("PUMP 코드가 제공되지 않았습니다.");
            }

            // RiskMatrix 데이터 처리 (기존 코드)
            var riskMatrixRepo = new RiskmatrixRepository();

            var hiDict = riskMatrixRepo.GetRiskMatrixByCode(pumpCode);

            ViewBag.HIDict = hiDict;

            // PUMP 기본정보 조회
            var model = pumpBasicInfoRepo.GetPUMPBasicInfoByCode(pumpCode);
            if (model == null)
            {
                return HttpNotFound("해당 PUMP 정보를 찾을 수 없습니다.");
            }

            // 보통점검 데이터 조회
            var pumpChkRepo = new PUMPChkRepository();
            List<PUMPChk> chkList;
            var result = pumpChkRepo.GetPUMPChkByPUMPCode(pumpCode, out chkList);
            ViewBag.PUMPChkList = chkList;

            return View("~/Views/Device/PUMP/PUMPDeviceDetail.cshtml", model);
        }

    }
}
