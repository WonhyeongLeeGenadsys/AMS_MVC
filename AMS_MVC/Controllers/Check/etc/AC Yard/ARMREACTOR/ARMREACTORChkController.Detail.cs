using AMS_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Check
{
    public partial class ARMREACTORChkController : Controller
    {
        // Ajax 요청: ARMREACTOR 보통점검 가져오기
        [HttpGet]
        public ActionResult ARMREACTORChkDetail(string armreactorCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(armreactorCode))
            {
                return RedirectToAction("Index");
            }

            List<ARMREACTORChk> armreactorChkList = new List<ARMREACTORChk>();
            var result = armreactorChkRepository.GetARMREACTORChkDetailByARMREACTORCode(armreactorCode, tblIdx, out armreactorChkList);

            if (!result.IsSuccess || armreactorChkList == null || armreactorChkList.Count == 0)
            {
                return HttpNotFound("ARMREACTOR 보통점검 정보를 찾을 수 없습니다.");
            }

            // tblIdx와 일치하는 상세 레코드를 선택합니다.
            var detailRecord = armreactorChkList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 보통점검 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Check/etc/AC Yard/ARMREACTOR/ARMREACTORChkDetail.cshtml", detailRecord);
        }

        // ARMREACTOR 보통점검 조회 (Ajax 요청)
        [HttpPost]
        public JsonResult GetARMREACTORChk(string armreactorCode)
        {
            if (string.IsNullOrEmpty(armreactorCode))
            {
                return Json(new { success = false, message = "올바른 ARMREACTOR_Code가 전달되지 않았습니다." });
            }
            List<ARMREACTORChk> armreactorChkList = new List<ARMREACTORChk>();
            var result = armreactorChkRepository.GetARMREACTORChkByARMREACTORCode(armreactorCode, out armreactorChkList);
            if (result == null)
            {
                return Json(new { success = false, message = "ARMREACTOR 보통점검 정보를 찾을 수 없습니다." });
            }

            return Json(new { success = true, data = result });
        }

        // ARMREACTOR 보통점검 삭제 (Ajax 요청)
        [HttpPost]
        public JsonResult DeleteARMREACTORChk(string armreactorCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(armreactorCode))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = armreactorChkRepository.DeleteARMREACTORChkInfoRepo(armreactorCode, tblIdx);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "ARMREACTOR 보통점검 정보가 삭제되었습니다." });
            }
            else
            {
                return Json(new { success = false, message = "ARMREACTOR 보통점검 정보 삭제 실패: " + result.Message });
            }
        }
    }
}