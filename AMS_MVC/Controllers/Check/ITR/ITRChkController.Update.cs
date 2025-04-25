// Controllers/Check/ITRChkController.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AMS_MVC.Models;
using AMS_MVC.Utlity;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Check
{
    public partial class ITRChkController : Controller
    {
        public ActionResult ITRChk1Update(string itrCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(itrCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("ITRChkTotalList");
            }

            var result = _chk1Repo.GetITRChk1DetailByITRCode(itrCode, tblIdx, out var itrChkList);

            if (!result.IsSuccess || itrChkList == null || !itrChkList.Any())
            {
                return HttpNotFound("ITR 보통점검 정보를 찾을 수 없습니다.");
            }

            var companies = new List<Company>();
            if (_companyRepo.GetAllCompanies(out companies).IsSuccess && companies != null)
            {
                ViewBag.Companies = companies;
            }
            else
            {
                ViewBag.ErrorMessage = "제작사 정보를 불러올 수 없습니다.";
            }

            var detailRecord = itrChkList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 보통점검 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Check/ITR/ITRChk1Update.cshtml", detailRecord); ;
        }

        [HttpPost]
        public ActionResult ITRChk1Update(ITRChk1 model)
        {
            Result result = new Result(true);
            try
            {
                if (!result.IsSuccess)
                {
                    result.Message = "ITR 보통점검 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("ITRChkUpdate Error", ex.Message);
            }

            var res = _chk1Repo.UpdateITRChk1InfoRepo(model);
            return Json(new { success = res.IsSuccess, message = result.Message });
        }

        public ActionResult ITRChk2Update(string itrCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(itrCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("ITRChk2TotalList");
            }

            var result = _chk2Repo.GetITRChk2DetailByITRCode(itrCode, tblIdx, out var itrChkList);

            if (!result.IsSuccess || itrChkList == null || !itrChkList.Any())
            {
                return HttpNotFound("ITR 정밀점검 정보를 찾을 수 없습니다.");
            }

            var companies = new List<Company>();
            if (_companyRepo.GetAllCompanies(out companies).IsSuccess && companies != null)
            {
                ViewBag.Companies = companies;
            }
            else
            {
                ViewBag.ErrorMessage = "제작사 정보를 불러올 수 없습니다.";
            }

            var detailRecord = itrChkList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 정밀점검 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Check/ITR/ITRChk2Update.cshtml", detailRecord); ;
        }

        [HttpPost]
        public ActionResult ITRChk2Update(ITRChk2 model)
        {
            Result result = new Result(true);
            try
            {
                if (!result.IsSuccess)
                {
                    result.Message = "ITR 정밀점검 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("ITRChk2Update Error", ex.Message);
            }

            var res = _chk2Repo.UpdateITRChk2InfoRepo(model);
            return Json(new { success = res.IsSuccess, message = result.Message });
        }
    }
}