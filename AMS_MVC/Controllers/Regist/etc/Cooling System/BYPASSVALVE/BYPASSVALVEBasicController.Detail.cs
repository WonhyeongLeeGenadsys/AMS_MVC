using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC
{
    public partial class BYPASSVALVEBasicController : Controller
    {
        // Ajax 요청: BYPASSVALVE 기본정보 가져오기
        [HttpGet]

        public ActionResult BYPASSVALVEBasicDetail(string tblIdx)
        {
            if (string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("Index");
            }

            var bypassvalveInfo = bypassvalveBasicRepository.GetBYPASSVALVEBasicInfoByTblIdxRepo(tblIdx);
            if (bypassvalveInfo == null)
            {
                return HttpNotFound("BYPASSVALVE 기본정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Regist/etc/Cooling System/BYPASSVALVE/BYPASSVALVEBasicDetail.cshtml", bypassvalveInfo); 
        }

        // BYPASSVALVE 기본정보 조회 (Ajax 요청)
        [HttpPost]
        public JsonResult GetBYPASSVALVEBasicInfo(string tblIdx)
        {
            if (string.IsNullOrEmpty(tblIdx))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = bypassvalveBasicRepository.GetBYPASSVALVEBasicInfoByTblIdxRepo(tblIdx);
            if (result == null)
            {
                return Json(new { success = false, message = "BYPASSVALVE 기본정보를 찾을 수 없습니다." });
            }

            return Json(new { success = true, data = result });
        }

        // BYPASSVALVE 기본정보 삭제 (Ajax 요청)
        [HttpPost]
        public JsonResult DeleteBYPASSVALVEBasicInfo(string tblIdx)
        {
            if (string.IsNullOrEmpty(tblIdx))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = bypassvalveBasicRepository.DeleteBYPASSVALVEBasicInfoRepo(tblIdx);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "BYPASSVALVE 기본정보가 삭제되었습니다." });
            }
            else
            {
                return Json(new { success = false, message = "BYPASSVALVE 기본정보 삭제 실패: " + result.Message });
            }
        }
    }
}