using AMS_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers
{
    public partial class CTGojangController : Controller
    {
        // 예시: 상세보기 페이지 (여러 고장정보 중 tblIdx에 해당하는 레코드를 선택)
        [HttpGet]
        public ActionResult CTGojangDetail(string ctCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(ctCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("Index");
            }

            List<CTFailureHistory> ctGojangList = new List<CTFailureHistory>();
            var result = ctGojangRepository.GetCTFHDetailByCTCode(ctCode, tblIdx, out ctGojangList);

            if (!result.IsSuccess || ctGojangList == null || !ctGojangList.Any())
            {
                return HttpNotFound("CT 고장이력 정보를 찾을 수 없습니다.");
            }

            // tblIdx와 일치하는 상세 레코드를 선택합니다.
            var detailRecord = ctGojangList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 고장 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Gojang/etc/AC Yard/CT/CTGojangDetail.cshtml", detailRecord);
        }

        // 기타 Ajax 액션 메서드들도 동일한 방식으로 수정합니다.
        [HttpPost]
        public JsonResult GetCTGojang(string ctCode)
        {
            if (string.IsNullOrEmpty(ctCode))
            {
                return Json(new { success = false, message = "올바른 CT_Code가 전달되지 않았습니다." });
            }
            List<CTFailureHistory> ctGojangList = new List<CTFailureHistory>();
            var result = ctGojangRepository.GetCTFHByCTCode(ctCode, out ctGojangList);
            if (result == null || !result.IsSuccess)
            {
                return Json(new { success = false, message = "CT 고장이력 정보를 찾을 수 없습니다." });
            }

            return Json(new { success = true, data = ctGojangList });
        }

        [HttpPost]
        public JsonResult CTGojangDelete(string ctCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(ctCode))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = ctGojangRepository.DeleteCTFHRepo(ctCode, tblIdx);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "CT 고장이력 정보가 삭제되었습니다." });
            }
            else
            {
                return Json(new { success = false, message = "CT 고장이력 정보 삭제 실패: " + result.Message });
            }
        }
    }
}