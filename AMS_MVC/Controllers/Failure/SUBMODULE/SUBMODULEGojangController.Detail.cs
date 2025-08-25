using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class SUBMODULEGojangController : Controller
    {
        // 예시: 상세보기 페이지 (여러 고장정보 중 tblIdx에 해당하는 레코드를 선택)
        [HttpGet]
        public ActionResult SUBMODULEGojangDetail(string submoduleCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(submoduleCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("Index");
            }

            List<SUBMODULEFailureHistory> submoduleGojangList = new List<SUBMODULEFailureHistory>();
            var result = submoduleGojangRepository.GetSUBMODULEFHDetailBySUBMODULECode(submoduleCode, tblIdx, out submoduleGojangList);

            if (!result.IsSuccess || submoduleGojangList == null || !submoduleGojangList.Any())
            {
                return HttpNotFound("SUBMODULE 고장이력 정보를 찾을 수 없습니다.");
            }

            // tblIdx와 일치하는 상세 레코드를 선택합니다.
            var detailRecord = submoduleGojangList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 고장 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Gojang/SUBMODULE/SUBMODULEGojangDetail.cshtml", detailRecord);
        }

        // 기타 Ajax 액션 메서드들도 동일한 방식으로 수정합니다.
        [HttpPost]
        public JsonResult GetSUBMODULEGojang(string submoduleCode)
        {
            if (string.IsNullOrEmpty(submoduleCode))
            {
                return Json(new { success = false, message = "올바른 SUBMODULE_Code가 전달되지 않았습니다." });
            }
            List<SUBMODULEFailureHistory> submoduleGojangList = new List<SUBMODULEFailureHistory>();
            var result = submoduleGojangRepository.GetSUBMODULEFHBySUBMODULECode(submoduleCode, out submoduleGojangList);
            if (result == null || !result.IsSuccess)
            {
                return Json(new { success = false, message = "SUBMODULE 고장이력 정보를 찾을 수 없습니다." });
            }

            return Json(new { success = true, data = submoduleGojangList });
        }

        [HttpPost]
        public JsonResult SUBMODULEGojangDelete(string submoduleCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(submoduleCode))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = submoduleGojangRepository.DeleteSUBMODULEFHRepo(submoduleCode, tblIdx);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "SUBMODULE 고장이력 정보가 삭제되었습니다." });
            }
            else
            {
                return Json(new { success = false, message = "SUBMODULE 고장이력 정보 삭제 실패: " + result.Message });
            }
        }
    }
}