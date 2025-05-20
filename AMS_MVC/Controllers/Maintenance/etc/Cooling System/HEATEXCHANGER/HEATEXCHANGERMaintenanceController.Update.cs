using AMS_MVC.Models;
using AMS_MVC.Utlity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Maintenance.HEATEXCHANGER
{
    public partial class HEATEXCHANGERMaintenanceController : Controller
    {
        [HttpGet]
        public ActionResult HEATEXCHANGERMaintenanceUpdate(string heatexchangerCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(heatexchangerCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("HEATEXCHANGERMaintenanceTotalList");
            }

            // 유지보수 이력 조회
            var result = heatexchangerMaintenanceRepository.GetHEATEXCHANGERMRDetailByHEATEXCHANGERCode(heatexchangerCode, tblIdx, out var heatexchangerMaintenanceList);

            if (!result.IsSuccess || heatexchangerMaintenanceList == null || !heatexchangerMaintenanceList.Any())
            {
                return HttpNotFound("HEATEXCHANGER 유지보수이력 정보를 찾을 수 없습니다.");
            }

            var detailRecord = heatexchangerMaintenanceList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 유지보수 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Maintenance/etc/Cooling System/HEATEXCHANGER/HEATEXCHANGERMaintenanceUpdate.cshtml", detailRecord);
        }

        [HttpPost]
        public ActionResult HEATEXCHANGERMaintenanceUpdate(HEATEXCHANGERMaintenanceHistory model)
        {
            Result result = new Result(true);
            try
            {
                model.MR_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";

                result = heatexchangerMaintenanceRepository.UpdateHEATEXCHANGERMRRepo(model);
                if (!result.IsSuccess)
                {
                    result.Message = "HEATEXCHANGER 유지보수이력 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("HEATEXCHANGERMaintenanceUpdate Error", ex.Message);
            }

            return Json(result);
        }
    }
}
