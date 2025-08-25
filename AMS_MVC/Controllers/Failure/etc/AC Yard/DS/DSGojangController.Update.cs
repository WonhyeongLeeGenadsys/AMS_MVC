
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class DSGojangController : Controller
    {
        [HttpGet]
        public ActionResult DSGojangUpdate(string dsCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(dsCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("DSGojangTotalList");
            }

            // 고장 이력 조회
            var result = dsGojangRepository.GetDSFHDetailByDSCode(dsCode, tblIdx, out var dsGojangList);

            if (!result.IsSuccess || dsGojangList == null || !dsGojangList.Any())
            {
                return HttpNotFound("DS 고장이력 정보를 찾을 수 없습니다.");
            }

            var detailRecord = dsGojangList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 고장 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Gojang/etc/AC Yard/DS/DSGojangUpdate.cshtml", detailRecord);
        }

        [HttpPost]
        public ActionResult DSGojangUpdate(DSFailureHistory model)
        {
            Result result = new Result(true);
            try
            {
                model.Fail_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";

                result = dsGojangRepository.UpdateDSFHRepo(model);
                if (!result.IsSuccess)
                {
                    result.Message = "DS 고장이력 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("DSGojangUpdate Error", ex.Message);
            }

            return Json(result);
        }
    }
}
