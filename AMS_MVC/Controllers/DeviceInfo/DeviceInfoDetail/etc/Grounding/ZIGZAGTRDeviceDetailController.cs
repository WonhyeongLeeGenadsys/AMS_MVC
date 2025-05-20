using AMS_MVC.Models;
using AMS_MVC.Repositories;
using System.Collections.Generic;
using System.Web.Mvc;

namespace AMS_MVC.Controllers.DeviceInfo.DeviceInfoDetail
{
    public class ZIGZAGTRDeviceDetailController : Controller
    {
        private readonly ZIGZAGTRBasicInfoRepository zigzagtrBasicInfoRepo = new ZIGZAGTRBasicInfoRepository();

        // URL 예: /ZIGZAGTRDeviceDetail/ZIGZAGTRDeviceDetail?zigzagtrCode=ZIGZAGTR0000001
        public ActionResult ZIGZAGTRDeviceDetail(string zigzagtrCode)
        {
            ViewBag.MenuType = "DeviceInfo";

            if (string.IsNullOrEmpty(zigzagtrCode))
            {
                return HttpNotFound("ZIGZAGTR 코드가 제공되지 않았습니다.");
            }

            // RiskMatrix 데이터 처리 (기존 코드)
            var riskMatrixRepo = new RiskmatrixRepository();

            var hiDict = riskMatrixRepo.GetRiskMatrixByCode(zigzagtrCode);
            var matrixDict = riskMatrixRepo.GetRiskMatrixPofCofByCode(zigzagtrCode);

            ViewBag.HIDict = hiDict;
            ViewBag.RiskMatrixDict = matrixDict;

            // ZIGZAGTR 기본정보 조회
            var model = zigzagtrBasicInfoRepo.GetZIGZAGTRBasicInfoByCode(zigzagtrCode);
            if (model == null)
            {
                return HttpNotFound("해당 ZIGZAGTR 정보를 찾을 수 없습니다.");
            }

            // 보통점검 데이터 조회
            var zigzagtrChkRepo = new ZIGZAGTRChkRepository();
            List<ZIGZAGTRChk> chkList;
            var result = zigzagtrChkRepo.GetZIGZAGTRChkByZIGZAGTRCode(zigzagtrCode, out chkList);
            ViewBag.ZIGZAGTRChkList = chkList;

            return View("~/Views/Device/ZIGZAGTR/ZIGZAGTRDeviceDetail.cshtml", model);
        }

    }
}
