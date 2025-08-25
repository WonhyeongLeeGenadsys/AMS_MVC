using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class NGRChkController : Controller
    {
        // Ajax 요청: NGR 보통점검 가져오기
        [HttpGet]
        public ActionResult NGRChkDetail(string ngrCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(ngrCode))
            {
                return RedirectToAction("Index");
            }

            List<NGRChk> ngrChkList = new List<NGRChk>();
            var result = ngrChkRepository.GetNGRChkDetailByNGRCode(ngrCode, tblIdx, out ngrChkList);

            if (!result.IsSuccess || ngrChkList == null || ngrChkList.Count == 0)
            {
                return HttpNotFound("NGR 보통점검 정보를 찾을 수 없습니다.");
            }

            // tblIdx와 일치하는 상세 레코드를 선택합니다.
            var detailRecord = ngrChkList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 보통점검 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Check/etc/Grounding/NGR/NGRChkDetail.cshtml", detailRecord);
        }

        // NGR 보통점검 조회 (Ajax 요청)
        [HttpPost]
        public JsonResult GetNGRChk(string ngrCode)
        {
            if (string.IsNullOrEmpty(ngrCode))
            {
                return Json(new { success = false, message = "올바른 NGR_Code가 전달되지 않았습니다." });
            }
            List<NGRChk> ngrChkList = new List<NGRChk>();
            var result = ngrChkRepository.GetNGRChkByNGRCode(ngrCode, out ngrChkList);
            if (result == null)
            {
                return Json(new { success = false, message = "NGR 보통점검 정보를 찾을 수 없습니다." });
            }

            return Json(new { success = true, data = result });
        }

        // NGR 보통점검 삭제 (Ajax 요청)
        [HttpPost]
        public JsonResult DeleteNGRChk(string ngrCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(ngrCode))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = ngrChkRepository.DeleteNGRChkInfoRepo(ngrCode, tblIdx);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "NGR 보통점검 정보가 삭제되었습니다." });
            }
            else
            {
                return Json(new { success = false, message = "NGR 보통점검 정보 삭제 실패: " + result.Message });
            }
        }
    }
}