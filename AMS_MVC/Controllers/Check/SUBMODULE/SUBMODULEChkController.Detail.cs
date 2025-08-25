using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class SUBMODULEChkController : Controller
    {
        // Ajax 요청: SUBMODULE 보통점검 가져오기
        [HttpGet]
        public ActionResult SUBMODULEChkDetail(string submoduleCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(submoduleCode))
            {
                return RedirectToAction("Index");
            }

            List<SUBMODULEChk> submoduleChkList = new List<SUBMODULEChk>();
            var result = submoduleChkRepository.GetSUBMODULEChkDetailBySUBMODULECode(submoduleCode, tblIdx, out submoduleChkList);

            if (!result.IsSuccess || submoduleChkList == null || submoduleChkList.Count == 0)
            {
                return HttpNotFound("SUBMODULE 보통점검 정보를 찾을 수 없습니다.");
            }

            // tblIdx와 일치하는 상세 레코드를 선택합니다.
            var detailRecord = submoduleChkList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 보통점검 정보를 찾을 수 없습니다.");
            }

            var basic = submoduleBasicInfoRepository.GetSUBMODULEBasicInfoByCode(submoduleCode);
            ViewBag.Name = basic?.Name ?? "";
            ViewBag.SerialNo = basic?.Serial_No ?? "";

            return View("~/Views/Check/SUBMODULE/SUBMODULEChkDetail.cshtml", detailRecord);
        }

        // SUBMODULE 보통점검 조회 (Ajax 요청)
        [HttpPost]
        public JsonResult GetSUBMODULEChk(string submoduleCode)
        {
            if (string.IsNullOrEmpty(submoduleCode))
            {
                return Json(new { success = false, message = "올바른 SUBMODULE_Code가 전달되지 않았습니다." });
            }
            List<SUBMODULEChk> submoduleChkList = new List<SUBMODULEChk>();
            var result = submoduleChkRepository.GetSUBMODULEChkBySUBMODULECode(submoduleCode, out submoduleChkList);
            if (result == null)
            {
                return Json(new { success = false, message = "SUBMODULE 보통점검 정보를 찾을 수 없습니다." });
            }

            return Json(new { success = true, data = result });
        }

        // SUBMODULE 보통점검 삭제 (Ajax 요청)
        [HttpPost]
        public JsonResult DeleteSUBMODULEChk(string submoduleCode, string tblIdx)
        {
            if (string.IsNullOrWhiteSpace(submoduleCode) || string.IsNullOrWhiteSpace(tblIdx))
                return Json(new { success = false, message = "vcbCode 또는 tblIdx가 전달되지 않았습니다." });

            var result = submoduleChkRepository.DeleteSUBMODULEChkInfoRepo(submoduleCode, tblIdx);

            return Json(new
            {
                success = result.IsSuccess,
                message = result.Message
            });
        }
    }
}