using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class DSMaintenanceController : Controller
    {
        // 예시: 상세보기 페이지 (여러 유지보수정보 중 tblIdx에 해당하는 레코드를 선택)
        [HttpGet]
        public ActionResult DSMaintenanceDetail(string dsCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(dsCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("Index");
            }

            List<DSMaintenanceHistory> dsMaintenanceList = new List<DSMaintenanceHistory>();
            var result = dsMaintenanceRepository.GetDSMRDetailByDSCode(dsCode, tblIdx, out dsMaintenanceList);

            if (!result.IsSuccess || dsMaintenanceList == null || !dsMaintenanceList.Any())
            {
                return HttpNotFound("DS 유지보수이력 정보를 찾을 수 없습니다.");
            }

            // tblIdx와 일치하는 상세 레코드를 선택합니다.
            var detailRecord = dsMaintenanceList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 유지보수 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Maintenance/etc/AC Yard/DS/DSMaintenanceDetail.cshtml", detailRecord);
        }

        // 기타 Ajax 액션 메서드들도 동일한 방식으로 수정합니다.
        [HttpPost]
        public JsonResult GetDSMaintenance(string dsCode)
        {
            if (string.IsNullOrEmpty(dsCode))
            {
                return Json(new { success = false, message = "올바른 DS_Code가 전달되지 않았습니다." });
            }
            List<DSMaintenanceHistory> dsMaintenanceList = new List<DSMaintenanceHistory>();
            var result = dsMaintenanceRepository.GetDSMRByDSCode(dsCode, out dsMaintenanceList);
            if (result == null || !result.IsSuccess)
            {
                return Json(new { success = false, message = "DS 유지보수이력 정보를 찾을 수 없습니다." });
            }

            return Json(new { success = true, data = dsMaintenanceList });
        }

        [HttpPost]
        public JsonResult DSMaintenanceDelete(string dsCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(dsCode))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = dsMaintenanceRepository.DeleteDSMRRepo(dsCode, tblIdx);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "DS 유지보수이력 정보가 삭제되었습니다." });
            }
            else
            {
                return Json(new { success = false, message = "DS 유지보수이력 정보 삭제 실패: " + result.Message });
            }
        }
    }
}