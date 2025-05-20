using AMS_MVC.Models;
using AMS_MVC.Repositories;
using System.Collections.Generic;
using System.Web.Mvc;

namespace AMS_MVC.Controllers.DeviceInfo.DeviceInfoDetail
{
    public class WALLBUSHINGDeviceDetailController : Controller
    {
        private readonly WALLBUSHINGBasicInfoRepository wallbushingBasicInfoRepo = new WALLBUSHINGBasicInfoRepository();

        // URL 예: /WALLBUSHINGDeviceDetail/WALLBUSHINGDeviceDetail?wallbushingCode=WALLBUSHING0000001
        public ActionResult WALLBUSHINGDeviceDetail(string wallbushingCode)
        {
            ViewBag.MenuType = "DeviceInfo";

            if (string.IsNullOrEmpty(wallbushingCode))
            {
                return HttpNotFound("WALLBUSHING 코드가 제공되지 않았습니다.");
            }

            // RiskMatrix 데이터 처리 (기존 코드)
            var riskMatrixRepo = new RiskmatrixRepository();

            var hiDict = riskMatrixRepo.GetRiskMatrixByCode(wallbushingCode);
            var matrixDict = riskMatrixRepo.GetRiskMatrixPofCofByCode(wallbushingCode);

            ViewBag.HIDict = hiDict;
            ViewBag.RiskMatrixDict = matrixDict;

            // WALLBUSHING 기본정보 조회
            var model = wallbushingBasicInfoRepo.GetWALLBUSHINGBasicInfoByCode(wallbushingCode);
            if (model == null)
            {
                return HttpNotFound("해당 WALLBUSHING 정보를 찾을 수 없습니다.");
            }

            // 보통점검 데이터 조회
            var wallbushingChkRepo = new WALLBUSHINGChkRepository();
            List<WALLBUSHINGChk> chkList;
            var result = wallbushingChkRepo.GetWALLBUSHINGChkByWALLBUSHINGCode(wallbushingCode, out chkList);
            ViewBag.WALLBUSHINGChkList = chkList;

            return View("~/Views/Device/WALLBUSHING/WALLBUSHINGDeviceDetail.cshtml", model);
        }

    }
}
