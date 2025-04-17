using AMS_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Check
{
    public partial class DCCABLEChkController : Controller
    {
        // Ajax 요청: DCCABLE 보통점검 가져오기
        [HttpGet]
        public ActionResult DCCABLEChkDetail(string dccableCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(dccableCode))
            {
                return RedirectToAction("Index");
            }

            List<DCCABLEChk> dccableChkList = new List<DCCABLEChk>();
            var result = dccableChkRepository.GetDCCABLEChkDetailByDCCABLECode(dccableCode, tblIdx, out dccableChkList);

            if (!result.IsSuccess || dccableChkList == null || dccableChkList.Count == 0)
            {
                return HttpNotFound("DCCABLE 보통점검 정보를 찾을 수 없습니다.");
            }

            // tblIdx와 일치하는 상세 레코드를 선택합니다.
            var detailRecord = dccableChkList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 보통점검 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Check/DCCABLE/DCCABLEChkDetail.cshtml", detailRecord);
        }

        // DCCABLE 보통점검 조회 (Ajax 요청)
        [HttpPost]
        public JsonResult GetDCCABLEChk(string dccableCode)
        {
            if (string.IsNullOrEmpty(dccableCode))
            {
                return Json(new { success = false, message = "올바른 DCCABLE_Code가 전달되지 않았습니다." });
            }
            List<DCCABLEChk> dccableChkList = new List<DCCABLEChk>();
            var result = dccableChkRepository.GetDCCABLEChkByDCCABLECode(dccableCode, out dccableChkList);
            if (result == null)
            {
                return Json(new { success = false, message = "DCCABLE 보통점검 정보를 찾을 수 없습니다." });
            }

            return Json(new { success = true, data = result });
        }

        // DCCABLE 보통점검 삭제 (Ajax 요청)
        [HttpPost]
        public JsonResult DeleteDCCABLEChk(string dccableCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(dccableCode))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = dccableChkRepository.DeleteDCCABLEChkInfoRepo(dccableCode, tblIdx);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "DCCABLE 보통점검 정보가 삭제되었습니다." });
            }
            else
            {
                return Json(new { success = false, message = "DCCABLE 보통점검 정보 삭제 실패: " + result.Message });
            }
        }
    }
}