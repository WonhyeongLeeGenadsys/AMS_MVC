using AMS_MVC.Models;
using AMS_MVC.Repositories;
using System.Collections.Generic;
using System.Web.Mvc;

namespace AMS_MVC.Controllers.DeviceInfo.DeviceInfoDetail
{
    public class NGRDeviceDetailController : Controller
    {
        private readonly NGRBasicInfoRepository ngrBasicInfoRepo = new NGRBasicInfoRepository();

        // URL 예: /NGRDeviceDetail/NGRDeviceDetail?ngrCode=NGR0000001
        public ActionResult NGRDeviceDetail(string ngrCode)
        {
            ViewBag.MenuType = "DeviceInfo";

            if (string.IsNullOrEmpty(ngrCode))
            {
                return HttpNotFound("NGR 코드가 제공되지 않았습니다.");
            }

            // RiskMatrix 데이터 처리 (기존 코드)
            var riskMatrixRepo = new RiskmatrixRepository();

            var hiDict = riskMatrixRepo.GetRiskMatrixByCode(ngrCode);
            var matrixDict = riskMatrixRepo.GetRiskMatrixPofCofByCode(ngrCode);

            ViewBag.HIDict = hiDict;
            ViewBag.RiskMatrixDict = matrixDict;

            // NGR 기본정보 조회
            var model = ngrBasicInfoRepo.GetNGRBasicInfoByCode(ngrCode);
            if (model == null)
            {
                return HttpNotFound("해당 NGR 정보를 찾을 수 없습니다.");
            }

            // 보통점검 데이터 조회
            var ngrChkRepo = new NGRChkRepository();
            List<NGRChk> chkList;
            var result = ngrChkRepo.GetNGRChkByNGRCode(ngrCode, out chkList);
            ViewBag.NGRChkList = chkList;

            return View("~/Views/Device/NGR/NGRDeviceDetail.cshtml", model);
        }

    }
}
