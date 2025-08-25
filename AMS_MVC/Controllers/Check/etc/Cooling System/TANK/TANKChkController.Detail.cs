using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class TANKChkController : Controller
    {
        // Ajax 요청: TANK 보통점검 가져오기
        [HttpGet]
        public ActionResult TANKChkDetail(string vcbCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(vcbCode))
            {
                return RedirectToAction("Index");
            }

            List<TANKChk> vcbChkList = new List<TANKChk>();
            var result = vcbChkRepository.GetTANKChkDetailByTANKCode(vcbCode, tblIdx, out vcbChkList);

            if (!result.IsSuccess || vcbChkList == null || vcbChkList.Count == 0)
            {
                return HttpNotFound("TANK 보통점검 정보를 찾을 수 없습니다.");
            }

            // tblIdx와 일치하는 상세 레코드를 선택합니다.
            var detailRecord = vcbChkList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 보통점검 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Check/etc/Cooling System/TANK/TANKChkDetail.cshtml", detailRecord);
        }

        // TANK 보통점검 조회 (Ajax 요청)
        [HttpPost]
        public JsonResult GetTANKChk(string vcbCode)
        {
            if (string.IsNullOrEmpty(vcbCode))
            {
                return Json(new { success = false, message = "올바른 TANK_Code가 전달되지 않았습니다." });
            }
            List<TANKChk> vcbChkList = new List<TANKChk>();
            var result = vcbChkRepository.GetTANKChkByTANKCode(vcbCode, out vcbChkList);
            if (result == null)
            {
                return Json(new { success = false, message = "TANK 보통점검 정보를 찾을 수 없습니다." });
            }

            return Json(new { success = true, data = result });
        }

        // TANK 보통점검 삭제 (Ajax 요청)
        [HttpPost]
        public JsonResult DeleteTANKChk(string vcbCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(vcbCode))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = vcbChkRepository.DeleteTANKChkInfoRepo(vcbCode, tblIdx);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "TANK 보통점검 정보가 삭제되었습니다." });
            }
            else
            {
                return Json(new { success = false, message = "TANK 보통점검 정보 삭제 실패: " + result.Message });
            }
        }
    }
}