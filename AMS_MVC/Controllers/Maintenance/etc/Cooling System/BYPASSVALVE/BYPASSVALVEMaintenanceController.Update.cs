
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class BYPASSVALVEMaintenanceController : Controller
    {
        [HttpGet]
        public ActionResult BYPASSVALVEMaintenanceUpdate(string bypassvalveCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(bypassvalveCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("BYPASSVALVEMaintenanceTotalList");
            }

            // 유지보수 이력 조회
            var result = bypassvalveMaintenanceRepository.GetBYPASSVALVEMRDetailByBYPASSVALVECode(bypassvalveCode, tblIdx, out var bypassvalveMaintenanceList);

            if (!result.IsSuccess || bypassvalveMaintenanceList == null || !bypassvalveMaintenanceList.Any())
            {
                return HttpNotFound("BYPASSVALVE 유지보수이력 정보를 찾을 수 없습니다.");
            }

            var detailRecord = bypassvalveMaintenanceList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 유지보수 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Maintenance/etc/Cooling System/BYPASSVALVE/BYPASSVALVEMaintenanceUpdate.cshtml", detailRecord);
        }

        [HttpPost]
        public ActionResult BYPASSVALVEMaintenanceUpdate(BYPASSVALVEMaintenanceHistory model)
        {
            Result result = new Result(true);
            try
            {
                model.MR_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";

                result = bypassvalveMaintenanceRepository.UpdateBYPASSVALVEMRRepo(model);
                if (!result.IsSuccess)
                {
                    result.Message = "BYPASSVALVE 유지보수이력 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("BYPASSVALVEMaintenanceUpdate Error", ex.Message);
            }

            return Json(result);
        }
    }
}
