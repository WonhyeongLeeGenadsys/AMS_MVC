using AMS_MVC.Models;
using AMS_MVC.Utlity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Maintenance.ES
{
    public partial class ESMaintenanceController : Controller
    {
        [HttpGet]
        public ActionResult ESMaintenanceUpdate(string esCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(esCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("ESMaintenanceTotalList");
            }

            // 유지보수 이력 조회
            var result = esMaintenanceRepository.GetESMRDetailByESCode(esCode, tblIdx, out var esMaintenanceList);

            if (!result.IsSuccess || esMaintenanceList == null || !esMaintenanceList.Any())
            {
                return HttpNotFound("ES 유지보수이력 정보를 찾을 수 없습니다.");
            }

            var detailRecord = esMaintenanceList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 유지보수 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Maintenance/etc/Grounding/ES/ESMaintenanceUpdate.cshtml", detailRecord);
        }

        [HttpPost]
        public ActionResult ESMaintenanceUpdate(ESMaintenanceHistory model)
        {
            Result result = new Result(true);
            try
            {
                model.MR_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";

                result = esMaintenanceRepository.UpdateESMRRepo(model);
                if (!result.IsSuccess)
                {
                    result.Message = "ES 유지보수이력 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("ESMaintenanceUpdate Error", ex.Message);
            }

            return Json(result);
        }
    }
}
