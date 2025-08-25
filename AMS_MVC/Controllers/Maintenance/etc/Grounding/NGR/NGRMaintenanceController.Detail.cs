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
        // 예시: 상세보기 페이지 (여러 유지보수정보 중 tblIdx에 해당하는 레코드를 선택)
        [HttpGet]
        public ActionResult NGRMaintenanceDetail(string ngrCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(ngrCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("Index");
            }

            List<NGRMaintenanceHistory> ngrMaintenanceList = new List<NGRMaintenanceHistory>();
            var result = ngrMaintenanceRepository.GetNGRMRDetailByNGRCode(ngrCode, tblIdx, out ngrMaintenanceList);

            if (!result.IsSuccess || ngrMaintenanceList == null || !ngrMaintenanceList.Any())
            {
                return HttpNotFound("NGR 유지보수이력 정보를 찾을 수 없습니다.");
            }

            // tblIdx와 일치하는 상세 레코드를 선택합니다.
            var detailRecord = ngrMaintenanceList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 유지보수 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Maintenance/etc/Grounding/NGR/NGRMaintenanceDetail.cshtml", detailRecord);
        }

        // 기타 Ajax 액션 메서드들도 동일한 방식으로 수정합니다.
        [HttpPost]
        public JsonResult GetNGRMaintenance(string ngrCode)
        {
            if (string.IsNullOrEmpty(ngrCode))
            {
                return Json(new { success = false, message = "올바른 NGR_Code가 전달되지 않았습니다." });
            }
            List<NGRMaintenanceHistory> ngrMaintenanceList = new List<NGRMaintenanceHistory>();
            var result = ngrMaintenanceRepository.GetNGRMRByNGRCode(ngrCode, out ngrMaintenanceList);
            if (result == null || !result.IsSuccess)
            {
                return Json(new { success = false, message = "NGR 유지보수이력 정보를 찾을 수 없습니다." });
            }

            return Json(new { success = true, data = ngrMaintenanceList });
        }

        [HttpPost]
        public JsonResult NGRMaintenanceDelete(string ngrCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(ngrCode))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = ngrMaintenanceRepository.DeleteNGRMRRepo(ngrCode, tblIdx);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "NGR 유지보수이력 정보가 삭제되었습니다." });
            }
            else
            {
                return Json(new { success = false, message = "NGR 유지보수이력 정보 삭제 실패: " + result.Message });
            }
        }
    }
}