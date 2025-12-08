using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class BYPASSVALVEChkController : Controller
    {
        // Ajax 요청: BYPASSVALVE 보통점검 가져오기
        [HttpGet]
        public ActionResult BYPASSVALVEChkDetail(string bypassvalveCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(bypassvalveCode))
            {
                return RedirectToAction("Index");
            }

            List<BYPASSVALVEChk> bypassvalveChkList = new List<BYPASSVALVEChk>();
            var result = bypassvalveChkRepository.GetBYPASSVALVEChkDetailByBYPASSVALVECode(bypassvalveCode, tblIdx, out bypassvalveChkList);

            if (!result.IsSuccess || bypassvalveChkList == null || bypassvalveChkList.Count == 0)
            {
                return HttpNotFound("BYPASSVALVE 보통점검 정보를 찾을 수 없습니다.");
            }

            // tblIdx와 일치하는 상세 레코드를 선택합니다.
            var detailRecord = bypassvalveChkList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 보통점검 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Check/etc/Cooling System/BYPASSVALVE/BYPASSVALVEChkDetail.cshtml", detailRecord);
        }

        // BYPASSVALVE 보통점검 조회 (Ajax 요청)
        [HttpPost]
        public JsonResult GetBYPASSVALVEChk(string bypassvalveCode)
        {
            if (string.IsNullOrEmpty(bypassvalveCode))
            {
                return Json(new { success = false, message = "올바른 BYPASSVALVE_Code가 전달되지 않았습니다." });
            }
            List<BYPASSVALVEChk> bypassvalveChkList = new List<BYPASSVALVEChk>();
            var result = bypassvalveChkRepository.GetBYPASSVALVEChkByBYPASSVALVECode(bypassvalveCode, out bypassvalveChkList);
            if (result == null)
            {
                return Json(new { success = false, message = "BYPASSVALVE 보통점검 정보를 찾을 수 없습니다." });
            }

            return Json(new { success = true, data = result });
        }

        // BYPASSVALVE 보통점검 삭제 (Ajax 요청)
        [HttpPost]
        public JsonResult DeleteBYPASSVALVEChk(string bypassvalveCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(bypassvalveCode))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = bypassvalveChkRepository.DeleteBYPASSVALVEChkInfoRepo(bypassvalveCode, tblIdx);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "BYPASSVALVE 보통점검 정보가 삭제되었습니다." });
            }
            else
            {
                return Json(new { success = false, message = "BYPASSVALVE 보통점검 정보 삭제 실패: " + result.Message });
            }
        }
    }
}