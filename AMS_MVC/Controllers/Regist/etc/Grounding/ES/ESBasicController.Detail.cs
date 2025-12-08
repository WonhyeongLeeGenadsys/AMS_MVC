using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC
{
    public partial class ESBasicController : Controller
    {
        // Ajax 요청: ES 기본정보 가져오기
        [HttpGet]

        public ActionResult ESBasicDetail(string tblIdx)
        {
            if (string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("Index");
            }

            var esInfo = esBasicRepository.GetESBasicInfoByTblIdxRepo(tblIdx);
            if (esInfo == null)
            {
                return HttpNotFound("ES 기본정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Regist/etc/Grounding/ES/ESBasicDetail.cshtml", esInfo); 
        }

        // ES 기본정보 조회 (Ajax 요청)
        [HttpPost]
        public JsonResult GetESBasicInfo(string tblIdx)
        {
            if (string.IsNullOrEmpty(tblIdx))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = esBasicRepository.GetESBasicInfoByTblIdxRepo(tblIdx);
            if (result == null)
            {
                return Json(new { success = false, message = "ES 기본정보를 찾을 수 없습니다." });
            }

            return Json(new { success = true, data = result });
        }

        // ES 기본정보 삭제 (Ajax 요청)
        [HttpPost]
        public JsonResult DeleteESBasicInfo(string tblIdx)
        {
            if (string.IsNullOrEmpty(tblIdx))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = esBasicRepository.DeleteESBasicInfoRepo(tblIdx);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "ES 기본정보가 삭제되었습니다." });
            }
            else
            {
                return Json(new { success = false, message = "ES 기본정보 삭제 실패: " + result.Message });
            }
        }
    }
}