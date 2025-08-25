
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class ITRGojangController : Controller
    {
        public ActionResult ITRGojangAdd(string ITR_Code)
        {
            // 선택한 ITR의 기본정보 조회
            var basicInfo = itrBasicInfoRepository.GetITRBasicInfoByITRCode(ITR_Code);

            // 모든 ITR 기본정보를 드롭다운에 바인딩
            List<ITRBasicInfo> itrs;
            var res = itrBasicInfoRepository.GetAllITRBasicInfoRepo(out itrs);
            ViewBag.ITRs = new SelectList(itrs, "ITR_Code", "SERIAL_NO");

            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.ITR_Code = ITR_Code;

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

            return View("~/Views/Gojang/ITR/ITRGojangAdd.cshtml");
        }

        [HttpPost]
        public ActionResult ITRGojangAdd(ITRFailureHistory model)
        {
            Result result = new Result(true);
            try
            {
                // 현재 로그인한 사용자의 이름을 Writer에 저장
                model.Fail_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";
                result = itrGojangRepository.CreateITRFHRepo(model);

                if (!result.IsSuccess)
                {
                    result.Message = "ITR 고장이력 정보를 추가하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("ITRGojangAdd Error", ex.Message);
            }

            return Json(result);
        }
    }
}
