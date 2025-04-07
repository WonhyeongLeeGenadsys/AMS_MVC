using AMS_MVC.Models;
using AMS_MVC.Repositories;
using System.Collections.Generic;
using System.Web.Mvc;

namespace AMS_MVC.Controllers.DeviceInfo.DeviceInfoDetail
{
    public class DCCABLEDeviceDetailController : Controller
    {
        private readonly DCCABLEBasicInfoRepository dccableBasicInfoRepo = new DCCABLEBasicInfoRepository();

        // URL 예: /DCCABLEDeviceDetail/DCCABLEDeviceDetail?dccableCode=DCCABLE0000001
        public ActionResult DCCABLEDeviceDetail(string dccableCode)
        {
            if (string.IsNullOrEmpty(dccableCode))
            {
                return HttpNotFound("DCCABLE 코드가 제공되지 않았습니다.");
            }

            // RiskMatrix 데이터 처리 (기존 코드)
            var riskMatrixRepo = new RiskmatrixRepository();

            var hiDict = riskMatrixRepo.GetRiskMatrixByCode(dccableCode);
            var matrixDict = riskMatrixRepo.GetRiskMatrixPofCofByCode(dccableCode);

            ViewBag.HIDict = hiDict;
            ViewBag.RiskMatrixDict = matrixDict;

            // DCCABLE 기본정보 조회
            var model = dccableBasicInfoRepo.GetDCCABLEBasicInfoByCode(dccableCode);
            if (model == null)
            {
                return HttpNotFound("해당 DCCABLE 정보를 찾을 수 없습니다.");
            }

            // 보통점검 데이터 조회
            var dccableChkRepo = new DCCABLEChkRepository();
            List<DCCABLEChk> chkList;
            var result = dccableChkRepo.GetDCCABLEChkByDCCABLECode(dccableCode, out chkList);
            ViewBag.DCCABLEChkList = chkList;

            return View("~/Views/Device/DCCABLE/DCCABLEDeviceDetail.cshtml", model);
        }

    }
}
