using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers
{
    public partial class PTBasicController : Controller
    {
        // Ajax 요청: PT 기본정보 가져오기
        [HttpGet]

        public ActionResult PTBasicDetail(string tblIdx)
        {
            if (string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("Index");
            }

            var ptInfo = ptBasicRepository.GetPTBasicInfoByTblIdxRepo(tblIdx);
            if (ptInfo == null)
            {
                return HttpNotFound("PT 기본정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Regist/etc/AC Yard/PT/PTBasicDetail.cshtml", ptInfo); 
        }

        // PT 기본정보 조회 (Ajax 요청)
        [HttpPost]
        public JsonResult GetPTBasicInfo(string tblIdx)
        {
            if (string.IsNullOrEmpty(tblIdx))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = ptBasicRepository.GetPTBasicInfoByTblIdxRepo(tblIdx);
            if (result == null)
            {
                return Json(new { success = false, message = "PT 기본정보를 찾을 수 없습니다." });
            }

            return Json(new { success = true, data = result });
        }

        // PT 기본정보 삭제 (Ajax 요청)
        [HttpPost]
        public JsonResult DeletePTBasicInfo(string tblIdx)
        {
            if (string.IsNullOrEmpty(tblIdx))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = ptBasicRepository.DeletePTBasicInfoRepo(tblIdx);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "PT 기본정보가 삭제되었습니다." });
            }
            else
            {
                return Json(new { success = false, message = "PT 기본정보 삭제 실패: " + result.Message });
            }
        }
    }
}