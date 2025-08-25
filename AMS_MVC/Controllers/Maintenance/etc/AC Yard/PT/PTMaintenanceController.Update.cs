
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class PTMaintenanceController : Controller
    {
        [HttpGet]
        public ActionResult PTMaintenanceUpdate(string ptCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(ptCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("PTMaintenanceTotalList");
            }

            // 유지보수 이력 조회
            var result = ptMaintenanceRepository.GetPTMRDetailByPTCode(ptCode, tblIdx, out var ptMaintenanceList);

            if (!result.IsSuccess || ptMaintenanceList == null || !ptMaintenanceList.Any())
            {
                return HttpNotFound("PT 유지보수이력 정보를 찾을 수 없습니다.");
            }

            var detailRecord = ptMaintenanceList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 유지보수 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Maintenance/etc/AC Yard/PT/PTMaintenanceUpdate.cshtml", detailRecord);
        }

        [HttpPost]
        public ActionResult PTMaintenanceUpdate(PTMaintenanceHistory model)
        {
            Result result = new Result(true);
            try
            {
                model.MR_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";

                result = ptMaintenanceRepository.UpdatePTMRRepo(model);
                if (!result.IsSuccess)
                {
                    result.Message = "PT 유지보수이력 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("PTMaintenanceUpdate Error", ex.Message);
            }

            return Json(result);
        }
    }
}
