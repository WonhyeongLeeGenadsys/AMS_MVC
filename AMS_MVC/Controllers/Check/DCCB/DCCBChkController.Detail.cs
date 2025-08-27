using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class DCCBChkController : Controller
    {
        // Ajax 요청: DCCB 보통점검 가져오기
        [HttpGet]
        public ActionResult DCCBChkDetail(string dccbCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(dccbCode))
            {
                return RedirectToAction("Index");
            }

            List<DCCBChk> dccbChkList = new List<DCCBChk>();
            var result = dccbChkRepository.GetDCCBChkDetailByDCCBCode(dccbCode, tblIdx, out dccbChkList);

            if (!result.IsSuccess || dccbChkList == null || dccbChkList.Count == 0)
            {
                return HttpNotFound("DCCB 보통점검 정보를 찾을 수 없습니다.");
            }

            // tblIdx와 일치하는 상세 레코드를 선택합니다.
            var detailRecord = dccbChkList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 보통점검 정보를 찾을 수 없습니다.");
            }

            var basic = dccbBasicInfoRepository.GetDCCBBasicInfoByCode(dccbCode);
            ViewBag.Name = basic?.Name ?? "";
            ViewBag.SerialNo = basic?.Serial_No ?? "";

            return View("~/Views/Check/DCCB/DCCBChkDetail.cshtml", detailRecord);
        }

        // DCCB 보통점검 조회 (Ajax 요청)
        [HttpPost]
        public JsonResult GetDCCBChk(string dccbCode)
        {
            if (string.IsNullOrEmpty(dccbCode))
            {
                return Json(new { success = false, message = "올바른 DCCB_Code가 전달되지 않았습니다." });
            }
            List<DCCBChk> dccbChkList = new List<DCCBChk>();
            var result = dccbChkRepository.GetDCCBChkByDCCBCode(dccbCode, out dccbChkList);
            if (result == null)
            {
                return Json(new { success = false, message = "DCCB 보통점검 정보를 찾을 수 없습니다." });
            }

            return Json(new { success = true, data = result });
        }

        // DCCB 보통점검 삭제 (Ajax 요청)
        [HttpPost]
        public JsonResult DeleteDCCBChk(string dccbCode, string tblIdx)
        {
            if (string.IsNullOrWhiteSpace(dccbCode) || string.IsNullOrWhiteSpace(tblIdx))
                return Json(new { success = false, message = "dccbCode 또는 tblIdx가 전달되지 않았습니다." });

            var result = dccbChkRepository.DeleteDCCBChkInfoRepo(dccbCode, tblIdx);

            return Json(new
            {
                success = result.IsSuccess,
                message = result.Message
            });
        }
    }
}