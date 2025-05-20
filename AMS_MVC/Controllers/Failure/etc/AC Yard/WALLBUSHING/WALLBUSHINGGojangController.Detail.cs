using AMS_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers
{
    public partial class WALLBUSHINGGojangController : Controller
    {
        // 예시: 상세보기 페이지 (여러 고장정보 중 tblIdx에 해당하는 레코드를 선택)
        [HttpGet]
        public ActionResult WALLBUSHINGGojangDetail(string wallbushingCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(wallbushingCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("Index");
            }

            List<WALLBUSHINGFailureHistory> wallbushingGojangList = new List<WALLBUSHINGFailureHistory>();
            var result = wallbushingGojangRepository.GetWALLBUSHINGFHDetailByWALLBUSHINGCode(wallbushingCode, tblIdx, out wallbushingGojangList);

            if (!result.IsSuccess || wallbushingGojangList == null || !wallbushingGojangList.Any())
            {
                return HttpNotFound("WALLBUSHING 고장이력 정보를 찾을 수 없습니다.");
            }

            // tblIdx와 일치하는 상세 레코드를 선택합니다.
            var detailRecord = wallbushingGojangList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 고장 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Gojang/etc/AC Yard/WALLBUSHING/WALLBUSHINGGojangDetail.cshtml", detailRecord);
        }

        // 기타 Ajax 액션 메서드들도 동일한 방식으로 수정합니다.
        [HttpPost]
        public JsonResult GetWALLBUSHINGGojang(string wallbushingCode)
        {
            if (string.IsNullOrEmpty(wallbushingCode))
            {
                return Json(new { success = false, message = "올바른 WALLBUSHING_Code가 전달되지 않았습니다." });
            }
            List<WALLBUSHINGFailureHistory> wallbushingGojangList = new List<WALLBUSHINGFailureHistory>();
            var result = wallbushingGojangRepository.GetWALLBUSHINGFHByWALLBUSHINGCode(wallbushingCode, out wallbushingGojangList);
            if (result == null || !result.IsSuccess)
            {
                return Json(new { success = false, message = "WALLBUSHING 고장이력 정보를 찾을 수 없습니다." });
            }

            return Json(new { success = true, data = wallbushingGojangList });
        }

        [HttpPost]
        public JsonResult WALLBUSHINGGojangDelete(string wallbushingCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(wallbushingCode))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = wallbushingGojangRepository.DeleteWALLBUSHINGFHRepo(wallbushingCode, tblIdx);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "WALLBUSHING 고장이력 정보가 삭제되었습니다." });
            }
            else
            {
                return Json(new { success = false, message = "WALLBUSHING 고장이력 정보 삭제 실패: " + result.Message });
            }
        }
    }
}