
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class ESMaintenanceController : Controller
    {
        public ActionResult ESMaintenanceAdd(string ES_Code)
        {
            // 선택한 ES의 기본정보 조회
            var basicInfo = esBasicInfoRepository.GetESBasicInfoByCode(ES_Code);

            // 모든 ES 기본정보를 드롭다운에 바인딩
            List<ESBasicInfo> ess;
            var res = esBasicInfoRepository.GetAllESBasicInfoRepo(out ess);
            ViewBag.ESs = new SelectList(ess, "ES_Code", "SERIAL_NO");

            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.ES_Code = ES_Code;

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

            return View("~/Views/Maintenance/etc/Grounding/ES/ESMaintenanceAdd.cshtml");
        }

        [HttpPost]
        public ActionResult ESMaintenanceAdd(ESMaintenanceHistory model)
        {
            Result result = new Result(true);
            try
            {
                // 현재 로그인한 사용자의 이름을 Writer에 저장
                model.MR_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";
                result = esMaintenanceRepository.CreateESMRRepo(model);

                if (!result.IsSuccess)
                {
                    result.Message = "ES 유지보수이력 정보를 추가하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("ESMaintenanceAdd Error", ex.Message);
            }

            return Json(result);
        }
    }
}
