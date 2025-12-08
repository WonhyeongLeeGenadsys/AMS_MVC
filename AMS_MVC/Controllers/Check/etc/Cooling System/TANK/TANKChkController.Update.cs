
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class TANKChkController : Controller
    {
        public ActionResult TANKChkUpdate(string vcbCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(vcbCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("TANKChkTotalList");
            }

            var result = vcbChkRepository.GetTANKChkDetailByTANKCode(vcbCode, tblIdx, out var vcbChkList);

            if (!result.IsSuccess || vcbChkList == null || !vcbChkList.Any())
            {
                return HttpNotFound("TANK 보통점검 정보를 찾을 수 없습니다.");
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

            var detailRecord = vcbChkList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 보통점검 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Check/etc/Cooling System/TANK/TANKChkUpdate.cshtml", detailRecord); ;
        }

        [HttpPost]
        public ActionResult TANKChkUpdate(TANKChk model)
        {
            Result result = new Result(true);
            try
            {
                if (!result.IsSuccess)
                {
                    result.Message = "TANK 보통점검 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("TANKChkUpdate Error", ex.Message);
            }

            var res = vcbChkRepository.UpdateTANKChkInfoRepo(model);
            return Json(new { success = res.IsSuccess, message = result.Message });
        }
    }
}