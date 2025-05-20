using AMS_MVC.Models;
using AMS_MVC.Repositories;
using System.Collections.Generic;
using System.Web.Mvc;

namespace AMS_MVC.Controllers.DeviceInfo.DeviceInfoDetail
{
    public class HEATEXCHANGERDeviceDetailController : Controller
    {
        private readonly HEATEXCHANGERBasicInfoRepository heatexchangerBasicInfoRepo = new HEATEXCHANGERBasicInfoRepository();

        // URL 예: /HEATEXCHANGERDeviceDetail/HEATEXCHANGERDeviceDetail?heatexchangerCode=HEATEXCHANGER0000001
        public ActionResult HEATEXCHANGERDeviceDetail(string heatexchangerCode)
        {
            ViewBag.MenuType = "DeviceInfo";

            if (string.IsNullOrEmpty(heatexchangerCode))
            {
                return HttpNotFound("HEATEXCHANGER 코드가 제공되지 않았습니다.");
            }

            // RiskMatrix 데이터 처리 (기존 코드)
            var riskMatrixRepo = new RiskmatrixRepository();

            var hiDict = riskMatrixRepo.GetRiskMatrixByCode(heatexchangerCode);
            var matrixDict = riskMatrixRepo.GetRiskMatrixPofCofByCode(heatexchangerCode);

            ViewBag.HIDict = hiDict;
            ViewBag.RiskMatrixDict = matrixDict;

            // HEATEXCHANGER 기본정보 조회
            var model = heatexchangerBasicInfoRepo.GetHEATEXCHANGERBasicInfoByCode(heatexchangerCode);
            if (model == null)
            {
                return HttpNotFound("해당 HEATEXCHANGER 정보를 찾을 수 없습니다.");
            }

            // 보통점검 데이터 조회
            var heatexchangerChkRepo = new HEATEXCHANGERChkRepository();
            List<HEATEXCHANGERChk> chkList;
            var result = heatexchangerChkRepo.GetHEATEXCHANGERChkByHEATEXCHANGERCode(heatexchangerCode, out chkList);
            ViewBag.HEATEXCHANGERChkList = chkList;

            return View("~/Views/Device/HEATEXCHANGER/HEATEXCHANGERDeviceDetail.cshtml", model);
        }

    }
}
