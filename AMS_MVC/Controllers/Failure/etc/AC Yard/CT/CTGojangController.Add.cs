using AMS_MVC.Models;
using AMS_MVC.Utlity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers
{
    public partial class CTGojangController : Controller
    {
        public ActionResult CTGojangAdd(string CT_Code)
        {
            // 선택한 CT의 기본정보 조회
            var basicInfo = ctBasicInfoRepository.GetCTBasicInfoByCode(CT_Code);

            // 모든 CT 기본정보를 드롭다운에 바인딩
            List<CTBasicInfo> cts;
            var res = ctBasicInfoRepository.GetAllCTBasicInfoRepo(out cts);
            ViewBag.CTs = new SelectList(cts, "CT_Code", "SERIAL_NO");

            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.CT_Code = CT_Code;

            // 업체 정보 로드 (제작사)
            var companies = new List<Company>();
            if (companyRepository.GetAllCompanies(out companies).IsSuccess && companies != null)
            {
                ViewBag.Companies = companies;
            }
            else
            {
                ViewBag.ErrorMessage = "제작사 정보를 불러올 수 없습니다.";
            }

            return View("~/Views/Gojang/etc/AC Yard/CT/CTGojangAdd.cshtml");
        }

        [HttpPost]
        public ActionResult CTGojangAdd(CTFailureHistory model)
        {
            Result result = new Result(true);
            try
            {
                // 현재 로그인한 사용자의 이름을 Writer에 저장
                model.Fail_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";
                result = ctGojangRepository.CreateCTFHRepo(model);

                if (!result.IsSuccess)
                {
                    result.Message = "CT 고장이력 정보를 추가하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("CTGojangAdd Error", ex.Message);
            }

            return Json(result);
        }
    }
}
