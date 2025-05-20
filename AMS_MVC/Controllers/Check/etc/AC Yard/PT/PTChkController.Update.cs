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
    public partial class PTChkController : Controller
    {
        public ActionResult PTChkUpdate(string ptCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(ptCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("PTChkTotalList");
            }

            var result = ptChkRepository.GetPTChkDetailByPTCode(ptCode, tblIdx, out var ptChkList);

            if (!result.IsSuccess || ptChkList == null || !ptChkList.Any())
            {
                return HttpNotFound("PT 보통점검 정보를 찾을 수 없습니다.");
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

            var detailRecord = ptChkList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 보통점검 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Check/etc/AC Yard/PT/PTChkUpdate.cshtml", detailRecord); ;
        }

        [HttpPost]
        public ActionResult PTChkUpdate(PTChk model)
        {
            Result result = new Result(true);
            try
            {
                if (!result.IsSuccess)
                {
                    result.Message = "PT 보통점검 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("PTChkUpdate Error", ex.Message);
            }

            var res = ptChkRepository.UpdatePTChkInfoRepo(model);
            return Json(new { success = res.IsSuccess, message = result.Message });
        }
    }
}