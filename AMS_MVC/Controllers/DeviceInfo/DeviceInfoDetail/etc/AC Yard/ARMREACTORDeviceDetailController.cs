using AMS_MVC.Models;
using AMS_MVC.Repositories;
using System.Collections.Generic;
using System.Web.Mvc;

namespace AMS_MVC.Controllers.DeviceInfo.DeviceInfoDetail
{
    public class ARMREACTORDeviceDetailController : Controller
    {
        private readonly ARMREACTORBasicInfoRepository armreactorBasicInfoRepo = new ARMREACTORBasicInfoRepository();

        // URL 예: /ARMREACTORDeviceDetail/ARMREACTORDeviceDetail?armreactorCode=ARMREACTOR0000001
        public ActionResult ARMREACTORDeviceDetail(string armreactorCode)
        {
            ViewBag.MenuType = "DeviceInfo";

            if (string.IsNullOrEmpty(armreactorCode))
            {
                return HttpNotFound("ARMREACTOR 코드가 제공되지 않았습니다.");
            }

            // RiskMatrix 데이터 처리 (기존 코드)
            var riskMatrixRepo = new RiskmatrixRepository();

            var hiDict = riskMatrixRepo.GetRiskMatrixByCode(armreactorCode);

            ViewBag.HIDict = hiDict;

            // ARMREACTOR 기본정보 조회
            var model = armreactorBasicInfoRepo.GetARMREACTORBasicInfoByCode(armreactorCode);
            if (model == null)
            {
                return HttpNotFound("해당 ARMREACTOR 정보를 찾을 수 없습니다.");
            }

            // 보통점검 데이터 조회
            var armreactorChkRepo = new ARMREACTORChkRepository();
            List<ARMREACTORChk> chkList;
            var result = armreactorChkRepo.GetARMREACTORChkByARMREACTORCode(armreactorCode, out chkList);
            ViewBag.ARMREACTORChkList = chkList;

            return View("~/Views/Device/ARMREACTOR/ARMREACTORDeviceDetail.cshtml", model);
        }

    }
}
