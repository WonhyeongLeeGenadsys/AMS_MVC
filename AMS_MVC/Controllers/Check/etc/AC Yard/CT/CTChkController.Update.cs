using AMS_MVC.Models;
using AMS_MVC.Utlity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Check
{
    public partial class CTChkController : Controller
    {
        public ActionResult CTChkUpdate(string ctCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(ctCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("CTChkTotalList");
            }

            var result = ctChkRepository.GetCTChkDetailByCTCode(ctCode, tblIdx, out var ctChkList);

            if (!result.IsSuccess || ctChkList == null || !ctChkList.Any())
            {
                return HttpNotFound("CT 보통점검 정보를 찾을 수 없습니다.");
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

            var detailRecord = ctChkList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 보통점검 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Check/etc/AC Yard/CT/CTChkUpdate.cshtml", detailRecord); ;
        }

        [HttpPost]
        public ActionResult CTChkUpdate(CTChk model)
        {
            Result result = new Result(true);
            try
            {
                if (!result.IsSuccess)
                {
                    result.Message = "CT 보통점검 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("CTChkUpdate Error", ex.Message);
            }

            var res = ctChkRepository.UpdateCTChkInfoRepo(model);
            return Json(new { success = res.IsSuccess, message = result.Message });
        }
    }
}