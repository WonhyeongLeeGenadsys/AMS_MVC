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
    public partial class NGRChkController : Controller
    {
        public ActionResult NGRChkUpdate(string ngrCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(ngrCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("NGRChkTotalList");
            }

            var result = ngrChkRepository.GetNGRChkDetailByNGRCode(ngrCode, tblIdx, out var ngrChkList);

            if (!result.IsSuccess || ngrChkList == null || !ngrChkList.Any())
            {
                return HttpNotFound("NGR 보통점검 정보를 찾을 수 없습니다.");
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

            var detailRecord = ngrChkList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 보통점검 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Check/etc/Grounding/NGR/NGRChkUpdate.cshtml", detailRecord); ;
        }

        [HttpPost]
        public ActionResult NGRChkUpdate(NGRChk model)
        {
            Result result = new Result(true);
            try
            {
                if (!result.IsSuccess)
                {
                    result.Message = "NGR 보통점검 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("NGRChkUpdate Error", ex.Message);
            }

            var res = ngrChkRepository.UpdateNGRChkInfoRepo(model);
            return Json(new { success = res.IsSuccess, message = result.Message });
        }
    }
}