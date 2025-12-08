
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class NGRGojangController : Controller
    {
        public ActionResult NGRGojangAdd(string NGR_Code)
        {
            // 선택한 NGR의 기본정보 조회
            var basicInfo = ngrBasicInfoRepository.GetNGRBasicInfoByCode(NGR_Code);

            // 모든 NGR 기본정보를 드롭다운에 바인딩
            List<NGRBasicInfo> ngrs;
            var res = ngrBasicInfoRepository.GetAllNGRBasicInfoRepo(out ngrs);
            ViewBag.NGRs = new SelectList(ngrs, "NGR_Code", "SERIAL_NO");

            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.NGR_Code = NGR_Code;

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

            return View("~/Views/Gojang/etc/Grounding/NGR/NGRGojangAdd.cshtml");
        }

        [HttpPost]
        public ActionResult NGRGojangAdd(NGRFailureHistory model)
        {
            Result result = new Result(true);
            try
            {
                // 현재 로그인한 사용자의 이름을 Writer에 저장
                model.Fail_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";
                result = ngrGojangRepository.CreateNGRFHRepo(model);

                if (!result.IsSuccess)
                {
                    result.Message = "NGR 고장이력 정보를 추가하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("NGRGojangAdd Error", ex.Message);
            }

            return Json(result);
        }
    }
}
