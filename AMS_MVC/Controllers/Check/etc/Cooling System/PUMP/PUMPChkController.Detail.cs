using AMS_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Check
{
    public partial class PUMPChkController : Controller
    {
        // Ajax 요청: PUMP 보통점검 가져오기
        [HttpGet]
        public ActionResult PUMPChkDetail(string pumpCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(pumpCode))
            {
                return RedirectToAction("Index");
            }

            List<PUMPChk> pumpChkList = new List<PUMPChk>();
            var result = pumpChkRepository.GetPUMPChkDetailByPUMPCode(pumpCode, tblIdx, out pumpChkList);

            if (!result.IsSuccess || pumpChkList == null || pumpChkList.Count == 0)
            {
                return HttpNotFound("PUMP 보통점검 정보를 찾을 수 없습니다.");
            }

            // tblIdx와 일치하는 상세 레코드를 선택합니다.
            var detailRecord = pumpChkList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 보통점검 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Check/etc/Cooling System/PUMP/PUMPChkDetail.cshtml", detailRecord);
        }

        // PUMP 보통점검 조회 (Ajax 요청)
        [HttpPost]
        public JsonResult GetPUMPChk(string pumpCode)
        {
            if (string.IsNullOrEmpty(pumpCode))
            {
                return Json(new { success = false, message = "올바른 PUMP_Code가 전달되지 않았습니다." });
            }
            List<PUMPChk> pumpChkList = new List<PUMPChk>();
            var result = pumpChkRepository.GetPUMPChkByPUMPCode(pumpCode, out pumpChkList);
            if (result == null)
            {
                return Json(new { success = false, message = "PUMP 보통점검 정보를 찾을 수 없습니다." });
            }

            return Json(new { success = true, data = result });
        }

        // PUMP 보통점검 삭제 (Ajax 요청)
        [HttpPost]
        public JsonResult DeletePUMPChk(string pumpCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(pumpCode))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = pumpChkRepository.DeletePUMPChkInfoRepo(pumpCode, tblIdx);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "PUMP 보통점검 정보가 삭제되었습니다." });
            }
            else
            {
                return Json(new { success = false, message = "PUMP 보통점검 정보 삭제 실패: " + result.Message });
            }
        }
    }
}