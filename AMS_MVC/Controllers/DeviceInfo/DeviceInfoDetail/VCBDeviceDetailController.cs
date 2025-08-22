using AMS_MVC.Models;
using AMS_MVC.Repositories;
using System.Collections.Generic;
using System.Web.Mvc;

namespace AMS_MVC.Controllers.DeviceInfo.DeviceInfoDetail
{
    public class VCBDeviceDetailController : Controller
    {
        private readonly VCBBasicInfoRepository vcbBasicInfoRepo = new VCBBasicInfoRepository();

        // URL 예: /VCBDeviceDetail/VCBDeviceDetail?vcbCode=VCB0000001
        public ActionResult VCBDeviceDetail(string vcbCode)
        {
            ViewBag.MenuType = "DeviceInfo";

            if (string.IsNullOrEmpty(vcbCode))
            {
                return HttpNotFound("VCB 코드가 제공되지 않았습니다.");
            }

            // RiskMatrix 데이터 처리 (기존 코드)
            var riskMatrixRepo = new RiskmatrixRepository();

            var hiDict = riskMatrixRepo.GetLatestRiskMatrixByCode(vcbCode);

            ViewBag.HIDict = hiDict;

            // VCB 기본정보 조회
            var model = vcbBasicInfoRepo.GetVCBBasicInfoByCode(vcbCode);
            if (model == null)
            {
                return HttpNotFound("해당 VCB 정보를 찾을 수 없습니다.");
            }

            // 보통점검 데이터 조회
            var vcbChkRepo = new VCBChkRepository();
            List<VCBChk> chkList;
            var result = vcbChkRepo.GetVCBChkByVCBCode(vcbCode, out chkList);
            ViewBag.VCBChkList = chkList;

            return View("~/Views/Device/VCB/VCBDeviceDetail.cshtml", model);
        }

    }
}
