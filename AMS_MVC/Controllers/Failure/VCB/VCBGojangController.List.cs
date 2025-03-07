using AMS_MVC.Models;
using AMS_MVC.Utlity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers
{
    public partial class VCBGojangController : Controller
    {
        // GET: VCBGojang
        public ActionResult VCBGojangList(string VCB_Code)
        {
            var basicInfo = vcbBasicInfoRepository.GetVCBBasicInfoByCode(VCB_Code);
            ViewBag.VCBCode = VCB_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Gojang/VCB/VCBGojangList.cshtml");
        }

        public ActionResult VCBGojangTotalList()
        {
            return View("~/Views/Gojang/Total/VCBGojangTotalList.cshtml");
        }

        /// <summary>
        /// VCB 고장 이력 데이터 가져오기
        /// </summary>
        [HttpPost]
        public JsonResult GetVCBFHByVCBCode(string vcbCode)
        {
            Result res = new Result(true);
            List<VCBFailureHistory> vcbFh = new List<VCBFailureHistory>();

            try
            {
                LogHelper.WriteLog("VCBFHController.cs", "GetVCBFHByVCBCode 실행");

                res = vcbGojangRepository.GetVCBFHByVCBCode(vcbCode, out vcbFh);

                if (!res.IsSuccess)
                {
                    LogHelper.WriteLog("VCBFHController.cs", res.Message);
                    return Json(new { success = false, message = res.Message });
                }

                if (vcbFh.Count == 0)
                {
                    LogHelper.WriteLog("VCBFHController.cs", "조회된 데이터가 없습니다.");
                    return Json(new { success = true, data = new List<VCBFailureHistory>() });
                }

                LogHelper.WriteLog("VCBFHController.cs", $"조회된 데이터: {vcbFh.Count}건");

                return Json(new { success = true, data = vcbFh });
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("VCBFHController.cs", "오류 발생: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalVCBGojangListData()
        {
            try
            {
                LogHelper.WriteLog("TotalVCBGojangController.List", "GetTotalVCBGojangListData 실행");

                List<VCBFailureHistory> vcbGojang = new List<VCBFailureHistory>();
                var repoResult = vcbGojangRepository.GetTotalVCBGojang(out vcbGojang);
                if (repoResult.IsSuccess)
                {
                    LogHelper.WriteLog("VCBGojangController.List", $"조회된 데이터: {vcbGojang.Count}건");
                    return Json(vcbGojang);
                }
                else
                {
                    LogHelper.WriteLog("VCBGojangController.List", "전체 VCB 고장이력 데이터 로드 실패");
                    return Json(new { success = false, message = "전체 VCB 고장이력 데이터 로드 실패" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("VCBGojangController.List", $"GetTotalVCBGojangListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}