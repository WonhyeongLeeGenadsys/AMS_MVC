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
        public ActionResult VCBChkAdd(string VCB_Code)
        {
            // 선택한 VCB의 기본정보 조회
            var basicInfo = vcbBasicInfoRepository.GetVCBBasicInfoByCode(VCB_Code);

            // 모든 VCB 기본정보를 드롭다운에 바인딩
            List<VCBBasicInfo> vcbs;
            var res = vcbBasicInfoRepository.GetAllVCBBasicInfoRepo(out vcbs);
            ViewBag.VCBs = new SelectList(vcbs, "VCB_Code", "SERIAL_NO");

            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.VCB_Code = VCB_Code;

            var companies = new List<Company>();
            if(companyRepository.GetAllCompanies(out companies).IsSuccess && companies != null)
            {
                ViewBag.Companies = companies;
            }
            else
            {
                ViewBag.ErrorMessage = "제작사 정보를 불러올 수 없습니다.";
            }
            return View("~/Views/Check/VCB/VCBChkAdd.cshtml");
        }

        [HttpPost]
        public ActionResult VCBChkAdd(VCBChk model)
        {
            Result result = new Result(true);
            try
            {
                model.CHK_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";
                result = vcbChkRepository.CreateVCBChkInfoRepo(model);

                if (!result.IsSuccess)
                {
                    result.Message = "VCB 보통점검 정보를 추가하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("VCBChkAdd Error", ex.Message);
            }
           
            return Json(result);
        }
    }
}