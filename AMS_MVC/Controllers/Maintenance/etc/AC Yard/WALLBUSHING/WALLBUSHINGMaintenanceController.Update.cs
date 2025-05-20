using AMS_MVC.Models;
using AMS_MVC.Utlity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Maintenance.WALLBUSHING
{
    public partial class WALLBUSHINGMaintenanceController : Controller
    {
        [HttpGet]
        public ActionResult WALLBUSHINGMaintenanceUpdate(string wallbushingCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(wallbushingCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("WALLBUSHINGMaintenanceTotalList");
            }

            // 유지보수 이력 조회
            var result = wallbushingMaintenanceRepository.GetWALLBUSHINGMRDetailByWALLBUSHINGCode(wallbushingCode, tblIdx, out var wallbushingMaintenanceList);

            if (!result.IsSuccess || wallbushingMaintenanceList == null || !wallbushingMaintenanceList.Any())
            {
                return HttpNotFound("WALLBUSHING 유지보수이력 정보를 찾을 수 없습니다.");
            }

            var detailRecord = wallbushingMaintenanceList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 유지보수 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Maintenance/etc/AC Yard/WALLBUSHING/WALLBUSHINGMaintenanceUpdate.cshtml", detailRecord);
        }

        [HttpPost]
        public ActionResult WALLBUSHINGMaintenanceUpdate(WALLBUSHINGMaintenanceHistory model)
        {
            Result result = new Result(true);
            try
            {
                model.MR_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";

                result = wallbushingMaintenanceRepository.UpdateWALLBUSHINGMRRepo(model);
                if (!result.IsSuccess)
                {
                    result.Message = "WALLBUSHING 유지보수이력 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("WALLBUSHINGMaintenanceUpdate Error", ex.Message);
            }

            return Json(result);
        }
    }
}
