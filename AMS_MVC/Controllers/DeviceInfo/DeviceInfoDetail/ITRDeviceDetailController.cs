
using System.Collections.Generic;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public class ITRDeviceDetailController : Controller
    {
        private readonly ITRBasicInfoRepository itrBasicInfoRepo = new ITRBasicInfoRepository();

        // URL 예: /ITRDeviceDetail/ITRDeviceDetail?itrCode=ITR0000001
        public ActionResult ITRDeviceDetail(string itrCode)
        {
            ViewBag.MenuType = "DeviceInfo";
            if (string.IsNullOrEmpty(itrCode))
            {
                return HttpNotFound("ITR 코드가 제공되지 않았습니다.");
            }

            // RiskMatrix 데이터 처리 (기존 코드)
            var riskMatrixRepo = new RiskmatrixRepository();

            var hiDict = riskMatrixRepo.GetLatestRiskMatrixByCode(itrCode);

            ViewBag.HIDict = hiDict;

            // ITR 기본정보 조회
            var model = itrBasicInfoRepo.GetITRBasicInfoByITRCode(itrCode);
            if (model == null)
            {
                return HttpNotFound("해당 ITR 정보를 찾을 수 없습니다.");
            }

            // 보통점검, 정밀점검 데이터 조회
            var itrChk1Repo = new ITRChk1Repository();
            List<ITRChk1> chk1List;
            var result1 = itrChk1Repo.GetITRChk1ByITRCode(itrCode, out chk1List);
            ViewBag.ITRChk1List = chk1List;

            var itrChk2Repo = new ITRChk2Repository();
            List<ITRChk2> chk2List;
            var result2 = itrChk2Repo.GetITRChk2ByITRCode(itrCode, out chk2List);
            ViewBag.ITRChk2List = chk2List;

            return View("~/Views/Device/ITR/ITRDeviceDetail.cshtml", model);
        }

    }
}
