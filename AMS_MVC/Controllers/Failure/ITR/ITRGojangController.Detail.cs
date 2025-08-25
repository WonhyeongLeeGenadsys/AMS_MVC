using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class ITRGojangController : Controller
    {
        [HttpGet]
        public ActionResult ITRGojangDetail(string itrCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(itrCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("Index");
            }

            List<ITRFailureHistory> itrGojangList = new List<ITRFailureHistory>();
            var result = itrGojangRepository.GetITRFHDetailByITRCode(itrCode, tblIdx, out itrGojangList);

            if (!result.IsSuccess || itrGojangList == null || !itrGojangList.Any())
            {
                return HttpNotFound("ITR 고장이력 정보를 찾을 수 없습니다.");
            }

            // tblIdx와 일치하는 상세 레코드를 선택합니다.
            var detailRecord = itrGojangList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 고장 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Gojang/ITR/ITRGojangDetail.cshtml", detailRecord);
        }

        // 기타 Ajax 액션 메서드들도 동일한 방식으로 수정합니다.
        [HttpPost]
        public JsonResult GetITRGojang(string itrCode)
        {
            if (string.IsNullOrEmpty(itrCode))
            {
                return Json(new { success = false, message = "올바른 ITR_Code가 전달되지 않았습니다." });
            }
            List<ITRFailureHistory> itrGojangList = new List<ITRFailureHistory>();
            var result = itrGojangRepository.GetITRFHByITRCode(itrCode, out itrGojangList);
            if (result == null || !result.IsSuccess)
            {
                return Json(new { success = false, message = "ITR 고장이력 정보를 찾을 수 없습니다." });
            }

            return Json(new { success = true, data = itrGojangList });
        }

        [HttpPost]
        public JsonResult ITRGojangDelete(string itrCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(itrCode))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = itrGojangRepository.DeleteITRFHRepo(itrCode, tblIdx);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "ITR 고장이력 정보가 삭제되었습니다." });
            }
            else
            {
                return Json(new { success = false, message = "ITR 고장이력 정보 삭제 실패: " + result.Message });
            }
        }
    }
}