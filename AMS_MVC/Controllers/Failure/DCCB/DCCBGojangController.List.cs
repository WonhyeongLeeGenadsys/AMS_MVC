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
    public partial class DCCBGojangController : Controller
    {
        // GET: DCCBGojang
        public ActionResult DCCBGojangList(string DCCB_Code)
        {
            var basicInfo = dccbBasicInfoRepository.GetDCCBBasicInfoByCode(DCCB_Code);
            ViewBag.DCCBCode = DCCB_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Gojang/DCCB/DCCBGojangList.cshtml");
        }

        public ActionResult DCCBGojangTotalList()
        {
            return View("~/Views/Gojang/Total/DCCBGojangTotalList.cshtml");
        }

        /// <summary>
        /// DCCB 고장 이력 데이터 가져오기
        /// </summary>
        [HttpPost]
        public JsonResult GetDCCBFHByDCCBCode(string dccbCode)
        {
            Result res = new Result(true);
            List<DCCBFailureHistory> dccbFh = new List<DCCBFailureHistory>();

            try
            {
                LogHelper.WriteLog("DCCBFHController.cs", "GetDCCBFHByDCCBCode 실행");

                res = dccbGojangRepository.GetDCCBFHByDCCBCode(dccbCode, out dccbFh);

                if (!res.IsSuccess)
                {
                    LogHelper.WriteLog("DCCBFHController.cs", res.Message);
                    return Json(new { success = false, message = res.Message });
                }

                if (dccbFh.Count == 0)
                {
                    LogHelper.WriteLog("DCCBFHController.cs", "조회된 데이터가 없습니다.");
                    return Json(new { success = true, data = new List<DCCBFailureHistory>() });
                }

                LogHelper.WriteLog("DCCBFHController.cs", $"조회된 데이터: {dccbFh.Count}건");

                return Json(new { success = true, data = dccbFh });
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("DCCBFHController.cs", "오류 발생: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalDCCBGojangListData()
        {
            try
            {
                LogHelper.WriteLog("TotalDCCBGojangController.List", "GetTotalDCCBGojangListData 실행");

                List<DCCBFailureHistory> dccbGojang = new List<DCCBFailureHistory>();
                var repoResult = dccbGojangRepository.GetTotalDCCBGojang(out dccbGojang);
                if (repoResult.IsSuccess)
                {
                    LogHelper.WriteLog("DCCBGojangController.List", $"조회된 데이터: {dccbGojang.Count}건");
                    return Json(dccbGojang);
                }
                else
                {
                    LogHelper.WriteLog("DCCBGojangController.List", "전체 DCCB 고장이력 데이터 로드 실패");
                    return Json(new { success = false, message = "전체 DCCB 고장이력 데이터 로드 실패" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("DCCBGojangController.List", $"GetTotalDCCBGojangListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}