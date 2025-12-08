
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class SAMaintenanceController : Controller
    {
        public ActionResult SAMaintenanceAdd(string SA_Code)
        {
            // 선택한 SA의 기본정보 조회
            var basicInfo = saBasicInfoRepository.GetSABasicInfoByCode(SA_Code);

            // 모든 SA 기본정보를 드롭다운에 바인딩
            List<SABasicInfo> sas;
            var res = saBasicInfoRepository.GetAllSABasicInfoRepo(out sas);
            ViewBag.SAs = new SelectList(sas, "SA_Code", "SERIAL_NO");

            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.SA_Code = SA_Code;

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

            return View("~/Views/Maintenance/etc/DC Yard/SA/SAMaintenanceAdd.cshtml");
        }

        [HttpPost]
        public ActionResult SAMaintenanceAdd(SAMaintenanceHistory model)
        {
            Result result = new Result(true);
            try
            {
                // 현재 로그인한 사용자의 이름을 Writer에 저장
                model.MR_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";
                result = saMaintenanceRepository.CreateSAMRRepo(model);

                if (!result.IsSuccess)
                {
                    result.Message = "SA 유지보수이력 정보를 추가하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("SAMaintenanceAdd Error", ex.Message);
            }

            return Json(result);
        }
    }
}
