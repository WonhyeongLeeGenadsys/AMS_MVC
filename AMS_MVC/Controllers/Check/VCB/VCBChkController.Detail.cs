using AMS_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Check
{
    public partial class VCBChkController : Controller
    {
        // Ajax 요청: VCB 보통점검 가져오기
        [HttpGet]
        public ActionResult VCBChkDetail(string vcbCode)
        {
            if (string.IsNullOrEmpty(vcbCode))
            {
                return RedirectToAction("Index");
            }

            List<VCBChk> vcbChkList = new List<VCBChk>();
            var result = vcbChkRepository.GetVCBChkDetailByVCBCode(vcbCode, out vcbChkList);

            if (!result.IsSuccess || vcbChkList == null || vcbChkList.Count == 0)
            {
                return HttpNotFound("VCB 보통점검 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Check/VCB/VCBChkDetail.cshtml", vcbChkList);
        }

        // VCB 보통점검 조회 (Ajax 요청)
        [HttpPost]
        public JsonResult GetVCBChk(string vcbCode)
        {
            if (string.IsNullOrEmpty(vcbCode))
            {
                return Json(new { success = false, message = "올바른 VCB_Code가 전달되지 않았습니다." });
            }
            List<VCBChk> vcbChkList = new List<VCBChk>();
            var result = vcbChkRepository.GetVCBChkByVCBCode(vcbCode, out vcbChkList);
            if (result == null)
            {
                return Json(new { success = false, message = "VCB 보통점검 정보를 찾을 수 없습니다." });
            }

            return Json(new { success = true, data = result });
        }

        // VCB 보통점검 삭제 (Ajax 요청)
        [HttpPost]
        public JsonResult DeleteVCBChk(string vcbCode)
        {
            if (string.IsNullOrEmpty(vcbCode))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = vcbChkRepository.DeleteVCBChkInfoRepo(vcbCode);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "VCB 보통점검 정보가 삭제되었습니다." });
            }
            else
            {
                return Json(new { success = false, message = "VCB 보통점검 정보 삭제 실패: " + result.Message });
            }
        }
    }
}