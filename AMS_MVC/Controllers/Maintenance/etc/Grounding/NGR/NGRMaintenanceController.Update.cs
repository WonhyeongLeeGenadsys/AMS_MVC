
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class NGRMaintenanceController : Controller
    {
        [HttpGet]
        public ActionResult NGRMaintenanceUpdate(string ngrCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(ngrCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("NGRMaintenanceTotalList");
            }

            // 유지보수 이력 조회
            var result = ngrMaintenanceRepository.GetNGRMRDetailByNGRCode(ngrCode, tblIdx, out var ngrMaintenanceList);

            if (!result.IsSuccess || ngrMaintenanceList == null || !ngrMaintenanceList.Any())
            {
                return HttpNotFound("NGR 유지보수이력 정보를 찾을 수 없습니다.");
            }

            var detailRecord = ngrMaintenanceList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 유지보수 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Maintenance/etc/Grounding/NGR/NGRMaintenanceUpdate.cshtml", detailRecord);
        }

        [HttpPost]
        public ActionResult NGRMaintenanceUpdate(NGRMaintenanceHistory model)
        {
            Result result = new Result(true);
            try
            {
                model.MR_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";

                result = ngrMaintenanceRepository.UpdateNGRMRRepo(model);
                if (!result.IsSuccess)
                {
                    result.Message = "NGR 유지보수이력 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("NGRMaintenanceUpdate Error", ex.Message);
            }

            return Json(result);
        }
    }
}
