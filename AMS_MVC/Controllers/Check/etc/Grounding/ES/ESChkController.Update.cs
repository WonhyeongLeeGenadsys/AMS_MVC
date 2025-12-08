
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class ESChkController : Controller
    {
        public ActionResult ESChkUpdate(string esCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(esCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("ESChkTotalList");
            }

            var result = esChkRepository.GetESChkDetailByESCode(esCode, tblIdx, out var esChkList);

            if (!result.IsSuccess || esChkList == null || !esChkList.Any())
            {
                return HttpNotFound("ES 보통점검 정보를 찾을 수 없습니다.");
            }

            var companies = new List<Company>();
            if (companyRepository.GetAllCompanies(out companies).IsSuccess && companies != null)
            {
                ViewBag.Companies = companies;
            }
            else
            {
                ViewBag.ErrorMessage = "제작사 정보를 불러올 수 없습니다.";
            }

            var detailRecord = esChkList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 보통점검 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Check/etc/Grounding/ES/ESChkUpdate.cshtml", detailRecord); ;
        }

        [HttpPost]
        public ActionResult ESChkUpdate(ESChk model)
        {
            Result result = new Result(true);
            try
            {
                if (!result.IsSuccess)
                {
                    result.Message = "ES 보통점검 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("ESChkUpdate Error", ex.Message);
            }

            var res = esChkRepository.UpdateESChkInfoRepo(model);
            return Json(new { success = res.IsSuccess, message = result.Message });
        }
    }
}