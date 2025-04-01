using AMS_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Maintenance.VCB
{
    public partial class VCBMaintenanceController : Controller
    {
        // 예시: 상세보기 페이지 (여러 유지보수정보 중 tblIdx에 해당하는 레코드를 선택)
        [HttpGet]
        public ActionResult VCBMaintenanceDetail(string vcbCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(vcbCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("Index");
            }

            List<VCBMaintenanceHistory> vcbMaintenanceList = new List<VCBMaintenanceHistory>();
            var result = vcbMaintenanceRepository.GetVCBMRDetailByVCBCode(vcbCode, tblIdx, out vcbMaintenanceList);

            if (!result.IsSuccess || vcbMaintenanceList == null || !vcbMaintenanceList.Any())
            {
                return HttpNotFound("VCB 유지보수이력 정보를 찾을 수 없습니다.");
            }

            // tblIdx와 일치하는 상세 레코드를 선택합니다.
            var detailRecord = vcbMaintenanceList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 유지보수 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Maintenance/VCB/VCBMaintenanceDetail.cshtml", detailRecord);
        }

        // 기타 Ajax 액션 메서드들도 동일한 방식으로 수정합니다.
        [HttpPost]
        public JsonResult GetVCBMaintenance(string vcbCode)
        {
            if (string.IsNullOrEmpty(vcbCode))
            {
                return Json(new { success = false, message = "올바른 VCB_Code가 전달되지 않았습니다." });
            }
            List<VCBMaintenanceHistory> vcbMaintenanceList = new List<VCBMaintenanceHistory>();
            var result = vcbMaintenanceRepository.GetVCBMRByVCBCode(vcbCode, out vcbMaintenanceList);
            if (result == null || !result.IsSuccess)
            {
                return Json(new { success = false, message = "VCB 유지보수이력 정보를 찾을 수 없습니다." });
            }

            return Json(new { success = true, data = vcbMaintenanceList });
        }

        [HttpPost]
        public JsonResult VCBMaintenanceDelete(string vcbCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(vcbCode))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = vcbMaintenanceRepository.DeleteVCBMRRepo(vcbCode, tblIdx);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "VCB 유지보수이력 정보가 삭제되었습니다." });
            }
            else
            {
                return Json(new { success = false, message = "VCB 유지보수이력 정보 삭제 실패: " + result.Message });
            }
        }
    }
}