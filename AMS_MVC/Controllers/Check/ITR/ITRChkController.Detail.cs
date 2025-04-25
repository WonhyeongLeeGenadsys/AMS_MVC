// Controllers/Check/ITRChkController.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AMS_MVC.Models;

namespace AMS_MVC.Controllers.Check
{
    public partial class ITRChkController : Controller
    {
        // 보통점검 
        [HttpGet]
        public ActionResult ITRChk1Detail(string itrCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(itrCode))
            {
                return RedirectToAction("Index");
            }

            List<ITRChk1> itrChkList = new List<ITRChk1>();
            var result = _chk1Repo.GetITRChk1DetailByITRCode(itrCode, tblIdx, out itrChkList);

            if (!result.IsSuccess || itrChkList == null || itrChkList.Count == 0)
            {
                return HttpNotFound("ITR 보통점검 정보를 찾을 수 없습니다.");
            }

            // tblIdx와 일치하는 상세 레코드를 선택합니다.
            var detailRecord = itrChkList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 보통점검 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Check/ITR/ITRChk1Detail.cshtml", detailRecord);
        }

        // ITR 보통점검 조회 (Ajax 요청)
        [HttpPost]
        public JsonResult GetITRChk1(string itrCode)
        {
            if (string.IsNullOrEmpty(itrCode))
            {
                return Json(new { success = false, message = "올바른 ITR_Code가 전달되지 않았습니다." });
            }
            List<ITRChk1> itrChkList = new List<ITRChk1>();
            var result = _chk1Repo.GetITRChk1ByITRCode(itrCode, out itrChkList);
            if (result == null)
            {
                return Json(new { success = false, message = "ITR 보통점검 정보를 찾을 수 없습니다." });
            }

            return Json(new { success = true, data = result });
        }

        // ITR 보통점검 삭제 (Ajax 요청)
        [HttpPost]
        public JsonResult DeleteITRChk1(string itrCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(itrCode))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = _chk1Repo.DeleteITRChk1InfoRepo(itrCode, tblIdx);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "ITR 보통점검 정보가 삭제되었습니다." });
            }
            else
            {
                return Json(new { success = false, message = "ITR 보통점검 정보 삭제 실패: " + result.Message });
            }
        }

        // 정밀점검
        // Ajax 요청: ITR 보통점검 가져오기
        [HttpGet]
        public ActionResult ITRChk2Detail(string itrCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(itrCode))
            {
                return RedirectToAction("Index");
            }

            List<ITRChk2> itrChkList = new List<ITRChk2>();
            var result = _chk2Repo.GetITRChk2DetailByITRCode(itrCode, tblIdx, out itrChkList);

            if (!result.IsSuccess || itrChkList == null || itrChkList.Count == 0)
            {
                return HttpNotFound("ITR 정밀점검 정보를 찾을 수 없습니다.");
            }

            // tblIdx와 일치하는 상세 레코드를 선택합니다.
            var detailRecord = itrChkList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 정밀점검 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Check/ITR/ITRChk2Detail.cshtml", detailRecord);
        }

        // ITR 정밀점검 조회 (Ajax 요청)
        [HttpPost]
        public JsonResult GetITRChk2(string itrCode)
        {
            if (string.IsNullOrEmpty(itrCode))
            {
                return Json(new { success = false, message = "올바른 ITR_Code가 전달되지 않았습니다." });
            }
            List<ITRChk2> itrChkList = new List<ITRChk2>();
            var result = _chk2Repo.GetITRChk2ByITRCode(itrCode, out itrChkList);
            if (result == null)
            {
                return Json(new { success = false, message = "ITR 정밀점검 정보를 찾을 수 없습니다." });
            }

            return Json(new { success = true, data = result });
        }

        // ITR 정밀점검 삭제 (Ajax 요청)
        [HttpPost]
        public JsonResult DeleteITRChk2(string itrCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(itrCode))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = _chk2Repo.DeleteITRChk2InfoRepo(itrCode, tblIdx);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "ITR 정밀점검 정보가 삭제되었습니다." });
            }
            else
            {
                return Json(new { success = false, message = "ITR 정밀점검 정보 삭제 실패: " + result.Message });
            }
        }
    }
}
