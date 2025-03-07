using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers
{
    public partial class ITRBasicController : Controller
    {
        // Ajax 요청: VCB 기본정보 가져오기
        [HttpGet]

        public ActionResult ITRBasicDetail(string tblIdx)
        {
            if (string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("Index");
            }

            var itrInfo = itrBasicRepository.GetITRBasicInfoByTblIdxRepo(tblIdx);
            if (itrInfo == null)
            {
                return HttpNotFound("ITR 기본정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Regist/ITR/ITRBasicDetail.cshtml", itrInfo); // Model을 View로 전달
        }

        // VCB 기본정보 조회 (Ajax 요청)
        [HttpPost]
        public JsonResult GetITRBasicInfo(string tblIdx)
        {
            if (string.IsNullOrEmpty(tblIdx))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = itrBasicRepository.GetITRBasicInfoByTblIdxRepo(tblIdx);
            if (result == null)
            {
                return Json(new { success = false, message = "ITR 기본정보를 찾을 수 없습니다." });
            }

            return Json(new { success = true, data = result });
        }

        // VCB 기본정보 삭제 (Ajax 요청)
        [HttpPost]
        public JsonResult DeleteITRBasicInfo(string tblIdx)
        {
            if (string.IsNullOrEmpty(tblIdx))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = itrBasicRepository.DeleteITRBasicInfoRepo(tblIdx);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "ITR 기본정보가 삭제되었습니다." });
            }
            else
            {
                return Json(new { success = false, message = "ITR 기본정보 삭제 실패: " + result.Message });
            }
        }
    }
}