using AMS_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Check
{
    public partial class ZIGZAGTRChkController : Controller
    {
        // Ajax 요청: ZIGZAGTR 보통점검 가져오기
        [HttpGet]
        public ActionResult ZIGZAGTRChkDetail(string zigzagtrCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(zigzagtrCode))
            {
                return RedirectToAction("Index");
            }

            List<ZIGZAGTRChk> zigzagtrChkList = new List<ZIGZAGTRChk>();
            var result = zigzagtrChkRepository.GetZIGZAGTRChkDetailByZIGZAGTRCode(zigzagtrCode, tblIdx, out zigzagtrChkList);

            if (!result.IsSuccess || zigzagtrChkList == null || zigzagtrChkList.Count == 0)
            {
                return HttpNotFound("ZIGZAGTR 보통점검 정보를 찾을 수 없습니다.");
            }

            // tblIdx와 일치하는 상세 레코드를 선택합니다.
            var detailRecord = zigzagtrChkList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 보통점검 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Check/etc/Grounding/ZIGZAGTR/ZIGZAGTRChkDetail.cshtml", detailRecord);
        }

        // ZIGZAGTR 보통점검 조회 (Ajax 요청)
        [HttpPost]
        public JsonResult GetZIGZAGTRChk(string zigzagtrCode)
        {
            if (string.IsNullOrEmpty(zigzagtrCode))
            {
                return Json(new { success = false, message = "올바른 ZIGZAGTR_Code가 전달되지 않았습니다." });
            }
            List<ZIGZAGTRChk> zigzagtrChkList = new List<ZIGZAGTRChk>();
            var result = zigzagtrChkRepository.GetZIGZAGTRChkByZIGZAGTRCode(zigzagtrCode, out zigzagtrChkList);
            if (result == null)
            {
                return Json(new { success = false, message = "ZIGZAGTR 보통점검 정보를 찾을 수 없습니다." });
            }

            return Json(new { success = true, data = result });
        }

        // ZIGZAGTR 보통점검 삭제 (Ajax 요청)
        [HttpPost]
        public JsonResult DeleteZIGZAGTRChk(string zigzagtrCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(zigzagtrCode))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = zigzagtrChkRepository.DeleteZIGZAGTRChkInfoRepo(zigzagtrCode, tblIdx);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "ZIGZAGTR 보통점검 정보가 삭제되었습니다." });
            }
            else
            {
                return Json(new { success = false, message = "ZIGZAGTR 보통점검 정보 삭제 실패: " + result.Message });
            }
        }
    }
}