
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class BYPASSVALVEGojangController : Controller
    {
        [HttpGet]
        public ActionResult BYPASSVALVEGojangUpdate(string bypassvalveCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(bypassvalveCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("BYPASSVALVEGojangTotalList");
            }

            // 고장 이력 조회
            var result = bypassvalveGojangRepository.GetBYPASSVALVEFHDetailByBYPASSVALVECode(bypassvalveCode, tblIdx, out var bypassvalveGojangList);

            if (!result.IsSuccess || bypassvalveGojangList == null || !bypassvalveGojangList.Any())
            {
                return HttpNotFound("BYPASSVALVE 고장이력 정보를 찾을 수 없습니다.");
            }

            var detailRecord = bypassvalveGojangList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 고장 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Gojang/etc/Cooling System/BYPASSVALVE/BYPASSVALVEGojangUpdate.cshtml", detailRecord);
        }

        [HttpPost]
        public ActionResult BYPASSVALVEGojangUpdate(BYPASSVALVEFailureHistory model)
        {
            Result result = new Result(true);
            try
            {
                model.Fail_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";

                result = bypassvalveGojangRepository.UpdateBYPASSVALVEFHRepo(model);
                if (!result.IsSuccess)
                {
                    result.Message = "BYPASSVALVE 고장이력 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("BYPASSVALVEGojangUpdate Error", ex.Message);
            }

            return Json(result);
        }
    }
}
