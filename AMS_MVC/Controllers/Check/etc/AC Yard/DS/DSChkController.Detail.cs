using AMS_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Check
{
    public partial class DSChkController : Controller
    {
        // Ajax 요청: DS 보통점검 가져오기
        [HttpGet]
        public ActionResult DSChkDetail(string dsCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(dsCode))
            {
                return RedirectToAction("Index");
            }

            List<DSChk> dsChkList = new List<DSChk>();
            var result = dsChkRepository.GetDSChkDetailByDSCode(dsCode, tblIdx, out dsChkList);

            if (!result.IsSuccess || dsChkList == null || dsChkList.Count == 0)
            {
                return HttpNotFound("DS 보통점검 정보를 찾을 수 없습니다.");
            }

            // tblIdx와 일치하는 상세 레코드를 선택합니다.
            var detailRecord = dsChkList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 보통점검 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Check/etc/AC Yard/DS/DSChkDetail.cshtml", detailRecord);
        }

        // DS 보통점검 조회 (Ajax 요청)
        [HttpPost]
        public JsonResult GetDSChk(string dsCode)
        {
            if (string.IsNullOrEmpty(dsCode))
            {
                return Json(new { success = false, message = "올바른 DS_Code가 전달되지 않았습니다." });
            }
            List<DSChk> dsChkList = new List<DSChk>();
            var result = dsChkRepository.GetDSChkByDSCode(dsCode, out dsChkList);
            if (result == null)
            {
                return Json(new { success = false, message = "DS 보통점검 정보를 찾을 수 없습니다." });
            }

            return Json(new { success = true, data = result });
        }

        // DS 보통점검 삭제 (Ajax 요청)
        [HttpPost]
        public JsonResult DeleteDSChk(string dsCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(dsCode))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = dsChkRepository.DeleteDSChkInfoRepo(dsCode, tblIdx);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "DS 보통점검 정보가 삭제되었습니다." });
            }
            else
            {
                return Json(new { success = false, message = "DS 보통점검 정보 삭제 실패: " + result.Message });
            }
        }
    }
}