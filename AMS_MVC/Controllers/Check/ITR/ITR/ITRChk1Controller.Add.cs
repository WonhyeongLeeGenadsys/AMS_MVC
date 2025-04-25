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
    public partial class ITRChk1Controller : Controller
    {
        public ActionResult ITRChk1Add(string ITR_Code)
        {
            var basicInfo = itrBasicInfoRepository.GetITRBasicInfoByITRCode(ITR_Code);
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.ITR_Code = ITR_Code;

            var companies = new List<Company>();
            if (companyRepository.GetAllCompanies(out companies).IsSuccess && companies != null)
            {
                ViewBag.Companies = companies;
            }
            else
            {
                ViewBag.ErrorMessage = "제작사 정보를 불러올 수 없습니다.";
            }
            return View("~/Views/Check/ITR/ITRChk1Add.cshtml");
        }

        [HttpPost]
        public ActionResult ITRChk1Add(ITRChk1 model)
        {
            Result result = new Result(true);
            try
            {
                model.CHK1_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";
                result = itrChk1Repository.CreateITRChk1InfoRepo(model);

                if (!result.IsSuccess)
                {
                    result.Message = "ITR 보통점검 정보를 추가하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("ITRChk1Add Error", ex.Message);
            }

            return Json(result);
        }
    }
}