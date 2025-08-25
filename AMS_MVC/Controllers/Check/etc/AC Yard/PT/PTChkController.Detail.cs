using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class PTChkController : Controller
    {
        // Ajax 요청: PT 보통점검 가져오기
        [HttpGet]
        public ActionResult PTChkDetail(string ptCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(ptCode))
            {
                return RedirectToAction("Index");
            }

            List<PTChk> ptChkList = new List<PTChk>();
            var result = ptChkRepository.GetPTChkDetailByPTCode(ptCode, tblIdx, out ptChkList);

            if (!result.IsSuccess || ptChkList == null || ptChkList.Count == 0)
            {
                return HttpNotFound("PT 보통점검 정보를 찾을 수 없습니다.");
            }

            // tblIdx와 일치하는 상세 레코드를 선택합니다.
            var detailRecord = ptChkList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 보통점검 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Check/etc/AC Yard/PT/PTChkDetail.cshtml", detailRecord);
        }

        // PT 보통점검 조회 (Ajax 요청)
        [HttpPost]
        public JsonResult GetPTChk(string ptCode)
        {
            if (string.IsNullOrEmpty(ptCode))
            {
                return Json(new { success = false, message = "올바른 PT_Code가 전달되지 않았습니다." });
            }
            List<PTChk> ptChkList = new List<PTChk>();
            var result = ptChkRepository.GetPTChkByPTCode(ptCode, out ptChkList);
            if (result == null)
            {
                return Json(new { success = false, message = "PT 보통점검 정보를 찾을 수 없습니다." });
            }

            return Json(new { success = true, data = result });
        }

        // PT 보통점검 삭제 (Ajax 요청)
        [HttpPost]
        public JsonResult DeletePTChk(string ptCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(ptCode))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = ptChkRepository.DeletePTChkInfoRepo(ptCode, tblIdx);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "PT 보통점검 정보가 삭제되었습니다." });
            }
            else
            {
                return Json(new { success = false, message = "PT 보통점검 정보 삭제 실패: " + result.Message });
            }
        }
    }
}