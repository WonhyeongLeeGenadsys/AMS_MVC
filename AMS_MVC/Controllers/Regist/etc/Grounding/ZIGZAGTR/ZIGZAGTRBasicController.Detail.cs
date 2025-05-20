using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers
{
    public partial class ZIGZAGTRBasicController : Controller
    {
        // Ajax 요청: ZIGZAGTR 기본정보 가져오기
        [HttpGet]

        public ActionResult ZIGZAGTRBasicDetail(string tblIdx)
        {
            if (string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("Index");
            }

            var zigzagtrInfo = zigzagtrBasicRepository.GetZIGZAGTRBasicInfoByTblIdxRepo(tblIdx);
            if (zigzagtrInfo == null)
            {
                return HttpNotFound("ZIGZAGTR 기본정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Regist/etc/Grounding/ZIGZAGTR/ZIGZAGTRBasicDetail.cshtml", zigzagtrInfo); 
        }

        // ZIGZAGTR 기본정보 조회 (Ajax 요청)
        [HttpPost]
        public JsonResult GetZIGZAGTRBasicInfo(string tblIdx)
        {
            if (string.IsNullOrEmpty(tblIdx))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = zigzagtrBasicRepository.GetZIGZAGTRBasicInfoByTblIdxRepo(tblIdx);
            if (result == null)
            {
                return Json(new { success = false, message = "ZIGZAGTR 기본정보를 찾을 수 없습니다." });
            }

            return Json(new { success = true, data = result });
        }

        // ZIGZAGTR 기본정보 삭제 (Ajax 요청)
        [HttpPost]
        public JsonResult DeleteZIGZAGTRBasicInfo(string tblIdx)
        {
            if (string.IsNullOrEmpty(tblIdx))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = zigzagtrBasicRepository.DeleteZIGZAGTRBasicInfoRepo(tblIdx);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "ZIGZAGTR 기본정보가 삭제되었습니다." });
            }
            else
            {
                return Json(new { success = false, message = "ZIGZAGTR 기본정보 삭제 실패: " + result.Message });
            }
        }
    }
}