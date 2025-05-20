using AMS_MVC.Models;
using AMS_MVC.Utlity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Maintenance.SA
{
    public partial class SAMaintenanceController : Controller
    {
        [HttpGet]
        public ActionResult SAMaintenanceUpdate(string saCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(saCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("SAMaintenanceTotalList");
            }

            // 유지보수 이력 조회
            var result = saMaintenanceRepository.GetSAMRDetailBySACode(saCode, tblIdx, out var saMaintenanceList);

            if (!result.IsSuccess || saMaintenanceList == null || !saMaintenanceList.Any())
            {
                return HttpNotFound("SA 유지보수이력 정보를 찾을 수 없습니다.");
            }

            var detailRecord = saMaintenanceList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 유지보수 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Maintenance/etc/DC Yard/SA/SAMaintenanceUpdate.cshtml", detailRecord);
        }

        [HttpPost]
        public ActionResult SAMaintenanceUpdate(SAMaintenanceHistory model)
        {
            Result result = new Result(true);
            try
            {
                model.MR_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";

                result = saMaintenanceRepository.UpdateSAMRRepo(model);
                if (!result.IsSuccess)
                {
                    result.Message = "SA 유지보수이력 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("SAMaintenanceUpdate Error", ex.Message);
            }

            return Json(result);
        }
    }
}
