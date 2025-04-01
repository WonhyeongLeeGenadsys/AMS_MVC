using AMS_MVC.Models;
using AMS_MVC.Utlity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Maintenance.SUBMODULE
{
    public partial class SUBMODULEMaintenanceController : Controller
    {
        [HttpGet]
        public ActionResult SUBMODULEMaintenanceUpdate(string submoduleCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(submoduleCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("SUBMODULEMaintenanceTotalList");
            }

            // 유지보수 이력 조회
            var result = submoduleMaintenanceRepository.GetSUBMODULEMRDetailBySUBMODULECode(submoduleCode, tblIdx, out var submoduleMaintenanceList);

            if (!result.IsSuccess || submoduleMaintenanceList == null || !submoduleMaintenanceList.Any())
            {
                return HttpNotFound("SUBMODULE 유지보수이력 정보를 찾을 수 없습니다.");
            }

            var detailRecord = submoduleMaintenanceList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 유지보수 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Maintenance/SUBMODULE/SUBMODULEMaintenanceUpdate.cshtml", detailRecord);
        }

        [HttpPost]
        public ActionResult SUBMODULEMaintenanceUpdate(SUBMODULEMaintenanceHistory model)
        {
            Result result = new Result(true);
            try
            {
                model.MR_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";

                result = submoduleMaintenanceRepository.UpdateSUBMODULEMRRepo(model);
                if (!result.IsSuccess)
                {
                    result.Message = "SUBMODULE 유지보수이력 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("SUBMODULEMaintenanceUpdate Error", ex.Message);
            }

            return Json(result);
        }
    }
}
