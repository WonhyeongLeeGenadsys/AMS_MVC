using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class CTChkController : Controller
    {
        // Ajax 요청: CT 보통점검 가져오기
        [HttpGet]
        public ActionResult CTChkDetail(string ctCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(ctCode))
            {
                return RedirectToAction("Index");
            }

            List<CTChk> ctChkList = new List<CTChk>();
            var result = ctChkRepository.GetCTChkDetailByCTCode(ctCode, tblIdx, out ctChkList);

            if (!result.IsSuccess || ctChkList == null || ctChkList.Count == 0)
            {
                return HttpNotFound("CT 보통점검 정보를 찾을 수 없습니다.");
            }

            // tblIdx와 일치하는 상세 레코드를 선택합니다.
            var detailRecord = ctChkList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 보통점검 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Check/etc/AC Yard/CT/CTChkDetail.cshtml", detailRecord);
        }

        // CT 보통점검 조회 (Ajax 요청)
        [HttpPost]
        public JsonResult GetCTChk(string ctCode)
        {
            if (string.IsNullOrEmpty(ctCode))
            {
                return Json(new { success = false, message = "올바른 CT_Code가 전달되지 않았습니다." });
            }
            List<CTChk> ctChkList = new List<CTChk>();
            var result = ctChkRepository.GetCTChkByCTCode(ctCode, out ctChkList);
            if (result == null)
            {
                return Json(new { success = false, message = "CT 보통점검 정보를 찾을 수 없습니다." });
            }

            return Json(new { success = true, data = result });
        }

        // CT 보통점검 삭제 (Ajax 요청)
        [HttpPost]
        public JsonResult DeleteCTChk(string ctCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(ctCode))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = ctChkRepository.DeleteCTChkInfoRepo(ctCode, tblIdx);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "CT 보통점검 정보가 삭제되었습니다." });
            }
            else
            {
                return Json(new { success = false, message = "CT 보통점검 정보 삭제 실패: " + result.Message });
            }
        }
    }
}