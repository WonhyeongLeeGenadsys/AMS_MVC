using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class DCCTChkController : Controller
    {
        // Ajax 요청: DCCT 보통점검 가져오기
        [HttpGet]
        public ActionResult DCCTChkDetail(string dcctCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(dcctCode))
            {
                return RedirectToAction("Index");
            }

            List<DCCTChk> dcctChkList = new List<DCCTChk>();
            var result = dcctChkRepository.GetDCCTChkDetailByDCCTCode(dcctCode, tblIdx, out dcctChkList);

            if (!result.IsSuccess || dcctChkList == null || dcctChkList.Count == 0)
            {
                return HttpNotFound("DCCT 보통점검 정보를 찾을 수 없습니다.");
            }

            // tblIdx와 일치하는 상세 레코드를 선택합니다.
            var detailRecord = dcctChkList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 보통점검 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Check/etc/DC Yard/DCCT/DCCTChkDetail.cshtml", detailRecord);
        }

        // DCCT 보통점검 조회 (Ajax 요청)
        [HttpPost]
        public JsonResult GetDCCTChk(string dcctCode)
        {
            if (string.IsNullOrEmpty(dcctCode))
            {
                return Json(new { success = false, message = "올바른 DCCT_Code가 전달되지 않았습니다." });
            }
            List<DCCTChk> dcctChkList = new List<DCCTChk>();
            var result = dcctChkRepository.GetDCCTChkByDCCTCode(dcctCode, out dcctChkList);
            if (result == null)
            {
                return Json(new { success = false, message = "DCCT 보통점검 정보를 찾을 수 없습니다." });
            }

            return Json(new { success = true, data = result });
        }

        // DCCT 보통점검 삭제 (Ajax 요청)
        [HttpPost]
        public JsonResult DeleteDCCTChk(string dcctCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(dcctCode))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = dcctChkRepository.DeleteDCCTChkInfoRepo(dcctCode, tblIdx);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "DCCT 보통점검 정보가 삭제되었습니다." });
            }
            else
            {
                return Json(new { success = false, message = "DCCT 보통점검 정보 삭제 실패: " + result.Message });
            }
        }
    }
}