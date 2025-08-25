using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class DCCABLEMaintenanceController : Controller
    {
        // 예시: 상세보기 페이지 (여러 유지보수정보 중 tblIdx에 해당하는 레코드를 선택)
        [HttpGet]
        public ActionResult DCCABLEMaintenanceDetail(string dccableCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(dccableCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("Index");
            }

            List<DCCABLEMaintenanceHistory> dccableMaintenanceList = new List<DCCABLEMaintenanceHistory>();
            var result = dccableMaintenanceRepository.GetDCCABLEMRDetailByDCCABLECode(dccableCode, tblIdx, out dccableMaintenanceList);

            if (!result.IsSuccess || dccableMaintenanceList == null || !dccableMaintenanceList.Any())
            {
                return HttpNotFound("DCCABLE 유지보수이력 정보를 찾을 수 없습니다.");
            }

            // tblIdx와 일치하는 상세 레코드를 선택합니다.
            var detailRecord = dccableMaintenanceList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 유지보수 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Maintenance/DCCABLE/DCCABLEMaintenanceDetail.cshtml", detailRecord);
        }

        // 기타 Ajax 액션 메서드들도 동일한 방식으로 수정합니다.
        [HttpPost]
        public JsonResult GetDCCABLEMaintenance(string dccableCode)
        {
            if (string.IsNullOrEmpty(dccableCode))
            {
                return Json(new { success = false, message = "올바른 DCCABLE_Code가 전달되지 않았습니다." });
            }
            List<DCCABLEMaintenanceHistory> dccableMaintenanceList = new List<DCCABLEMaintenanceHistory>();
            var result = dccableMaintenanceRepository.GetDCCABLEMRByDCCABLECode(dccableCode, out dccableMaintenanceList);
            if (result == null || !result.IsSuccess)
            {
                return Json(new { success = false, message = "DCCABLE 유지보수이력 정보를 찾을 수 없습니다." });
            }

            return Json(new { success = true, data = dccableMaintenanceList });
        }

        [HttpPost]
        public JsonResult DCCABLEMaintenanceDelete(string dccableCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(dccableCode))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = dccableMaintenanceRepository.DeleteDCCABLEMRRepo(dccableCode, tblIdx);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "DCCABLE 유지보수이력 정보가 삭제되었습니다." });
            }
            else
            {
                return Json(new { success = false, message = "DCCABLE 유지보수이력 정보 삭제 실패: " + result.Message });
            }
        }
    }
}