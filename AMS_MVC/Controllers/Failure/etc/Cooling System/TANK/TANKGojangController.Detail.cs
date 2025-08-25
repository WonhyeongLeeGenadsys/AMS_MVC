using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class TANKGojangController : Controller
    {
        // 예시: 상세보기 페이지 (여러 고장정보 중 tblIdx에 해당하는 레코드를 선택)
        [HttpGet]
        public ActionResult TANKGojangDetail(string tankCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(tankCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("Index");
            }

            List<TANKFailureHistory> tankGojangList = new List<TANKFailureHistory>();
            var result = tankGojangRepository.GetTANKFHDetailByTANKCode(tankCode, tblIdx, out tankGojangList);

            if (!result.IsSuccess || tankGojangList == null || !tankGojangList.Any())
            {
                return HttpNotFound("TANK 고장이력 정보를 찾을 수 없습니다.");
            }

            // tblIdx와 일치하는 상세 레코드를 선택합니다.
            var detailRecord = tankGojangList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 고장 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Gojang/etc/Cooling System/TANK/TANKGojangDetail.cshtml", detailRecord);
        }

        // 기타 Ajax 액션 메서드들도 동일한 방식으로 수정합니다.
        [HttpPost]
        public JsonResult GetTANKGojang(string tankCode)
        {
            if (string.IsNullOrEmpty(tankCode))
            {
                return Json(new { success = false, message = "올바른 TANK_Code가 전달되지 않았습니다." });
            }
            List<TANKFailureHistory> tankGojangList = new List<TANKFailureHistory>();
            var result = tankGojangRepository.GetTANKFHByTANKCode(tankCode, out tankGojangList);
            if (result == null || !result.IsSuccess)
            {
                return Json(new { success = false, message = "TANK 고장이력 정보를 찾을 수 없습니다." });
            }

            return Json(new { success = true, data = tankGojangList });
        }

        [HttpPost]
        public JsonResult TANKGojangDelete(string tankCode, string tblIdx)
        {
            if (string.IsNullOrEmpty(tankCode))
            {
                return Json(new { success = false, message = "올바른 Tbl_Idx가 전달되지 않았습니다." });
            }

            var result = tankGojangRepository.DeleteTANKFHRepo(tankCode, tblIdx);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "TANK 고장이력 정보가 삭제되었습니다." });
            }
            else
            {
                return Json(new { success = false, message = "TANK 고장이력 정보 삭제 실패: " + result.Message });
            }
        }
    }
}