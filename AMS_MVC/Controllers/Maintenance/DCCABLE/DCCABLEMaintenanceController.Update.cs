using AMS_MVC.Models;
using AMS_MVC.Utlity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Maintenance.DCCABLE
{
    public partial class DCCABLEMaintenanceController : Controller
    {
        [HttpGet]
        public ActionResult DCCABLEMaintenanceUpdate(string dccableCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(dccableCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("DCCABLEMaintenanceTotalList");
            }

            // 유지보수 이력 조회
            var result = dccableMaintenanceRepository.GetDCCABLEMRDetailByDCCABLECode(dccableCode, tblIdx, out var dccableMaintenanceList);

            if (!result.IsSuccess || dccableMaintenanceList == null || !dccableMaintenanceList.Any())
            {
                return HttpNotFound("DCCABLE 유지보수이력 정보를 찾을 수 없습니다.");
            }

            var detailRecord = dccableMaintenanceList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 유지보수 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Maintenance/DCCABLE/DCCABLEMaintenanceUpdate.cshtml", detailRecord);
        }

        [HttpPost]
        public ActionResult DCCABLEMaintenanceUpdate(DCCABLEMaintenanceHistory model)
        {
            Result result = new Result(true);
            try
            {
                model.MR_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";

                result = dccableMaintenanceRepository.UpdateDCCABLEMRRepo(model);
                if (!result.IsSuccess)
                {
                    result.Message = "DCCABLE 유지보수이력 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("DCCABLEMaintenanceUpdate Error", ex.Message);
            }

            return Json(result);
        }
    }
}
