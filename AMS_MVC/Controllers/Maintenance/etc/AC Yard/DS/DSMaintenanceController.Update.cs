
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class DSMaintenanceController : Controller
    {
        [HttpGet]
        public ActionResult DSMaintenanceUpdate(string dsCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(dsCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("DSMaintenanceTotalList");
            }

            // 유지보수 이력 조회
            var result = dsMaintenanceRepository.GetDSMRDetailByDSCode(dsCode, tblIdx, out var dsMaintenanceList);

            if (!result.IsSuccess || dsMaintenanceList == null || !dsMaintenanceList.Any())
            {
                return HttpNotFound("DS 유지보수이력 정보를 찾을 수 없습니다.");
            }

            var detailRecord = dsMaintenanceList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 유지보수 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Maintenance/etc/AC Yard/DS/DSMaintenanceUpdate.cshtml", detailRecord);
        }

        [HttpPost]
        public ActionResult DSMaintenanceUpdate(DSMaintenanceHistory model)
        {
            Result result = new Result(true);
            try
            {
                model.MR_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";

                result = dsMaintenanceRepository.UpdateDSMRRepo(model);
                if (!result.IsSuccess)
                {
                    result.Message = "DS 유지보수이력 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("DSMaintenanceUpdate Error", ex.Message);
            }

            return Json(result);
        }
    }
}
