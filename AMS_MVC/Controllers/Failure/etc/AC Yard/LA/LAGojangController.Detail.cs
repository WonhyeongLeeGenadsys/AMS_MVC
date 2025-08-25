using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class LAGojangController : Controller
    {
        // 예시: 상세보기 페이지 (여러 고장정보 중 tblIdx에 해당하는 레코드를 선택)
        [HttpGet]
        public ActionResult LAGojangDetail(string laCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(laCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("Index");
            }

            List<LAFailureHistory> laGojangList = new List<LAFailureHistory>();
            var result = laGojangRepository.GetLAFHDetailByLACode(laCode, tblIdx, out laGojangList);

            if (!result.IsSuccess || laGojangList == null || !laGojangList.Any())
            {
                return HttpNotFound("LA 고장이력 정보를 찾을 수 없습니다.");
            }

            // tblIdx와 일치하는 상세 레코드를 선택합니다.
            var detailRecord = laGojangList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 고장 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Gojang/etc/AC Yard/LA/LAGojangDetail.cshtml", detailRecord);
        }

        // 기타 Ajax 액션 메서드들도 동일한 방식으로 수정합니다.
        [HttpPost]
        public JsonResult GetLAGojang(string laCode)
        {
            if (string.IsNullOrEmpty(laCode))
            {
                return Json(new { success = false, message = "올바른 LA_Code가 전달되지 않았습니다." });
            }
            List<LAFailureHistory> laGojangList = new List<LAFailureHistory>();
            var result = laGojangRepository.GetLAFHByLACode(laCode, out laGojangList);
            if (result == null || !result.IsSuccess)
            {
                return Json(new { success = false, message = "LA 고장이력 정보를 찾을 수 없습니다." });
            }

            return Json(new { success = true, data = laGojangList });
        }

        [HttpPost]
        public JsonResult LAGojangDelete(string laCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(laCode))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = laGojangRepository.DeleteLAFHRepo(laCode, tblIdx);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "LA 고장이력 정보가 삭제되었습니다." });
            }
            else
            {
                return Json(new { success = false, message = "LA 고장이력 정보 삭제 실패: " + result.Message });
            }
        }
    }
}