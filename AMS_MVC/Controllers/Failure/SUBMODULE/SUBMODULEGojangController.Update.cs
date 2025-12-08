
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
        [HttpGet]
        public ActionResult SUBMODULEGojangUpdate(string submoduleCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(submoduleCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("SUBMODULEGojangTotalList");
            }

            // 고장 이력 조회
            var result = submoduleGojangRepository.GetSUBMODULEFHDetailBySUBMODULECode(submoduleCode, tblIdx, out var submoduleGojangList);

            if (!result.IsSuccess || submoduleGojangList == null || !submoduleGojangList.Any())
            {
                return HttpNotFound("SUBMODULE 고장이력 정보를 찾을 수 없습니다.");
            }

            var detailRecord = submoduleGojangList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 고장 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Gojang/SUBMODULE/SUBMODULEGojangUpdate.cshtml", detailRecord);
        }

        [HttpPost]
        public ActionResult SUBMODULEGojangUpdate(SUBMODULEFailureHistory model)
        {
            Result result = new Result(true);
            try
            {
                model.Fail_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";

                result = submoduleGojangRepository.UpdateSUBMODULEFHRepo(model);
                if (!result.IsSuccess)
                {
                    result.Message = "SUBMODULE 고장이력 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("SUBMODULEGojangUpdate Error", ex.Message);
            }

            return Json(result);
        }
    }
}
