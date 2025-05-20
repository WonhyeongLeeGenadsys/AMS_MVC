using AMS_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers
{
    public partial class DCCTGojangController : Controller
    {
        // 예시: 상세보기 페이지 (여러 고장정보 중 tblIdx에 해당하는 레코드를 선택)
        [HttpGet]
        public ActionResult DCCTGojangDetail(string dcctCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(dcctCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("Index");
            }

            List<DCCTFailureHistory> dcctGojangList = new List<DCCTFailureHistory>();
            var result = dcctGojangRepository.GetDCCTFHDetailByDCCTCode(dcctCode, tblIdx, out dcctGojangList);

            if (!result.IsSuccess || dcctGojangList == null || !dcctGojangList.Any())
            {
                return HttpNotFound("DCCT 고장이력 정보를 찾을 수 없습니다.");
            }

            // tblIdx와 일치하는 상세 레코드를 선택합니다.
            var detailRecord = dcctGojangList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 고장 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Gojang/etc/DC Yard/DCCT/DCCTGojangDetail.cshtml", detailRecord);
        }

        // 기타 Ajax 액션 메서드들도 동일한 방식으로 수정합니다.
        [HttpPost]
        public JsonResult GetDCCTGojang(string dcctCode)
        {
            if (string.IsNullOrEmpty(dcctCode))
            {
                return Json(new { success = false, message = "올바른 DCCT_Code가 전달되지 않았습니다." });
            }
            List<DCCTFailureHistory> dcctGojangList = new List<DCCTFailureHistory>();
            var result = dcctGojangRepository.GetDCCTFHByDCCTCode(dcctCode, out dcctGojangList);
            if (result == null || !result.IsSuccess)
            {
                return Json(new { success = false, message = "DCCT 고장이력 정보를 찾을 수 없습니다." });
            }

            return Json(new { success = true, data = dcctGojangList });
        }

        [HttpPost]
        public JsonResult DCCTGojangDelete(string dcctCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(dcctCode))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = dcctGojangRepository.DeleteDCCTFHRepo(dcctCode, tblIdx);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "DCCT 고장이력 정보가 삭제되었습니다." });
            }
            else
            {
                return Json(new { success = false, message = "DCCT 고장이력 정보 삭제 실패: " + result.Message });
            }
        }
    }
}