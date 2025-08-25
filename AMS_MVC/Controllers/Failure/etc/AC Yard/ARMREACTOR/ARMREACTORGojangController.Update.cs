
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class ARMREACTORGojangController : Controller
    {
        [HttpGet]
        public ActionResult ARMREACTORGojangUpdate(string armreactorCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(armreactorCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("ARMREACTORGojangTotalList");
            }

            // 고장 이력 조회
            var result = armreactorGojangRepository.GetARMREACTORFHDetailByARMREACTORCode(armreactorCode, tblIdx, out var armreactorGojangList);

            if (!result.IsSuccess || armreactorGojangList == null || !armreactorGojangList.Any())
            {
                return HttpNotFound("ARMREACTOR 고장이력 정보를 찾을 수 없습니다.");
            }

            var detailRecord = armreactorGojangList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 고장 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Gojang/etc/AC Yard/ARMREACTOR/ARMREACTORGojangUpdate.cshtml", detailRecord);
        }

        [HttpPost]
        public ActionResult ARMREACTORGojangUpdate(ARMREACTORFailureHistory model)
        {
            Result result = new Result(true);
            try
            {
                model.Fail_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";

                result = armreactorGojangRepository.UpdateARMREACTORFHRepo(model);
                if (!result.IsSuccess)
                {
                    result.Message = "ARMREACTOR 고장이력 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("ARMREACTORGojangUpdate Error", ex.Message);
            }

            return Json(result);
        }
    }
}
