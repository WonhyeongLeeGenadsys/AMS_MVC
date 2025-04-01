using AMS_MVC.Models;
using AMS_MVC.Repositories;
using System.Web.Mvc;

namespace AMS_MVC.Controllers.DeviceInfo.DeviceInfoDetail
{
    public class VCBDeviceDetailController : Controller
    {
        private readonly VCBBasicInfoRepository vcbBasicInfoRepo = new VCBBasicInfoRepository();

        // URL 예: /VCBDeviceDetail/VCBDeviceDetail?vcbCode=VCB0000001
        public ActionResult VCBDeviceDetail(string vcbCode)
        {
            if (string.IsNullOrEmpty(vcbCode))
            {
                return HttpNotFound("VCB 코드가 제공되지 않았습니다.");
            }

            // 1) RiskMatrixRepository에서 집계 메서드 호출
            var riskMatrixRepo = new RiskmatrixRepository();

            var hiDict = riskMatrixRepo.GetRiskMatrixByVCBCode(vcbCode);
            var matrixDict = riskMatrixRepo.GetRiskMatrixPofCofByVCBCode(vcbCode);

            ViewBag.HIDict = hiDict;
            ViewBag.RiskMatrixDict = matrixDict;

            // 2) 개별 VCB 기본정보 불러오기
            var model = vcbBasicInfoRepo.GetVCBBasicInfoByCode(vcbCode);
            if (model == null)
            {
                return HttpNotFound("해당 VCB 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Device/VCB/VCBDeviceDetail.cshtml", model);
        }

    }
}
