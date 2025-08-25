using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class VCBGojangController : Controller
    {
        // 예시: 상세보기 페이지 (여러 고장정보 중 tblIdx에 해당하는 레코드를 선택)
        [HttpGet]
        public ActionResult VCBGojangDetail(string vcbCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(vcbCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("Index");
            }

            List<VCBFailureHistory> vcbGojangList = new List<VCBFailureHistory>();
            var result = vcbGojangRepository.GetVCBFHDetailByVCBCode(vcbCode, tblIdx, out vcbGojangList);

            if (!result.IsSuccess || vcbGojangList == null || !vcbGojangList.Any())
            {
                return HttpNotFound("VCB 고장이력 정보를 찾을 수 없습니다.");
            }

            // tblIdx와 일치하는 상세 레코드를 선택합니다.
            var detailRecord = vcbGojangList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 고장 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Gojang/VCB/VCBGojangDetail.cshtml", detailRecord);
        }

        // 기타 Ajax 액션 메서드들도 동일한 방식으로 수정합니다.
        [HttpPost]
        public JsonResult GetVCBGojang(string vcbCode)
        {
            if (string.IsNullOrEmpty(vcbCode))
            {
                return Json(new { success = false, message = "올바른 VCB_Code가 전달되지 않았습니다." });
            }
            List<VCBFailureHistory> vcbGojangList = new List<VCBFailureHistory>();
            var result = vcbGojangRepository.GetVCBFHByVCBCode(vcbCode, out vcbGojangList);
            if (result == null || !result.IsSuccess)
            {
                return Json(new { success = false, message = "VCB 고장이력 정보를 찾을 수 없습니다." });
            }

            return Json(new { success = true, data = vcbGojangList });
        }

        [HttpPost]
        public JsonResult VCBGojangDelete(string vcbCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(vcbCode))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = vcbGojangRepository.DeleteVCBFHRepo(vcbCode, tblIdx);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "VCB 고장이력 정보가 삭제되었습니다." });
            }
            else
            {
                return Json(new { success = false, message = "VCB 고장이력 정보 삭제 실패: " + result.Message });
            }
        }
    }
}