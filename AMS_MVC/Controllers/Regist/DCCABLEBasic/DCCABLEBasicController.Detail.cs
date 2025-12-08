using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC
{
    public partial class DCCABLEBasicController : Controller
    {
        // Ajax 요청: DCCABLE 기본정보 가져오기
        [HttpGet]

        public ActionResult DCCABLEBasicDetail(string tblIdx)
        {
            if (string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("Index");
            }

            var dccableInfo = dccableBasicRepository.GetDCCABLEBasicInfoByTblIdxRepo(tblIdx);
            if (dccableInfo == null)
            {
                return HttpNotFound("DCCABLE 기본정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Regist/DCCABLE/DCCABLEBasicDetail.cshtml", dccableInfo); 
        }

        // DCCABLE 기본정보 조회 (Ajax 요청)
        [HttpPost]
        public JsonResult GetDCCABLEBasicInfo(string tblIdx)
        {
            if (string.IsNullOrEmpty(tblIdx))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = dccableBasicRepository.GetDCCABLEBasicInfoByTblIdxRepo(tblIdx);
            if (result == null)
            {
                return Json(new { success = false, message = "DCCABLE 기본정보를 찾을 수 없습니다." });
            }

            return Json(new { success = true, data = result });
        }

        // DCCABLE 기본정보 삭제 (Ajax 요청)
        [HttpPost]
        public JsonResult DeleteDCCABLEBasicInfo(string tblIdx)
        {
            if (string.IsNullOrEmpty(tblIdx))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = dccableBasicRepository.DeleteDCCABLEBasicInfoRepo(tblIdx);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "DCCABLE 기본정보가 삭제되었습니다." });
            }
            else
            {
                return Json(new { success = false, message = "DCCABLE 기본정보 삭제 실패: " + result.Message });
            }
        }
    }
}