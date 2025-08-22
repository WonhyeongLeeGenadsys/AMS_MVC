using AMS_MVC.Models;
using AMS_MVC.Repositories;
using System.Collections.Generic;
using System.Web.Mvc;

namespace AMS_MVC.Controllers.DeviceInfo.DeviceInfoDetail
{
    public class SADeviceDetailController : Controller
    {
        private readonly SABasicInfoRepository saBasicInfoRepo = new SABasicInfoRepository();

        // URL 예: /SADeviceDetail/SADeviceDetail?saCode=SA0000001
        public ActionResult SADeviceDetail(string saCode)
        {
            ViewBag.MenuType = "DeviceInfo";

            if (string.IsNullOrEmpty(saCode))
            {
                return HttpNotFound("SA 코드가 제공되지 않았습니다.");
            }

            // RiskMatrix 데이터 처리 (기존 코드)
            var riskMatrixRepo = new RiskmatrixRepository();

            var hiDict = riskMatrixRepo.GetLatestRiskMatrixByCode(saCode);

            ViewBag.HIDict = hiDict;

            // SA 기본정보 조회
            var model = saBasicInfoRepo.GetSABasicInfoByCode(saCode);
            if (model == null)
            {
                return HttpNotFound("해당 SA 정보를 찾을 수 없습니다.");
            }

            // 보통점검 데이터 조회
            var saChkRepo = new SAChkRepository();
            List<SAChk> chkList;
            var result = saChkRepo.GetSAChkBySACode(saCode, out chkList);
            ViewBag.SAChkList = chkList;

            return View("~/Views/Device/SA/SADeviceDetail.cshtml", model);
        }

    }
}
