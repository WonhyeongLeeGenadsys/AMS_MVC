using AMS_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Maintenance.CT
{
    public partial class CTMaintenanceController : Controller
    {
        // 예시: 상세보기 페이지 (여러 유지보수정보 중 tblIdx에 해당하는 레코드를 선택)
        [HttpGet]
        public ActionResult CTMaintenanceDetail(string ctCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(ctCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("Index");
            }

            List<CTMaintenanceHistory> ctMaintenanceList = new List<CTMaintenanceHistory>();
            var result = ctMaintenanceRepository.GetCTMRDetailByCTCode(ctCode, tblIdx, out ctMaintenanceList);

            if (!result.IsSuccess || ctMaintenanceList == null || !ctMaintenanceList.Any())
            {
                return HttpNotFound("CT 유지보수이력 정보를 찾을 수 없습니다.");
            }

            // tblIdx와 일치하는 상세 레코드를 선택합니다.
            var detailRecord = ctMaintenanceList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 유지보수 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Maintenance/etc/AC Yard/CT/CTMaintenanceDetail.cshtml", detailRecord);
        }

        // 기타 Ajax 액션 메서드들도 동일한 방식으로 수정합니다.
        [HttpPost]
        public JsonResult GetCTMaintenance(string ctCode)
        {
            if (string.IsNullOrEmpty(ctCode))
            {
                return Json(new { success = false, message = "올바른 CT_Code가 전달되지 않았습니다." });
            }
            List<CTMaintenanceHistory> ctMaintenanceList = new List<CTMaintenanceHistory>();
            var result = ctMaintenanceRepository.GetCTMRByCTCode(ctCode, out ctMaintenanceList);
            if (result == null || !result.IsSuccess)
            {
                return Json(new { success = false, message = "CT 유지보수이력 정보를 찾을 수 없습니다." });
            }

            return Json(new { success = true, data = ctMaintenanceList });
        }

        [HttpPost]
        public JsonResult CTMaintenanceDelete(string ctCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(ctCode))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = ctMaintenanceRepository.DeleteCTMRRepo(ctCode, tblIdx);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "CT 유지보수이력 정보가 삭제되었습니다." });
            }
            else
            {
                return Json(new { success = false, message = "CT 유지보수이력 정보 삭제 실패: " + result.Message });
            }
        }
    }
}