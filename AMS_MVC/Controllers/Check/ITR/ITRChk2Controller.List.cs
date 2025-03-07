using AMS_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Check
{
    public partial class ITRChk2Controller : Controller
    {
        public ActionResult ITRChk2List(string ITR_Code)
        {
            var basicInfo = itrBasicInfoRepository.GetITRBasicInfoByITRCode(ITR_Code);
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Check/ITR/ITRChk2List.cshtml");
        }

        public ActionResult ITRChk2TotalList()
        {
            return View("~/Views/Check/Total/ITRChk2TotalList.cshtml");
        }

        [HttpPost]
        public ActionResult GetITRChk2ListData(string itrCode)
        {
            try
            {
                LogHelper.WriteLog("ITRChk2Controller.List", "GetITRChk1ListData 실행");
                LogHelper.WriteLog("itrCode 조회 : ", $"{itrCode}");

                List<ITRChk2> itrChks = new List<ITRChk2>();
                var repoResult = itrChk2Repository.GetITRChk1ByITRCode(itrCode, out itrChks);
                if (repoResult.IsSuccess)
                {
                    LogHelper.WriteLog("ITRChk2Controller.List", $"조회된 데이터: {itrChks.Count}건");
                    return Json(itrChks);
                }
                else
                {
                    LogHelper.WriteLog("ITRChk2Controller.List", "ITR 정밀점검 데이터 로드 실패");
                    return Json(new { success = false, message = "ITR 정밀점검 데이터 로드 실패" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("ITRChk2Controller.List", $"GetITRChk2ListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 전체 ITR 정밀점검 데이터 조회하기
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetTotalITRChk2ListData()
        {
            try
            {
                LogHelper.WriteLog("TotalITRChkController.List", "GetTotalITRChk2ListData 실행");

                List<ITRChk2> itrChks = new List<ITRChk2>();
                var repoResult = itrChk2Repository.GetTotalITRChk2(out itrChks);
                if (repoResult.IsSuccess)
                {
                    LogHelper.WriteLog("TotalITRChkController.List", $"조회된 데이터: {itrChks.Count}건");
                    return Json(itrChks);
                }
                else
                {
                    LogHelper.WriteLog("TotalITRChkController.List", "전체 ITR 정밀점검 데이터 로드 실패");
                    return Json(new { success = false, message = "전체 ITR 정밀점검 데이터 로드 실패" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("TotalITRChkController.List", $"GetTotalITRChk2ListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}