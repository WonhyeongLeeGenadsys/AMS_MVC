using AMS_MVC.Models;
using AMS_MVC.Repositories;
using System.Collections.Generic;
using System.Web.Mvc;

namespace AMS_MVC.Controllers.DeviceInfo.DeviceInfoDetail
{
    public class SUBMODULEDeviceDetailController : Controller
    {
        private readonly SUBMODULEBasicInfoRepository submoduleBasicInfoRepo = new SUBMODULEBasicInfoRepository();

        // URL 예: /SUBMODULEDeviceDetail/SUBMODULEDeviceDetail?submoduleCode=SUBMODULE0000001
        public ActionResult SUBMODULEDeviceDetail(string submoduleCode)
        {
            ViewBag.MenuType = "DeviceInfo";

            if (string.IsNullOrEmpty(submoduleCode))
            {
                return HttpNotFound("SUBMODULE 코드가 제공되지 않았습니다.");
            }

            // RiskMatrix 데이터 처리 (기존 코드)
            var riskMatrixRepo = new RiskmatrixRepository();

            var hiDict = riskMatrixRepo.GetRiskMatrixByCode(submoduleCode);
            var matrixDict = riskMatrixRepo.GetRiskMatrixPofCofByCode(submoduleCode);

            ViewBag.HIDict = hiDict;
            ViewBag.RiskMatrixDict = matrixDict;

            // SUBMODULE 기본정보 조회
            var model = submoduleBasicInfoRepo.GetSUBMODULEBasicInfoByCode(submoduleCode);
            if (model == null)
            {
                return HttpNotFound("해당 SUBMODULE 정보를 찾을 수 없습니다.");
            }

            // 보통점검 데이터 조회
            var submoduleChkRepo = new SUBMODULEChkRepository();
            List<SUBMODULEChk> chkList;
            var result = submoduleChkRepo.GetSUBMODULEChkBySUBMODULECode(submoduleCode, out chkList);
            ViewBag.SUBMODULEChkList = chkList;

            return View("~/Views/Device/SUBMODULE/SUBMODULEDeviceDetail.cshtml", model);
        }

    }
}
