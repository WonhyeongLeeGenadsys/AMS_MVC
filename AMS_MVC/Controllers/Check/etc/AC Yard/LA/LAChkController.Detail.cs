using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class LAChkController : Controller
    {
        // Ajax 요청: LA 보통점검 가져오기
        [HttpGet]
        public ActionResult LAChkDetail(string laCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(laCode))
            {
                return RedirectToAction("Index");
            }

            List<LAChk> laChkList = new List<LAChk>();
            var result = laChkRepository.GetLAChkDetailByLACode(laCode, tblIdx, out laChkList);

            if (!result.IsSuccess || laChkList == null || laChkList.Count == 0)
            {
                return HttpNotFound("LA 보통점검 정보를 찾을 수 없습니다.");
            }

            // tblIdx와 일치하는 상세 레코드를 선택합니다.
            var detailRecord = laChkList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 보통점검 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Check/etc/AC Yard/LA/LAChkDetail.cshtml", detailRecord);
        }

        // LA 보통점검 조회 (Ajax 요청)
        [HttpPost]
        public JsonResult GetLAChk(string laCode)
        {
            if (string.IsNullOrEmpty(laCode))
            {
                return Json(new { success = false, message = "올바른 LA_Code가 전달되지 않았습니다." });
            }
            List<LAChk> laChkList = new List<LAChk>();
            var result = laChkRepository.GetLAChkByLACode(laCode, out laChkList);
            if (result == null)
            {
                return Json(new { success = false, message = "LA 보통점검 정보를 찾을 수 없습니다." });
            }

            return Json(new { success = true, data = result });
        }

        // LA 보통점검 삭제 (Ajax 요청)
        [HttpPost]
        public JsonResult DeleteLAChk(string laCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(laCode))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = laChkRepository.DeleteLAChkInfoRepo(laCode, tblIdx);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "LA 보통점검 정보가 삭제되었습니다." });
            }
            else
            {
                return Json(new { success = false, message = "LA 보통점검 정보 삭제 실패: " + result.Message });
            }
        }
    }
}