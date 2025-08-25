
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{ 
    public partial class DCCABLEMaintenanceController : Controller
    {
        public ActionResult DCCABLEMaintenanceAdd(string DCCABLE_Code)
        {
            // 선택한 DCCABLE의 기본정보 조회
            var basicInfo = dccableBasicInfoRepository.GetDCCABLEBasicInfoByCode(DCCABLE_Code);

            // 모든 DCCABLE 기본정보를 드롭다운에 바인딩
            List<DCCABLEBasicInfo> dccables;
            var res = dccableBasicInfoRepository.GetAllDCCABLEBasicInfoRepo(out dccables);
            ViewBag.DCCABLEs = new SelectList(dccables, "DCCABLE_Code", "SERIAL_NO");

            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.DCCABLE_Code = DCCABLE_Code;

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

            return View("~/Views/Maintenance/DCCABLE/DCCABLEMaintenanceAdd.cshtml");
        }

        [HttpPost]
        public ActionResult DCCABLEMaintenanceAdd(DCCABLEMaintenanceHistory model)
        {
            Result result = new Result(true);
            try
            {
                // 현재 로그인한 사용자의 이름을 Writer에 저장
                model.MR_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";
                result = dccableMaintenanceRepository.CreateDCCABLEMRRepo(model);

                if (!result.IsSuccess)
                {
                    result.Message = "DCCABLE 유지보수이력 정보를 추가하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("DCCABLEMaintenanceAdd Error", ex.Message);
            }

            return Json(result);
        }
    }
}
