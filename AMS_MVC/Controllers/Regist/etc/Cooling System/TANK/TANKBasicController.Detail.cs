using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers
{
    public partial class TANKBasicController : Controller
    {
        // Ajax 요청: TANK 기본정보 가져오기
        [HttpGet]

        public ActionResult TANKBasicDetail(string tblIdx)
        {
            if (string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("Index");
            }

            var tankInfo = tankBasicRepository.GetTANKBasicInfoByTblIdxRepo(tblIdx);
            if (tankInfo == null)
            {
                return HttpNotFound("TANK 기본정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Regist/etc/Cooling System/TANK/TANKBasicDetail.cshtml", tankInfo); 
        }

        // TANK 기본정보 조회 (Ajax 요청)
        [HttpPost]
        public JsonResult GetTANKBasicInfo(string tblIdx)
        {
            if (string.IsNullOrEmpty(tblIdx))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = tankBasicRepository.GetTANKBasicInfoByTblIdxRepo(tblIdx);
            if (result == null)
            {
                return Json(new { success = false, message = "TANK 기본정보를 찾을 수 없습니다." });
            }

            return Json(new { success = true, data = result });
        }

        // TANK 기본정보 삭제 (Ajax 요청)
        [HttpPost]
        public JsonResult DeleteTANKBasicInfo(string tblIdx)
        {
            if (string.IsNullOrEmpty(tblIdx))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = tankBasicRepository.DeleteTANKBasicInfoRepo(tblIdx);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "TANK 기본정보가 삭제되었습니다." });
            }
            else
            {
                return Json(new { success = false, message = "TANK 기본정보 삭제 실패: " + result.Message });
            }
        }
    }
}