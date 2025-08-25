
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class VCBGojangController : Controller
    {
        [HttpGet]
        public ActionResult VCBGojangUpdate(string vcbCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(vcbCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("VCBGojangTotalList");
            }

            // 고장 이력 조회
            var result = vcbGojangRepository.GetVCBFHDetailByVCBCode(vcbCode, tblIdx, out var vcbGojangList);

            if (!result.IsSuccess || vcbGojangList == null || !vcbGojangList.Any())
            {
                return HttpNotFound("VCB 고장이력 정보를 찾을 수 없습니다.");
            }

            var detailRecord = vcbGojangList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 고장 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Gojang/VCB/VCBGojangUpdate.cshtml", detailRecord);
        }

        [HttpPost]
        public ActionResult VCBGojangUpdate(VCBFailureHistory model)
        {
            Result result = new Result(true);
            try
            {
                model.Fail_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";

                result = vcbGojangRepository.UpdateVCBFHRepo(model);
                if (!result.IsSuccess)
                {
                    result.Message = "VCB 고장이력 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("VCBGojangUpdate Error", ex.Message);
            }

            return Json(result);
        }
    }
}
