using AMS_MVC.Models;
using AMS_MVC.Repositories;
using System.Collections.Generic;
using System.Web.Mvc;

namespace AMS_MVC.Controllers.DeviceInfo.DeviceInfoDetail
{
    public class TANKDeviceDetailController : Controller
    {
        private readonly TANKBasicInfoRepository tankBasicInfoRepo = new TANKBasicInfoRepository();

        // URL 예: /TANKDeviceDetail/TANKDeviceDetail?tankCode=TANK0000001
        public ActionResult TANKDeviceDetail(string tankCode)
        {
            ViewBag.MenuType = "DeviceInfo";

            if (string.IsNullOrEmpty(tankCode))
            {
                return HttpNotFound("TANK 코드가 제공되지 않았습니다.");
            }

            // RiskMatrix 데이터 처리 (기존 코드)
            var riskMatrixRepo = new RiskmatrixRepository();

            var hiDict = riskMatrixRepo.GetLatestRiskMatrixByCode(tankCode);
            var matrixDict = riskMatrixRepo.GetRiskMatrixPofCofByCode(tankCode);

            ViewBag.HIDict = hiDict;
            ViewBag.RiskMatrixDict = matrixDict;

            // TANK 기본정보 조회
            var model = tankBasicInfoRepo.GetTANKBasicInfoByCode(tankCode);
            if (model == null)
            {
                return HttpNotFound("해당 TANK 정보를 찾을 수 없습니다.");
            }

            // 보통점검 데이터 조회
            var tankChkRepo = new TANKChkRepository();
            List<TANKChk> chkList;
            var result = tankChkRepo.GetTANKChkByTANKCode(tankCode, out chkList);
            ViewBag.TANKChkList = chkList;

            return View("~/Views/Device/TANK/TANKDeviceDetail.cshtml", model);
        }

    }
}
