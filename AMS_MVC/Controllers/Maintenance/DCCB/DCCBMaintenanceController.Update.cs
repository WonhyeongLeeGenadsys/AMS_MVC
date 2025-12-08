
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class DCCBMaintenanceController : Controller
    {
        [HttpGet]
        public ActionResult DCCBMaintenanceUpdate(string dccbCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(dccbCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("DCCBMaintenanceTotalList");
            }

            // 유지보수 이력 조회
            var result = dccbMaintenanceRepository.GetDCCBMRDetailByDCCBCode(dccbCode, tblIdx, out var dccbMaintenanceList);

            if (!result.IsSuccess || dccbMaintenanceList == null || !dccbMaintenanceList.Any())
            {
                return HttpNotFound("DCCB 유지보수이력 정보를 찾을 수 없습니다.");
            }

            var detailRecord = dccbMaintenanceList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 유지보수 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Maintenance/DCCB/DCCBMaintenanceUpdate.cshtml", detailRecord);
        }

        [HttpPost]
        public ActionResult DCCBMaintenanceUpdate(DCCBMaintenanceHistory model)
        {
            Result result = new Result(true);
            try
            {
                model.MR_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";

                result = dccbMaintenanceRepository.UpdateDCCBMRRepo(model);
                if (!result.IsSuccess)
                {
                    result.Message = "DCCB 유지보수이력 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("DCCBMaintenanceUpdate Error", ex.Message);
            }

            return Json(result);
        }
    }
}
