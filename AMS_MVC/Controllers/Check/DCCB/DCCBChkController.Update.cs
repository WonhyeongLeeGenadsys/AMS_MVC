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
    public partial class DCCBChkController : Controller
    {
        public ActionResult DCCBChkUpdate(string dccbCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(dccbCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("DCCBChkTotalList");
            }

            var result = dccbChkRepository.GetDCCBChkDetailByDCCBCode(dccbCode, tblIdx, out var dccbChkList);

            if (!result.IsSuccess || dccbChkList == null || !dccbChkList.Any())
            {
                return HttpNotFound("DCCB 보통점검 정보를 찾을 수 없습니다.");
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

            var detailRecord = dccbChkList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 보통점검 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Check/DCCB/DCCBChkUpdate.cshtml", detailRecord); ;
        }

        [HttpPost]
        public ActionResult DCCBChkUpdate(DCCBChk model)
        {
            Result result = new Result(true);
            try
            {
                if (!result.IsSuccess)
                {
                    result.Message = "DCCB 보통점검 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("DCCBChkUpdate Error", ex.Message);
            }

            var res = dccbChkRepository.UpdateDCCBChkInfoRepo(model);
            return Json(new { success = res.IsSuccess, message = result.Message });
        }
    }
}