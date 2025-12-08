
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class DCCTMaintenanceController : Controller
    {
        [HttpGet]
        public ActionResult DCCTMaintenanceUpdate(string dcctCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(dcctCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("DCCTMaintenanceTotalList");
            }

            // 유지보수 이력 조회
            var result = dcctMaintenanceRepository.GetDCCTMRDetailByDCCTCode(dcctCode, tblIdx, out var dcctMaintenanceList);

            if (!result.IsSuccess || dcctMaintenanceList == null || !dcctMaintenanceList.Any())
            {
                return HttpNotFound("DCCT 유지보수이력 정보를 찾을 수 없습니다.");
            }

            var detailRecord = dcctMaintenanceList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 유지보수 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Maintenance/etc/DC Yard/DCCT/DCCTMaintenanceUpdate.cshtml", detailRecord);
        }

        [HttpPost]
        public ActionResult DCCTMaintenanceUpdate(DCCTMaintenanceHistory model)
        {
            Result result = new Result(true);
            try
            {
                model.MR_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";

                result = dcctMaintenanceRepository.UpdateDCCTMRRepo(model);
                if (!result.IsSuccess)
                {
                    result.Message = "DCCT 유지보수이력 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("DCCTMaintenanceUpdate Error", ex.Message);
            }

            return Json(result);
        }
    }
}
