
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class ITRMaintenanceController : Controller
    {
        [HttpGet]
        public ActionResult ITRMaintenanceUpdate(string itrCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(itrCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("ITRMaintenanceTotalList");
            }

            // 유지보수 이력 조회
            var result = itrMaintenanceRepository.GetITRMRDetailByITRCode(itrCode, tblIdx, out var itrMaintenanceList);

            if (!result.IsSuccess || itrMaintenanceList == null || !itrMaintenanceList.Any())
            {
                return HttpNotFound("ITR 유지보수이력 정보를 찾을 수 없습니다.");
            }

            var detailRecord = itrMaintenanceList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 유지보수 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Maintenance/ITR/ITRMaintenanceUpdate.cshtml", detailRecord);
        }

        [HttpPost]
        public ActionResult ITRMaintenanceUpdate(ITRMaintenanceHistory model)
        {
            Result result = new Result(true);
            try
            {
                model.MR_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";

                result = itrMaintenanceRepository.UpdateITRMRRepo(model);
                if (!result.IsSuccess)
                {
                    result.Message = "ITR 유지보수이력 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("ITRMaintenanceUpdate Error", ex.Message);
            }

            return Json(result);
        }
    }
}
