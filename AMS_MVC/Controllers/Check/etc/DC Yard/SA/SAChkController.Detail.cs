using AMS_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Check
{
    public partial class SAChkController : Controller
    {
        // Ajax 요청: SA 보통점검 가져오기
        [HttpGet]
        public ActionResult SAChkDetail(string saCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(saCode))
            {
                return RedirectToAction("Index");
            }

            List<SAChk> saChkList = new List<SAChk>();
            var result = saChkRepository.GetSAChkDetailBySACode(saCode, tblIdx, out saChkList);

            if (!result.IsSuccess || saChkList == null || saChkList.Count == 0)
            {
                return HttpNotFound("SA 보통점검 정보를 찾을 수 없습니다.");
            }

            // tblIdx와 일치하는 상세 레코드를 선택합니다.
            var detailRecord = saChkList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 보통점검 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Check/etc/DC Yard/SA/SAChkDetail.cshtml", detailRecord);
        }

        // SA 보통점검 조회 (Ajax 요청)
        [HttpPost]
        public JsonResult GetSAChk(string saCode)
        {
            if (string.IsNullOrEmpty(saCode))
            {
                return Json(new { success = false, message = "올바른 SA_Code가 전달되지 않았습니다." });
            }
            List<SAChk> saChkList = new List<SAChk>();
            var result = saChkRepository.GetSAChkBySACode(saCode, out saChkList);
            if (result == null)
            {
                return Json(new { success = false, message = "SA 보통점검 정보를 찾을 수 없습니다." });
            }

            return Json(new { success = true, data = result });
        }

        // SA 보통점검 삭제 (Ajax 요청)
        [HttpPost]
        public JsonResult DeleteSAChk(string saCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(saCode))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = saChkRepository.DeleteSAChkInfoRepo(saCode, tblIdx);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "SA 보통점검 정보가 삭제되었습니다." });
            }
            else
            {
                return Json(new { success = false, message = "SA 보통점검 정보 삭제 실패: " + result.Message });
            }
        }
    }
}