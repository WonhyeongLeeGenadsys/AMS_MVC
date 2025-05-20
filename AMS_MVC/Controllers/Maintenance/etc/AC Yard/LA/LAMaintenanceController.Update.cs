using AMS_MVC.Models;
using AMS_MVC.Utlity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Maintenance.LA
{
    public partial class LAMaintenanceController : Controller
    {
        [HttpGet]
        public ActionResult LAMaintenanceUpdate(string laCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(laCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("LAMaintenanceTotalList");
            }

            // 유지보수 이력 조회
            var result = laMaintenanceRepository.GetLAMRDetailByLACode(laCode, tblIdx, out var laMaintenanceList);

            if (!result.IsSuccess || laMaintenanceList == null || !laMaintenanceList.Any())
            {
                return HttpNotFound("LA 유지보수이력 정보를 찾을 수 없습니다.");
            }

            var detailRecord = laMaintenanceList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 유지보수 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Maintenance/etc/AC Yard/LA/LAMaintenanceUpdate.cshtml", detailRecord);
        }

        [HttpPost]
        public ActionResult LAMaintenanceUpdate(LAMaintenanceHistory model)
        {
            Result result = new Result(true);
            try
            {
                model.MR_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";

                result = laMaintenanceRepository.UpdateLAMRRepo(model);
                if (!result.IsSuccess)
                {
                    result.Message = "LA 유지보수이력 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("LAMaintenanceUpdate Error", ex.Message);
            }

            return Json(result);
        }
    }
}
