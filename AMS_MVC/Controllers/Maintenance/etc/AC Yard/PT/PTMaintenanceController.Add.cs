using AMS_MVC.Models;
using AMS_MVC.Utlity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Maintenance.PT
{
    public partial class PTMaintenanceController : Controller
    {
        public ActionResult PTMaintenanceAdd(string PT_Code)
        {
            // 선택한 PT의 기본정보 조회
            var basicInfo = ptBasicInfoRepository.GetPTBasicInfoByCode(PT_Code);

            // 모든 PT 기본정보를 드롭다운에 바인딩
            List<PTBasicInfo> pts;
            var res = ptBasicInfoRepository.GetAllPTBasicInfoRepo(out pts);
            ViewBag.PTs = new SelectList(pts, "PT_Code", "SERIAL_NO");

            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.PT_Code = PT_Code;

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

            return View("~/Views/Maintenance/etc/AC Yard/PT/PTMaintenanceAdd.cshtml");
        }

        [HttpPost]
        public ActionResult PTMaintenanceAdd(PTMaintenanceHistory model)
        {
            Result result = new Result(true);
            try
            {
                // 현재 로그인한 사용자의 이름을 Writer에 저장
                model.MR_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";
                result = ptMaintenanceRepository.CreatePTMRRepo(model);

                if (!result.IsSuccess)
                {
                    result.Message = "PT 유지보수이력 정보를 추가하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("PTMaintenanceAdd Error", ex.Message);
            }

            return Json(result);
        }
    }
}
