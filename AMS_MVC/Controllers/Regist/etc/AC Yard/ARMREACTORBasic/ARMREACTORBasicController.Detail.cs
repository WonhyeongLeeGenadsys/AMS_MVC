using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers
{
    public partial class ARMREACTORBasicController : Controller
    {
        // Ajax 요청: ARMREACTOR 기본정보 가져오기
        [HttpGet]

        public ActionResult ARMREACTORBasicDetail(string tblIdx)
        {
            if (string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("Index");
            }

            var armreactorInfo = armreactorBasicRepository.GetARMREACTORBasicInfoByTblIdxRepo(tblIdx);
            if (armreactorInfo == null)
            {
                return HttpNotFound("ARMREACTOR 기본정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Regist/etc/AC Yard/ARMREACTOR/ARMREACTORBasicDetail.cshtml", armreactorInfo); 
        }

        // ARMREACTOR 기본정보 조회 (Ajax 요청)
        [HttpPost]
        public JsonResult GetARMREACTORBasicInfo(string tblIdx)
        {
            if (string.IsNullOrEmpty(tblIdx))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = armreactorBasicRepository.GetARMREACTORBasicInfoByTblIdxRepo(tblIdx);
            if (result == null)
            {
                return Json(new { success = false, message = "ARMREACTOR 기본정보를 찾을 수 없습니다." });
            }

            return Json(new { success = true, data = result });
        }

        // ARMREACTOR 기본정보 삭제 (Ajax 요청)
        [HttpPost]
        public JsonResult DeleteARMREACTORBasicInfo(string tblIdx)
        {
            if (string.IsNullOrEmpty(tblIdx))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = armreactorBasicRepository.DeleteARMREACTORBasicInfoRepo(tblIdx);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "ARMREACTOR 기본정보가 삭제되었습니다." });
            }
            else
            {
                return Json(new { success = false, message = "ARMREACTOR 기본정보 삭제 실패: " + result.Message });
            }
        }
    }
}