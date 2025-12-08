
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class ZIGZAGTRMaintenanceController : Controller
    {
        [HttpGet]
        public ActionResult ZIGZAGTRMaintenanceUpdate(string zigzagtrCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(zigzagtrCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("ZIGZAGTRMaintenanceTotalList");
            }

            // 유지보수 이력 조회
            var result = zigzagtrMaintenanceRepository.GetZIGZAGTRMRDetailByZIGZAGTRCode(zigzagtrCode, tblIdx, out var zigzagtrMaintenanceList);

            if (!result.IsSuccess || zigzagtrMaintenanceList == null || !zigzagtrMaintenanceList.Any())
            {
                return HttpNotFound("ZIGZAGTR 유지보수이력 정보를 찾을 수 없습니다.");
            }

            var detailRecord = zigzagtrMaintenanceList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 유지보수 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Maintenance/etc/Grounding/ZIGZAGTR/ZIGZAGTRMaintenanceUpdate.cshtml", detailRecord);
        }

        [HttpPost]
        public ActionResult ZIGZAGTRMaintenanceUpdate(ZIGZAGTRMaintenanceHistory model)
        {
            Result result = new Result(true);
            try
            {
                model.MR_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";

                result = zigzagtrMaintenanceRepository.UpdateZIGZAGTRMRRepo(model);
                if (!result.IsSuccess)
                {
                    result.Message = "ZIGZAGTR 유지보수이력 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("ZIGZAGTRMaintenanceUpdate Error", ex.Message);
            }

            return Json(result);
        }
    }
}
