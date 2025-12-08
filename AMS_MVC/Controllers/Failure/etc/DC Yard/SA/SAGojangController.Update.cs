
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class SAGojangController : Controller
    {
        [HttpGet]
        public ActionResult SAGojangUpdate(string saCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(saCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("SAGojangTotalList");
            }

            // 고장 이력 조회
            var result = saGojangRepository.GetSAFHDetailBySACode(saCode, tblIdx, out var saGojangList);

            if (!result.IsSuccess || saGojangList == null || !saGojangList.Any())
            {
                return HttpNotFound("SA 고장이력 정보를 찾을 수 없습니다.");
            }

            var detailRecord = saGojangList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 고장 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Gojang/etc/DC Yard/SA/SAGojangUpdate.cshtml", detailRecord);
        }

        [HttpPost]
        public ActionResult SAGojangUpdate(SAFailureHistory model)
        {
            Result result = new Result(true);
            try
            {
                model.Fail_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";

                result = saGojangRepository.UpdateSAFHRepo(model);
                if (!result.IsSuccess)
                {
                    result.Message = "SA 고장이력 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("SAGojangUpdate Error", ex.Message);
            }

            return Json(result);
        }
    }
}
