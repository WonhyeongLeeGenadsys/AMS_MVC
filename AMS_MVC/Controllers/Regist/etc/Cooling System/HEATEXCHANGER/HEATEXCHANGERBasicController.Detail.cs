using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC
{
    public partial class HEATEXCHANGERBasicController : Controller
    {
        // Ajax 요청: HEATEXCHANGER 기본정보 가져오기
        [HttpGet]

        public ActionResult HEATEXCHANGERBasicDetail(string tblIdx)
        {
            if (string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("Index");
            }

            var heatexchangerInfo = heatexchangerBasicRepository.GetHEATEXCHANGERBasicInfoByTblIdxRepo(tblIdx);
            if (heatexchangerInfo == null)
            {
                return HttpNotFound("HEATEXCHANGER 기본정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Regist/etc/Cooling System/HEATEXCHANGER/HEATEXCHANGERBasicDetail.cshtml", heatexchangerInfo); 
        }

        // HEATEXCHANGER 기본정보 조회 (Ajax 요청)
        [HttpPost]
        public JsonResult GetHEATEXCHANGERBasicInfo(string tblIdx)
        {
            if (string.IsNullOrEmpty(tblIdx))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = heatexchangerBasicRepository.GetHEATEXCHANGERBasicInfoByTblIdxRepo(tblIdx);
            if (result == null)
            {
                return Json(new { success = false, message = "HEATEXCHANGER 기본정보를 찾을 수 없습니다." });
            }

            return Json(new { success = true, data = result });
        }

        // HEATEXCHANGER 기본정보 삭제 (Ajax 요청)
        [HttpPost]
        public JsonResult DeleteHEATEXCHANGERBasicInfo(string tblIdx)
        {
            if (string.IsNullOrEmpty(tblIdx))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = heatexchangerBasicRepository.DeleteHEATEXCHANGERBasicInfoRepo(tblIdx);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "HEATEXCHANGER 기본정보가 삭제되었습니다." });
            }
            else
            {
                return Json(new { success = false, message = "HEATEXCHANGER 기본정보 삭제 실패: " + result.Message });
            }
        }
    }
}