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
    public partial class SUBMODULEChkController : Controller
    {
        public ActionResult SUBMODULEChkUpdate(string submoduleCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(submoduleCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("SUBMODULEChkTotalList");
            }

            var result = submoduleChkRepository.GetSUBMODULEChkDetailBySUBMODULECode(submoduleCode, tblIdx, out var submoduleChkList);

            if (!result.IsSuccess || submoduleChkList == null || !submoduleChkList.Any())
            {
                return HttpNotFound("SUBMODULE 보통점검 정보를 찾을 수 없습니다.");
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

            var detailRecord = submoduleChkList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 보통점검 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Check/SUBMODULE/SUBMODULEChkUpdate.cshtml", detailRecord); ;
        }

        [HttpPost]
        public ActionResult SUBMODULEChkUpdate(SUBMODULEChk model)
        {
            Result result = new Result(true);
            try
            {
                if (!result.IsSuccess)
                {
                    result.Message = "SUBMODULE 보통점검 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("SUBMODULEChkUpdate Error", ex.Message);
            }

            var res = submoduleChkRepository.UpdateSUBMODULEChkInfoRepo(model);
            return Json(new { success = res.IsSuccess, message = result.Message });
        }
    }
}