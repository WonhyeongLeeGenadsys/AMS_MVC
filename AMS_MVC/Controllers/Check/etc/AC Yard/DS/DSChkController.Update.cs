
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class DSChkController : Controller
    {
        public ActionResult DSChkUpdate(string dsCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(dsCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("DSChkTotalList");
            }

            var result = dsChkRepository.GetDSChkDetailByDSCode(dsCode, tblIdx, out var dsChkList);

            if (!result.IsSuccess || dsChkList == null || !dsChkList.Any())
            {
                return HttpNotFound("DS 보통점검 정보를 찾을 수 없습니다.");
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

            var detailRecord = dsChkList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 보통점검 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Check/etc/AC Yard/DS/DSChkUpdate.cshtml", detailRecord); ;
        }

        [HttpPost]
        public ActionResult DSChkUpdate(DSChk model)
        {
            Result result = new Result(true);
            try
            {
                if (!result.IsSuccess)
                {
                    result.Message = "DS 보통점검 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("DSChkUpdate Error", ex.Message);
            }

            var res = dsChkRepository.UpdateDSChkInfoRepo(model);
            return Json(new { success = res.IsSuccess, message = result.Message });
        }
    }
}