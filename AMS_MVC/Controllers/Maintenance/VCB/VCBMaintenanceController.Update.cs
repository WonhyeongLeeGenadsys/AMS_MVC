using AMS_MVC.Models;
using AMS_MVC.Utlity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Maintenance.VCB
{
    public partial class VCBMaintenanceController : Controller
    {
        [HttpGet]
        public ActionResult VCBMaintenanceUpdate(string vcbCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(vcbCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("VCBMaintenanceTotalList");
            }

            // 유지보수 이력 조회
            var result = vcbMaintenanceRepository.GetVCBMRDetailByVCBCode(vcbCode, tblIdx, out var vcbMaintenanceList);

            if (!result.IsSuccess || vcbMaintenanceList == null || !vcbMaintenanceList.Any())
            {
                return HttpNotFound("VCB 유지보수이력 정보를 찾을 수 없습니다.");
            }

            var detailRecord = vcbMaintenanceList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 유지보수 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Maintenance/VCB/VCBMaintenanceUpdate.cshtml", detailRecord);
        }

        [HttpPost]
        public ActionResult VCBMaintenanceUpdate(VCBMaintenanceHistory model)
        {
            Result result = new Result(true);
            try
            {
                model.MR_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";

                result = vcbMaintenanceRepository.UpdateVCBMRRepo(model);
                if (!result.IsSuccess)
                {
                    result.Message = "VCB 유지보수이력 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("VCBMaintenanceUpdate Error", ex.Message);
            }

            return Json(result);
        }
    }
}
