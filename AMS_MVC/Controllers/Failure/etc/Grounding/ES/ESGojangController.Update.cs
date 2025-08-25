
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class ESGojangController : Controller
    {
        [HttpGet]
        public ActionResult ESGojangUpdate(string esCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(esCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("ESGojangTotalList");
            }

            // 고장 이력 조회
            var result = esGojangRepository.GetESFHDetailByESCode(esCode, tblIdx, out var esGojangList);

            if (!result.IsSuccess || esGojangList == null || !esGojangList.Any())
            {
                return HttpNotFound("ES 고장이력 정보를 찾을 수 없습니다.");
            }

            var detailRecord = esGojangList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 고장 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Gojang/etc/Grounding/ES/ESGojangUpdate.cshtml", detailRecord);
        }

        [HttpPost]
        public ActionResult ESGojangUpdate(ESFailureHistory model)
        {
            Result result = new Result(true);
            try
            {
                model.Fail_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";

                result = esGojangRepository.UpdateESFHRepo(model);
                if (!result.IsSuccess)
                {
                    result.Message = "ES 고장이력 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("ESGojangUpdate Error", ex.Message);
            }

            return Json(result);
        }
    }
}
