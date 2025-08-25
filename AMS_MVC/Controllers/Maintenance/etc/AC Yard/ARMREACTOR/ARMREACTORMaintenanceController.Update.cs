
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class ARMREACTORMaintenanceController : Controller
    {
        [HttpGet]
        public ActionResult ARMREACTORMaintenanceUpdate(string armreactorCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(armreactorCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("ARMREACTORMaintenanceTotalList");
            }

            // 유지보수 이력 조회
            var result = armreactorMaintenanceRepository.GetARMREACTORMRDetailByARMREACTORCode(armreactorCode, tblIdx, out var armreactorMaintenanceList);

            if (!result.IsSuccess || armreactorMaintenanceList == null || !armreactorMaintenanceList.Any())
            {
                return HttpNotFound("ARMREACTOR 유지보수이력 정보를 찾을 수 없습니다.");
            }

            var detailRecord = armreactorMaintenanceList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 유지보수 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Maintenance/etc/AC Yard/ARMREACTOR/ARMREACTORMaintenanceUpdate.cshtml", detailRecord);
        }

        [HttpPost]
        public ActionResult ARMREACTORMaintenanceUpdate(ARMREACTORMaintenanceHistory model)
        {
            Result result = new Result(true);
            try
            {
                model.MR_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";

                result = armreactorMaintenanceRepository.UpdateARMREACTORMRRepo(model);
                if (!result.IsSuccess)
                {
                    result.Message = "ARMREACTOR 유지보수이력 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("ARMREACTORMaintenanceUpdate Error", ex.Message);
            }

            return Json(result);
        }
    }
}
