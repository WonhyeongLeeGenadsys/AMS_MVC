using AMS_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Maintenance.SA
{
    public partial class SAMaintenanceController : Controller
    {
        // 예시: 상세보기 페이지 (여러 유지보수정보 중 tblIdx에 해당하는 레코드를 선택)
        [HttpGet]
        public ActionResult SAMaintenanceDetail(string saCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(saCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("Index");
            }

            List<SAMaintenanceHistory> saMaintenanceList = new List<SAMaintenanceHistory>();
            var result = saMaintenanceRepository.GetSAMRDetailBySACode(saCode, tblIdx, out saMaintenanceList);

            if (!result.IsSuccess || saMaintenanceList == null || !saMaintenanceList.Any())
            {
                return HttpNotFound("SA 유지보수이력 정보를 찾을 수 없습니다.");
            }

            // tblIdx와 일치하는 상세 레코드를 선택합니다.
            var detailRecord = saMaintenanceList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 유지보수 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Maintenance/etc/DC Yard/SA/SAMaintenanceDetail.cshtml", detailRecord);
        }

        // 기타 Ajax 액션 메서드들도 동일한 방식으로 수정합니다.
        [HttpPost]
        public JsonResult GetSAMaintenance(string saCode)
        {
            if (string.IsNullOrEmpty(saCode))
            {
                return Json(new { success = false, message = "올바른 SA_Code가 전달되지 않았습니다." });
            }
            List<SAMaintenanceHistory> saMaintenanceList = new List<SAMaintenanceHistory>();
            var result = saMaintenanceRepository.GetSAMRBySACode(saCode, out saMaintenanceList);
            if (result == null || !result.IsSuccess)
            {
                return Json(new { success = false, message = "SA 유지보수이력 정보를 찾을 수 없습니다." });
            }

            return Json(new { success = true, data = saMaintenanceList });
        }

        [HttpPost]
        public JsonResult SAMaintenanceDelete(string saCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(saCode))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = saMaintenanceRepository.DeleteSAMRRepo(saCode, tblIdx);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "SA 유지보수이력 정보가 삭제되었습니다." });
            }
            else
            {
                return Json(new { success = false, message = "SA 유지보수이력 정보 삭제 실패: " + result.Message });
            }
        }
    }
}