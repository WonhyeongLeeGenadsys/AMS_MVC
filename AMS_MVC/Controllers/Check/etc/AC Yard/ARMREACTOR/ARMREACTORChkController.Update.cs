
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class ARMREACTORChkController : Controller
    {
        public ActionResult ARMREACTORChkUpdate(string armreactorCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(armreactorCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("ARMREACTORChkTotalList");
            }

            var result = armreactorChkRepository.GetARMREACTORChkDetailByARMREACTORCode(armreactorCode, tblIdx, out var armreactorChkList);

            if (!result.IsSuccess || armreactorChkList == null || !armreactorChkList.Any())
            {
                return HttpNotFound("ARMREACTOR 보통점검 정보를 찾을 수 없습니다.");
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

            var detailRecord = armreactorChkList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 보통점검 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Check/etc/AC Yard/ARMREACTOR/ARMREACTORChkUpdate.cshtml", detailRecord); ;
        }

        [HttpPost]
        public ActionResult ARMREACTORChkUpdate(ARMREACTORChk model)
        {
            Result result = new Result(true);
            try
            {
                if (!result.IsSuccess)
                {
                    result.Message = "ARMREACTOR 보통점검 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("ARMREACTORChkUpdate Error", ex.Message);
            }

            var res = armreactorChkRepository.UpdateARMREACTORChkInfoRepo(model);
            return Json(new { success = res.IsSuccess, message = result.Message });
        }
    }
}