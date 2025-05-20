using AMS_MVC.Models;
using AMS_MVC.Utlity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Maintenance.CT
{
    public partial class CTMaintenanceController : Controller
    {
        [HttpGet]
        public ActionResult CTMaintenanceUpdate(string ctCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(ctCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("CTMaintenanceTotalList");
            }

            // 유지보수 이력 조회
            var result = ctMaintenanceRepository.GetCTMRDetailByCTCode(ctCode, tblIdx, out var ctMaintenanceList);

            if (!result.IsSuccess || ctMaintenanceList == null || !ctMaintenanceList.Any())
            {
                return HttpNotFound("CT 유지보수이력 정보를 찾을 수 없습니다.");
            }

            var detailRecord = ctMaintenanceList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 유지보수 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Maintenance/etc/AC Yard/CT/CTMaintenanceUpdate.cshtml", detailRecord);
        }

        [HttpPost]
        public ActionResult CTMaintenanceUpdate(CTMaintenanceHistory model)
        {
            Result result = new Result(true);
            try
            {
                model.MR_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";

                result = ctMaintenanceRepository.UpdateCTMRRepo(model);
                if (!result.IsSuccess)
                {
                    result.Message = "CT 유지보수이력 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("CTMaintenanceUpdate Error", ex.Message);
            }

            return Json(result);
        }
    }
}
