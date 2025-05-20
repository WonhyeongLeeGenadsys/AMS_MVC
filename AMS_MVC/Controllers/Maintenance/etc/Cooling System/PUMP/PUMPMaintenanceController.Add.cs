using AMS_MVC.Models;
using AMS_MVC.Utlity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Maintenance.PUMP
{
    public partial class PUMPMaintenanceController : Controller
    {
        public ActionResult PUMPMaintenanceAdd(string PUMP_Code)
        {
            // 선택한 PUMP의 기본정보 조회
            var basicInfo = pumpBasicInfoRepository.GetPUMPBasicInfoByCode(PUMP_Code);

            // 모든 PUMP 기본정보를 드롭다운에 바인딩
            List<PUMPBasicInfo> pumps;
            var res = pumpBasicInfoRepository.GetAllPUMPBasicInfoRepo(out pumps);
            ViewBag.PUMPs = new SelectList(pumps, "PUMP_Code", "SERIAL_NO");

            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.PUMP_Code = PUMP_Code;

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

            return View("~/Views/Maintenance/etc/Cooling System/PUMP/PUMPMaintenanceAdd.cshtml");
        }

        [HttpPost]
        public ActionResult PUMPMaintenanceAdd(PUMPMaintenanceHistory model)
        {
            Result result = new Result(true);
            try
            {
                // 현재 로그인한 사용자의 이름을 Writer에 저장
                model.MR_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";
                result = pumpMaintenanceRepository.CreatePUMPMRRepo(model);

                if (!result.IsSuccess)
                {
                    result.Message = "PUMP 유지보수이력 정보를 추가하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("PUMPMaintenanceAdd Error", ex.Message);
            }

            return Json(result);
        }
    }
}
