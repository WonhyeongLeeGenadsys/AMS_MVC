using AMS_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Maintenance.BYPASSVALVE
{
    public partial class BYPASSVALVEMaintenanceController : Controller
    {
        // 예시: 상세보기 페이지 (여러 유지보수정보 중 tblIdx에 해당하는 레코드를 선택)
        [HttpGet]
        public ActionResult BYPASSVALVEMaintenanceDetail(string bypassvalveCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(bypassvalveCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("Index");
            }

            List<BYPASSVALVEMaintenanceHistory> bypassvalveMaintenanceList = new List<BYPASSVALVEMaintenanceHistory>();
            var result = bypassvalveMaintenanceRepository.GetBYPASSVALVEMRDetailByBYPASSVALVECode(bypassvalveCode, tblIdx, out bypassvalveMaintenanceList);

            if (!result.IsSuccess || bypassvalveMaintenanceList == null || !bypassvalveMaintenanceList.Any())
            {
                return HttpNotFound("BYPASSVALVE 유지보수이력 정보를 찾을 수 없습니다.");
            }

            // tblIdx와 일치하는 상세 레코드를 선택합니다.
            var detailRecord = bypassvalveMaintenanceList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 유지보수 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Maintenance/etc/Cooling System/BYPASSVALVE/BYPASSVALVEMaintenanceDetail.cshtml", detailRecord);
        }

        // 기타 Ajax 액션 메서드들도 동일한 방식으로 수정합니다.
        [HttpPost]
        public JsonResult GetBYPASSVALVEMaintenance(string bypassvalveCode)
        {
            if (string.IsNullOrEmpty(bypassvalveCode))
            {
                return Json(new { success = false, message = "올바른 BYPASSVALVE_Code가 전달되지 않았습니다." });
            }
            List<BYPASSVALVEMaintenanceHistory> bypassvalveMaintenanceList = new List<BYPASSVALVEMaintenanceHistory>();
            var result = bypassvalveMaintenanceRepository.GetBYPASSVALVEMRByBYPASSVALVECode(bypassvalveCode, out bypassvalveMaintenanceList);
            if (result == null || !result.IsSuccess)
            {
                return Json(new { success = false, message = "BYPASSVALVE 유지보수이력 정보를 찾을 수 없습니다." });
            }

            return Json(new { success = true, data = bypassvalveMaintenanceList });
        }

        [HttpPost]
        public JsonResult BYPASSVALVEMaintenanceDelete(string bypassvalveCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(bypassvalveCode))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = bypassvalveMaintenanceRepository.DeleteBYPASSVALVEMRRepo(bypassvalveCode, tblIdx);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "BYPASSVALVE 유지보수이력 정보가 삭제되었습니다." });
            }
            else
            {
                return Json(new { success = false, message = "BYPASSVALVE 유지보수이력 정보 삭제 실패: " + result.Message });
            }
        }
    }
}