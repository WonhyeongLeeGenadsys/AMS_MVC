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
    public partial class LAGojangController : Controller
    {
        public ActionResult LAGojangAdd(string LA_Code)
        {
            // 선택한 LA의 기본정보 조회
            var basicInfo = laBasicInfoRepository.GetLABasicInfoByCode(LA_Code);

            // 모든 LA 기본정보를 드롭다운에 바인딩
            List<LABasicInfo> las;
            var res = laBasicInfoRepository.GetAllLABasicInfoRepo(out las);
            ViewBag.LAs = new SelectList(las, "LA_Code", "SERIAL_NO");

            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.LA_Code = LA_Code;

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

            return View("~/Views/Gojang/etc/AC Yard/LA/LAGojangAdd.cshtml");
        }

        [HttpPost]
        public ActionResult LAGojangAdd(LAFailureHistory model)
        {
            Result result = new Result(true);
            try
            {
                // 현재 로그인한 사용자의 이름을 Writer에 저장
                model.Fail_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";
                result = laGojangRepository.CreateLAFHRepo(model);

                if (!result.IsSuccess)
                {
                    result.Message = "LA 고장이력 정보를 추가하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("LAGojangAdd Error", ex.Message);
            }

            return Json(result);
        }
    }
}
