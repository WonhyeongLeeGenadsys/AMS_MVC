using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class ESChkController : Controller
    {
        // Ajax 요청: ES 보통점검 가져오기
        [HttpGet]
        public ActionResult ESChkDetail(string esCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(esCode))
            {
                return RedirectToAction("Index");
            }

            List<ESChk> esChkList = new List<ESChk>();
            var result = esChkRepository.GetESChkDetailByESCode(esCode, tblIdx, out esChkList);

            if (!result.IsSuccess || esChkList == null || esChkList.Count == 0)
            {
                return HttpNotFound("ES 보통점검 정보를 찾을 수 없습니다.");
            }

            // tblIdx와 일치하는 상세 레코드를 선택합니다.
            var detailRecord = esChkList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 보통점검 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Check/etc/Grounding/ES/ESChkDetail.cshtml", detailRecord);
        }

        // ES 보통점검 조회 (Ajax 요청)
        [HttpPost]
        public JsonResult GetESChk(string esCode)
        {
            if (string.IsNullOrEmpty(esCode))
            {
                return Json(new { success = false, message = "올바른 ES_Code가 전달되지 않았습니다." });
            }
            List<ESChk> esChkList = new List<ESChk>();
            var result = esChkRepository.GetESChkByESCode(esCode, out esChkList);
            if (result == null)
            {
                return Json(new { success = false, message = "ES 보통점검 정보를 찾을 수 없습니다." });
            }

            return Json(new { success = true, data = result });
        }

        // ES 보통점검 삭제 (Ajax 요청)
        [HttpPost]
        public JsonResult DeleteESChk(string esCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(esCode))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = esChkRepository.DeleteESChkInfoRepo(esCode, tblIdx);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "ES 보통점검 정보가 삭제되었습니다." });
            }
            else
            {
                return Json(new { success = false, message = "ES 보통점검 정보 삭제 실패: " + result.Message });
            }
        }
    }
}