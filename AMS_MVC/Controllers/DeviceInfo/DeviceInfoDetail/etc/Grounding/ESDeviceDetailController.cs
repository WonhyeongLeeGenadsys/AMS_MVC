using AMS_MVC.Models;
using AMS_MVC.Repositories;
using System.Collections.Generic;
using System.Web.Mvc;

namespace AMS_MVC.Controllers.DeviceInfo.DeviceInfoDetail
{
    public class ESDeviceDetailController : Controller
    {
        private readonly ESBasicInfoRepository esBasicInfoRepo = new ESBasicInfoRepository();

        // URL 예: /ESDeviceDetail/ESDeviceDetail?esCode=ES0000001
        public ActionResult ESDeviceDetail(string esCode)
        {
            ViewBag.MenuType = "DeviceInfo";

            if (string.IsNullOrEmpty(esCode))
            {
                return HttpNotFound("ES 코드가 제공되지 않았습니다.");
            }

            // RiskMatrix 데이터 처리 (기존 코드)
            var riskMatrixRepo = new RiskmatrixRepository();

            var hiDict = riskMatrixRepo.GetRiskMatrixByCode(esCode);
            var matrixDict = riskMatrixRepo.GetRiskMatrixPofCofByCode(esCode);

            ViewBag.HIDict = hiDict;
            ViewBag.RiskMatrixDict = matrixDict;

            // ES 기본정보 조회
            var model = esBasicInfoRepo.GetESBasicInfoByCode(esCode);
            if (model == null)
            {
                return HttpNotFound("해당 ES 정보를 찾을 수 없습니다.");
            }

            // 보통점검 데이터 조회
            var esChkRepo = new ESChkRepository();
            List<ESChk> chkList;
            var result = esChkRepo.GetESChkByESCode(esCode, out chkList);
            ViewBag.ESChkList = chkList;

            return View("~/Views/Device/ES/ESDeviceDetail.cshtml", model);
        }

    }
}
