using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class SAGojangController : Controller
    {
        // 예시: 상세보기 페이지 (여러 고장정보 중 tblIdx에 해당하는 레코드를 선택)
        [HttpGet]
        public ActionResult SAGojangDetail(string saCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(saCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("Index");
            }

            List<SAFailureHistory> saGojangList = new List<SAFailureHistory>();
            var result = saGojangRepository.GetSAFHDetailBySACode(saCode, tblIdx, out saGojangList);

            if (!result.IsSuccess || saGojangList == null || !saGojangList.Any())
            {
                return HttpNotFound("SA 고장이력 정보를 찾을 수 없습니다.");
            }

            // tblIdx와 일치하는 상세 레코드를 선택합니다.
            var detailRecord = saGojangList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 고장 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Gojang/etc/DC Yard/SA/SAGojangDetail.cshtml", detailRecord);
        }

        // 기타 Ajax 액션 메서드들도 동일한 방식으로 수정합니다.
        [HttpPost]
        public JsonResult GetSAGojang(string saCode)
        {
            if (string.IsNullOrEmpty(saCode))
            {
                return Json(new { success = false, message = "올바른 SA_Code가 전달되지 않았습니다." });
            }
            List<SAFailureHistory> saGojangList = new List<SAFailureHistory>();
            var result = saGojangRepository.GetSAFHBySACode(saCode, out saGojangList);
            if (result == null || !result.IsSuccess)
            {
                return Json(new { success = false, message = "SA 고장이력 정보를 찾을 수 없습니다." });
            }

            return Json(new { success = true, data = saGojangList });
        }

        [HttpPost]
        public JsonResult SAGojangDelete(string saCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(saCode))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = saGojangRepository.DeleteSAFHRepo(saCode, tblIdx);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "SA 고장이력 정보가 삭제되었습니다." });
            }
            else
            {
                return Json(new { success = false, message = "SA 고장이력 정보 삭제 실패: " + result.Message });
            }
        }
    }
}