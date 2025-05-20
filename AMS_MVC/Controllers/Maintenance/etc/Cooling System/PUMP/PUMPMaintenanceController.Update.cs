using AMS_MVC.Models;
using AMS_MVC.Utlity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Maintenance.PUMP
{
    public partial class PUMPMaintenanceController : Controller
    {
        [HttpGet]
        public ActionResult PUMPMaintenanceUpdate(string pumpCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(pumpCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("PUMPMaintenanceTotalList");
            }

            // 유지보수 이력 조회
            var result = pumpMaintenanceRepository.GetPUMPMRDetailByPUMPCode(pumpCode, tblIdx, out var pumpMaintenanceList);

            if (!result.IsSuccess || pumpMaintenanceList == null || !pumpMaintenanceList.Any())
            {
                return HttpNotFound("PUMP 유지보수이력 정보를 찾을 수 없습니다.");
            }

            var detailRecord = pumpMaintenanceList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 유지보수 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Maintenance/etc/Cooling System/PUMP/PUMPMaintenanceUpdate.cshtml", detailRecord);
        }

        [HttpPost]
        public ActionResult PUMPMaintenanceUpdate(PUMPMaintenanceHistory model)
        {
            Result result = new Result(true);
            try
            {
                model.MR_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";

                result = pumpMaintenanceRepository.UpdatePUMPMRRepo(model);
                if (!result.IsSuccess)
                {
                    result.Message = "PUMP 유지보수이력 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("PUMPMaintenanceUpdate Error", ex.Message);
            }

            return Json(result);
        }
    }
}
