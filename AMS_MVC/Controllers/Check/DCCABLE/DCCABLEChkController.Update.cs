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
    public partial class DCCABLEChkController : Controller
    {
        public ActionResult DCCABLEChkUpdate(string dccableCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(dccableCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("DCCABLEChkTotalList");
            }

            var result = dccableChkRepository.GetDCCABLEChkDetailByDCCABLECode(dccableCode, tblIdx, out var dccableChkList);

            if (!result.IsSuccess || dccableChkList == null || !dccableChkList.Any())
            {
                return HttpNotFound("DCCABLE 보통점검 정보를 찾을 수 없습니다.");
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

            var detailRecord = dccableChkList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 보통점검 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Check/DCCABLE/DCCABLEChkUpdate.cshtml", detailRecord); ;
        }

        [HttpPost]
        public ActionResult DCCABLEChkUpdate(DCCABLEChk model)
        {
            Result result = new Result(true);
            try
            {
                if (!result.IsSuccess)
                {
                    result.Message = "DCCABLE 보통점검 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("DCCABLEChkUpdate Error", ex.Message);
            }

            var res = dccableChkRepository.UpdateDCCABLEChkInfoRepo(model);
            return Json(new { success = res.IsSuccess, message = result.Message });
        }
    }
}