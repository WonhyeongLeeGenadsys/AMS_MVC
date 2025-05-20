using AMS_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Check
{
    public partial class WALLBUSHINGChkController : Controller
    {
        // Ajax 요청: WALLBUSHING 보통점검 가져오기
        [HttpGet]
        public ActionResult WALLBUSHINGChkDetail(string wallbushingCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(wallbushingCode))
            {
                return RedirectToAction("Index");
            }

            List<WALLBUSHINGChk> wallbushingChkList = new List<WALLBUSHINGChk>();
            var result = wallbushingChkRepository.GetWALLBUSHINGChkDetailByWALLBUSHINGCode(wallbushingCode, tblIdx, out wallbushingChkList);

            if (!result.IsSuccess || wallbushingChkList == null || wallbushingChkList.Count == 0)
            {
                return HttpNotFound("WALLBUSHING 보통점검 정보를 찾을 수 없습니다.");
            }

            // tblIdx와 일치하는 상세 레코드를 선택합니다.
            var detailRecord = wallbushingChkList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 보통점검 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Check/etc/AC Yard/WALLBUSHING/WALLBUSHINGChkDetail.cshtml", detailRecord);
        }

        // WALLBUSHING 보통점검 조회 (Ajax 요청)
        [HttpPost]
        public JsonResult GetWALLBUSHINGChk(string wallbushingCode)
        {
            if (string.IsNullOrEmpty(wallbushingCode))
            {
                return Json(new { success = false, message = "올바른 WALLBUSHING_Code가 전달되지 않았습니다." });
            }
            List<WALLBUSHINGChk> wallbushingChkList = new List<WALLBUSHINGChk>();
            var result = wallbushingChkRepository.GetWALLBUSHINGChkByWALLBUSHINGCode(wallbushingCode, out wallbushingChkList);
            if (result == null)
            {
                return Json(new { success = false, message = "WALLBUSHING 보통점검 정보를 찾을 수 없습니다." });
            }

            return Json(new { success = true, data = result });
        }

        // WALLBUSHING 보통점검 삭제 (Ajax 요청)
        [HttpPost]
        public JsonResult DeleteWALLBUSHINGChk(string wallbushingCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(wallbushingCode))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = wallbushingChkRepository.DeleteWALLBUSHINGChkInfoRepo(wallbushingCode, tblIdx);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "WALLBUSHING 보통점검 정보가 삭제되었습니다." });
            }
            else
            {
                return Json(new { success = false, message = "WALLBUSHING 보통점검 정보 삭제 실패: " + result.Message });
            }
        }
    }
}