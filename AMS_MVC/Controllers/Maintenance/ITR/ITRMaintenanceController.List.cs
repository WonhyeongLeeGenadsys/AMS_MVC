using AMS_MVC.Models;
using AMS_MVC.Utlity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Maintenance.ITR
{
    public partial class ITRMaintenanceController : Controller
    {
        // GET: ITRMaintenance
        public ActionResult ITRMaintenanceList(string ITR_Code)
        {
            var basicInfo = itrBasicInfoRepository.GetITRBasicInfoByITRCode(ITR_Code);
            ViewBag.ITRCode = ITR_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Maintenance/ITR/ITRMaintenanceList.cshtml");
        }

        public ActionResult ITRMaintenanceTotalList()
        {
            return View("~/Views/Maintenance/Total/ITRMaintenanceTotalList.cshtml");
        }

        /// <summary>
        /// ITR 유지보수 데이터 가져오기
        /// </summary>
        [HttpPost]
        public JsonResult GetITRMRByITRCode(string itrCode)
        {
            Result res = new Result(true);
            List<ITRMaintenanceHistory> itrMR = new List<ITRMaintenanceHistory>();

            try
            {
                LogHelper.WriteLog("ITRMRController.cs", "GetITRMRByITRCode 실행");

                res = itrMaintenanceRepository.GetITRMRByITRCode(itrCode, out itrMR);

                if (!res.IsSuccess)
                {
                    LogHelper.WriteLog("ITRMRController.cs", res.Message);
                    return Json(new { success = false, message = res.Message });
                }

                if (itrMR.Count == 0)
                {
                    LogHelper.WriteLog("ITRMRController.cs", "조회된 데이터가 없습니다.");
                    return Json(new { success = true, data = new List<ITRMaintenanceHistory>() });
                }

                LogHelper.WriteLog("ITRMRController.cs", $"조회된 데이터: {itrMR.Count}건");

                return Json(new { success = true, data = itrMR });
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("ITRMRController.cs", "오류 발생: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalITRMaintenanceListData()
        {
            try
            {
                LogHelper.WriteLog("TotalITRMaintenanceController.List", "GetTotalITRMaintenanceListData 실행");

                List<ITRMaintenanceHistory> itrMaintenance = new List<ITRMaintenanceHistory>();
                var repoResult = itrMaintenanceRepository.GetTotalITRMaintenance(out itrMaintenance);
                if (repoResult.IsSuccess)
                {
                    LogHelper.WriteLog("ITRMaintenanceController.List", $"조회된 데이터: {itrMaintenance.Count}건");
                    return Json(itrMaintenance);
                }
                else
                {
                    LogHelper.WriteLog("ITRMaintenanceController.List", "전체 ITR 유지보수 데이터 로드 실패");
                    return Json(new { success = false, message = "전체 ITR 유지보수 데이터 로드 실패" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("ITRMaintenanceController.List", $"GetTotalITRMaintenanceListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}