using AMS_MVC.Models;
using AMS_MVC.Repositories;
using System.Collections.Generic;
using System.Web.Mvc;

namespace AMS_MVC.Controllers.DeviceInfo.DeviceInfoDetail
{
    public class CTDeviceDetailController : Controller
    {
        private readonly CTBasicInfoRepository ctBasicInfoRepo = new CTBasicInfoRepository();

        // URL 예: /CTDeviceDetail/CTDeviceDetail?ctCode=CT0000001
        public ActionResult CTDeviceDetail(string ctCode)
        {
            ViewBag.MenuType = "DeviceInfo";

            if (string.IsNullOrEmpty(ctCode))
            {
                return HttpNotFound("CT 코드가 제공되지 않았습니다.");
            }

            // RiskMatrix 데이터 처리 (기존 코드)
            var riskMatrixRepo = new RiskmatrixRepository();

            var hiDict = riskMatrixRepo.GetRiskMatrixByCode(ctCode);
            var matrixDict = riskMatrixRepo.GetRiskMatrixPofCofByCode(ctCode);

            ViewBag.HIDict = hiDict;
            ViewBag.RiskMatrixDict = matrixDict;

            // CT 기본정보 조회
            var model = ctBasicInfoRepo.GetCTBasicInfoByCode(ctCode);
            if (model == null)
            {
                return HttpNotFound("해당 CT 정보를 찾을 수 없습니다.");
            }

            // 보통점검 데이터 조회
            var ctChkRepo = new CTChkRepository();
            List<CTChk> chkList;
            var result = ctChkRepo.GetCTChkByCTCode(ctCode, out chkList);
            ViewBag.CTChkList = chkList;

            return View("~/Views/Device/CT/CTDeviceDetail.cshtml", model);
        }

    }
}
