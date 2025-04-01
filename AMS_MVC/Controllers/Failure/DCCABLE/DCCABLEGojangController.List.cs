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
    public partial class DCCABLEGojangController : Controller
    {
        // GET: DCCABLEGojang
        public ActionResult DCCABLEGojangList(string DCCABLE_Code)
        {
            var basicInfo = dccableBasicInfoRepository.GetDCCABLEBasicInfoByCode(DCCABLE_Code);
            ViewBag.DCCABLECode = DCCABLE_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Gojang/DCCABLE/DCCABLEGojangList.cshtml");
        }

        public ActionResult DCCABLEGojangTotalList()
        {
            return View("~/Views/Gojang/Total/DCCABLEGojangTotalList.cshtml");
        }

        /// <summary>
        /// DCCABLE 고장 이력 데이터 가져오기
        /// </summary>
        [HttpPost]
        public JsonResult GetDCCABLEFHByDCCABLECode(string dccableCode)
        {
            Result res = new Result(true);
            List<DCCABLEFailureHistory> dccableFh = new List<DCCABLEFailureHistory>();

            try
            {
                LogHelper.WriteLog("DCCABLEFHController.cs", "GetDCCABLEFHByDCCABLECode 실행");

                res = dccableGojangRepository.GetDCCABLEFHByDCCABLECode(dccableCode, out dccableFh);

                if (!res.IsSuccess)
                {
                    LogHelper.WriteLog("DCCABLEFHController.cs", res.Message);
                    return Json(new { success = false, message = res.Message });
                }

                if (dccableFh.Count == 0)
                {
                    LogHelper.WriteLog("DCCABLEFHController.cs", "조회된 데이터가 없습니다.");
                    return Json(new { success = true, data = new List<DCCABLEFailureHistory>() });
                }

                LogHelper.WriteLog("DCCABLEFHController.cs", $"조회된 데이터: {dccableFh.Count}건");

                return Json(new { success = true, data = dccableFh });
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("DCCABLEFHController.cs", "오류 발생: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalDCCABLEGojangListData()
        {
            try
            {
                LogHelper.WriteLog("TotalDCCABLEGojangController.List", "GetTotalDCCABLEGojangListData 실행");

                List<DCCABLEFailureHistory> dccableGojang = new List<DCCABLEFailureHistory>();
                var repoResult = dccableGojangRepository.GetTotalDCCABLEGojang(out dccableGojang);
                if (repoResult.IsSuccess)
                {
                    LogHelper.WriteLog("DCCABLEGojangController.List", $"조회된 데이터: {dccableGojang.Count}건");
                    return Json(dccableGojang);
                }
                else
                {
                    LogHelper.WriteLog("DCCABLEGojangController.List", "전체 DCCABLE 고장이력 데이터 로드 실패");
                    return Json(new { success = false, message = "전체 DCCABLE 고장이력 데이터 로드 실패" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("DCCABLEGojangController.List", $"GetTotalDCCABLEGojangListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}