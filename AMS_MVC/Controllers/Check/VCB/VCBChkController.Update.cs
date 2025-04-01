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
    public partial class VCBChkController : Controller
    {
        public ActionResult VCBChkUpdate(string vcbCode)
        {
            var model = vcbChkRepository.GetVCBChkByCode(vcbCode);
            //ViewBag.SerialNo = model != null ? model.Serial_No : "";
            ViewBag.VCB_Code = model.VCB_Code;

            var companies = new List<Company>();
            if (companyRepository.GetAllCompanies(out companies).IsSuccess && companies != null)
            {
                ViewBag.Companies = companies;
            }
            else
            {
                ViewBag.ErrorMessage = "제작사 정보를 불러올 수 없습니다.";
            }
            return View("~/Views/Check/VCB/VCBChkUpdate.cshtml", model); ;
        }

        [HttpPost]
        public ActionResult VCBChkUpdate(VCBChk model)
        {
            Result result = new Result(true);
            try
            {
                if (!result.IsSuccess)
                {
                    result.Message = "VCB 보통점검 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("VCBChkUpdate Error", ex.Message);
            }

            var res = vcbChkRepository.UpdateVCBChkInfoRepo(model);
            return Json(new { success = res.IsSuccess, message = result.Message });
        }
    }
}