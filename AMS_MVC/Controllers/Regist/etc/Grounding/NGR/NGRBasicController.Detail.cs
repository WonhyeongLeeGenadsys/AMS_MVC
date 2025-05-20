using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers
{
    public partial class NGRBasicController : Controller
    {
        // Ajax 요청: NGR 기본정보 가져오기
        [HttpGet]

        public ActionResult NGRBasicDetail(string tblIdx)
        {
            if (string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("Index");
            }

            var ngrInfo = ngrBasicRepository.GetNGRBasicInfoByTblIdxRepo(tblIdx);
            if (ngrInfo == null)
            {
                return HttpNotFound("NGR 기본정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Regist/etc/Grounding/NGR/NGRBasicDetail.cshtml", ngrInfo); 
        }

        // NGR 기본정보 조회 (Ajax 요청)
        [HttpPost]
        public JsonResult GetNGRBasicInfo(string tblIdx)
        {
            if (string.IsNullOrEmpty(tblIdx))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = ngrBasicRepository.GetNGRBasicInfoByTblIdxRepo(tblIdx);
            if (result == null)
            {
                return Json(new { success = false, message = "NGR 기본정보를 찾을 수 없습니다." });
            }

            return Json(new { success = true, data = result });
        }

        // NGR 기본정보 삭제 (Ajax 요청)
        [HttpPost]
        public JsonResult DeleteNGRBasicInfo(string tblIdx)
        {
            if (string.IsNullOrEmpty(tblIdx))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = ngrBasicRepository.DeleteNGRBasicInfoRepo(tblIdx);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "NGR 기본정보가 삭제되었습니다." });
            }
            else
            {
                return Json(new { success = false, message = "NGR 기본정보 삭제 실패: " + result.Message });
            }
        }
    }
}