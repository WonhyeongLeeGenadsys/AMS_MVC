using AMS_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers
{
    public partial class ARMREACTORGojangController : Controller
    {
        // 예시: 상세보기 페이지 (여러 고장정보 중 tblIdx에 해당하는 레코드를 선택)
        [HttpGet]
        public ActionResult ARMREACTORGojangDetail(string armreactorCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(armreactorCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("Index");
            }

            List<ARMREACTORFailureHistory> armreactorGojangList = new List<ARMREACTORFailureHistory>();
            var result = armreactorGojangRepository.GetARMREACTORFHDetailByARMREACTORCode(armreactorCode, tblIdx, out armreactorGojangList);

            if (!result.IsSuccess || armreactorGojangList == null || !armreactorGojangList.Any())
            {
                return HttpNotFound("ARMREACTOR 고장이력 정보를 찾을 수 없습니다.");
            }

            // tblIdx와 일치하는 상세 레코드를 선택합니다.
            var detailRecord = armreactorGojangList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 고장 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Gojang/etc/AC Yard/ARMREACTOR/ARMREACTORGojangDetail.cshtml", detailRecord);
        }

        // 기타 Ajax 액션 메서드들도 동일한 방식으로 수정합니다.
        [HttpPost]
        public JsonResult GetARMREACTORGojang(string armreactorCode)
        {
            if (string.IsNullOrEmpty(armreactorCode))
            {
                return Json(new { success = false, message = "올바른 ARMREACTOR_Code가 전달되지 않았습니다." });
            }
            List<ARMREACTORFailureHistory> armreactorGojangList = new List<ARMREACTORFailureHistory>();
            var result = armreactorGojangRepository.GetARMREACTORFHByARMREACTORCode(armreactorCode, out armreactorGojangList);
            if (result == null || !result.IsSuccess)
            {
                return Json(new { success = false, message = "ARMREACTOR 고장이력 정보를 찾을 수 없습니다." });
            }

            return Json(new { success = true, data = armreactorGojangList });
        }

        [HttpPost]
        public JsonResult ARMREACTORGojangDelete(string armreactorCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(armreactorCode))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = armreactorGojangRepository.DeleteARMREACTORFHRepo(armreactorCode, tblIdx);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "ARMREACTOR 고장이력 정보가 삭제되었습니다." });
            }
            else
            {
                return Json(new { success = false, message = "ARMREACTOR 고장이력 정보 삭제 실패: " + result.Message });
            }
        }
    }
}