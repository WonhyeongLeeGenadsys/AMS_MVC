
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class NGRGojangController : Controller
    {
        [HttpGet]
        public ActionResult NGRGojangUpdate(string ngrCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(ngrCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("NGRGojangTotalList");
            }

            // 고장 이력 조회
            var result = ngrGojangRepository.GetNGRFHDetailByNGRCode(ngrCode, tblIdx, out var ngrGojangList);

            if (!result.IsSuccess || ngrGojangList == null || !ngrGojangList.Any())
            {
                return HttpNotFound("NGR 고장이력 정보를 찾을 수 없습니다.");
            }

            var detailRecord = ngrGojangList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 고장 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Gojang/etc/Grounding/NGR/NGRGojangUpdate.cshtml", detailRecord);
        }

        [HttpPost]
        public ActionResult NGRGojangUpdate(NGRFailureHistory model)
        {
            Result result = new Result(true);
            try
            {
                model.Fail_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";

                result = ngrGojangRepository.UpdateNGRFHRepo(model);
                if (!result.IsSuccess)
                {
                    result.Message = "NGR 고장이력 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("NGRGojangUpdate Error", ex.Message);
            }

            return Json(result);
        }
    }
}
