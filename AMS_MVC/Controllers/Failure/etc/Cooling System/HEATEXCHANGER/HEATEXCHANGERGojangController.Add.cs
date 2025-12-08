
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class HEATEXCHANGERGojangController : Controller
    {
        public ActionResult HEATEXCHANGERGojangAdd(string HEATEXCHANGER_Code)
        {
            // 선택한 HEATEXCHANGER의 기본정보 조회
            var basicInfo = heatexchangerBasicInfoRepository.GetHEATEXCHANGERBasicInfoByCode(HEATEXCHANGER_Code);

            // 모든 HEATEXCHANGER 기본정보를 드롭다운에 바인딩
            List<HEATEXCHANGERBasicInfo> heatexchangers;
            var res = heatexchangerBasicInfoRepository.GetAllHEATEXCHANGERBasicInfoRepo(out heatexchangers);
            ViewBag.HEATEXCHANGERs = new SelectList(heatexchangers, "HEATEXCHANGER_Code", "SERIAL_NO");

            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.HEATEXCHANGER_Code = HEATEXCHANGER_Code;

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

            return View("~/Views/Gojang/etc/Cooling System/HEATEXCHANGER/HEATEXCHANGERGojangAdd.cshtml");
        }

        [HttpPost]
        public ActionResult HEATEXCHANGERGojangAdd(HEATEXCHANGERFailureHistory model)
        {
            Result result = new Result(true);
            try
            {
                // 현재 로그인한 사용자의 이름을 Writer에 저장
                model.Fail_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";
                result = heatexchangerGojangRepository.CreateHEATEXCHANGERFHRepo(model);

                if (!result.IsSuccess)
                {
                    result.Message = "HEATEXCHANGER 고장이력 정보를 추가하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("HEATEXCHANGERGojangAdd Error", ex.Message);
            }

            return Json(result);
        }
    }
}
