using AMS_MVC.Models;
using AMS_MVC.Utlity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Maintenance.TANK
{
    public partial class TANKMaintenanceController : Controller
    {
        [HttpGet]
        public ActionResult TANKMaintenanceUpdate(string tankCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(tankCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("TANKMaintenanceTotalList");
            }

            // 유지보수 이력 조회
            var result = tankMaintenanceRepository.GetTANKMRDetailByTANKCode(tankCode, tblIdx, out var tankMaintenanceList);

            if (!result.IsSuccess || tankMaintenanceList == null || !tankMaintenanceList.Any())
            {
                return HttpNotFound("TANK 유지보수이력 정보를 찾을 수 없습니다.");
            }

            var detailRecord = tankMaintenanceList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 유지보수 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Maintenance/etc/Cooling System/TANK/TANKMaintenanceUpdate.cshtml", detailRecord);
        }

        [HttpPost]
        public ActionResult TANKMaintenanceUpdate(TANKMaintenanceHistory model)
        {
            Result result = new Result(true);
            try
            {
                model.MR_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";

                result = tankMaintenanceRepository.UpdateTANKMRRepo(model);
                if (!result.IsSuccess)
                {
                    result.Message = "TANK 유지보수이력 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("TANKMaintenanceUpdate Error", ex.Message);
            }

            return Json(result);
        }
    }
}
