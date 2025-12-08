
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class DCCABLEGojangController : Controller
    {
        [HttpGet]
        public ActionResult DCCABLEGojangUpdate(string dccableCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(dccableCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("DCCABLEGojangTotalList");
            }

            // 고장 이력 조회
            var result = dccableGojangRepository.GetDCCABLEFHDetailByDCCABLECode(dccableCode, tblIdx, out var dccableGojangList);

            if (!result.IsSuccess || dccableGojangList == null || !dccableGojangList.Any())
            {
                return HttpNotFound("DCCABLE 고장이력 정보를 찾을 수 없습니다.");
            }

            var detailRecord = dccableGojangList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 고장 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Gojang/DCCABLE/DCCABLEGojangUpdate.cshtml", detailRecord);
        }

        [HttpPost]
        public ActionResult DCCABLEGojangUpdate(DCCABLEFailureHistory model)
        {
            Result result = new Result(true);
            try
            {
                model.Fail_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";

                result = dccableGojangRepository.UpdateDCCABLEFHRepo(model);
                if (!result.IsSuccess)
                {
                    result.Message = "DCCABLE 고장이력 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("DCCABLEGojangUpdate Error", ex.Message);
            }

            return Json(result);
        }
    }
}
