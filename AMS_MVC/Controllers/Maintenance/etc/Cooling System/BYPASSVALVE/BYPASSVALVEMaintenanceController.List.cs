
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class BYPASSVALVEMaintenanceController : Controller
    {
        // GET: BYPASSVALVEMaintenance
        public ActionResult BYPASSVALVEMaintenanceList(string BYPASSVALVE_Code)
        {
            var basicInfo = bypassvalveBasicInfoRepository.GetBYPASSVALVEBasicInfoByCode(BYPASSVALVE_Code);
            ViewBag.BYPASSVALVECode = BYPASSVALVE_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Maintenance/etc/Cooling System/BYPASSVALVE/BYPASSVALVEMaintenanceList.cshtml");
        }

        public ActionResult BYPASSVALVEMaintenanceTotalList()
        {
            return View("~/Views/Maintenance/Total/etc/Cooling System/BYPASSVALVEMaintenanceTotalList.cshtml");
        }

        /// <summary>
        /// BYPASSVALVE 유지보수 데이터 가져오기
        /// </summary>
        [HttpPost]
        public JsonResult GetBYPASSVALVEMRByBYPASSVALVECode(string bypassvalveCode)
        {
            Result res = new Result(true);
            List<BYPASSVALVEMaintenanceHistory> bypassvalveMR = new List<BYPASSVALVEMaintenanceHistory>();

            try
            {
                LogHelper.WriteLog("BYPASSVALVEMRController.cs", "GetBYPASSVALVEMRByBYPASSVALVECode 실행");

                res = bypassvalveMaintenanceRepository.GetBYPASSVALVEMRByBYPASSVALVECode(bypassvalveCode, out bypassvalveMR);

                if (!res.IsSuccess)
                {
                    LogHelper.WriteLog("BYPASSVALVEMRController.cs", res.Message);
                    return Json(new { success = false, message = res.Message });
                }

                if (bypassvalveMR.Count == 0)
                {
                    LogHelper.WriteLog("BYPASSVALVEMRController.cs", "조회된 데이터가 없습니다.");
                    return Json(new { success = true, data = new List<BYPASSVALVEMaintenanceHistory>() });
                }

                LogHelper.WriteLog("BYPASSVALVEMRController.cs", $"조회된 데이터: {bypassvalveMR.Count}건");

                return Json(new { success = true, data = bypassvalveMR });
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("BYPASSVALVEMRController.cs", "오류 발생: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalBYPASSVALVEMaintenanceListData()
        {
            try
            {
                LogHelper.WriteLog("TotalBYPASSVALVEMaintenanceController.List", "GetTotalBYPASSVALVEMaintenanceListData 실행");

                // 1) 전체 유지보수 이력 조회
                List<BYPASSVALVEMaintenanceHistory> bypassvalveMaintenance;
                var repoResult = bypassvalveMaintenanceRepository.GetTotalBYPASSVALVEMaintenance(out bypassvalveMaintenance);
                if (!repoResult.IsSuccess)
                    return Json(new { success = false, message = repoResult.Message });

                // 2) 기본정보 전체 조회 → 코드별 매핑
                bypassvalveBasicInfoRepository.GetAllBYPASSVALVEBasicInfoRepo(out var basics);
                var basicMap = basics.ToDictionary(b => b.BYPASSVALVE_Code, b => b);

                // 3) JSON에 Name, Serial_No 포함
                var formattedData = bypassvalveMaintenance.Select(item =>
                {
                    basicMap.TryGetValue(item.BYPASSVALVE_Code, out var basic);
                    return new
                    {
                        item.Tbl_Idx,
                        item.BYPASSVALVE_Code,
                        Name = basic?.Name ?? "",
                        Serial_No = basic?.Serial_No ?? "",
                        item.MR_Bosu_Name,
                        item.MR_Weather,
                        item.MR_Temp,
                        item.MR_Hum,
                        item.MR_Content,
                        item.MR_Status,
                        item.MR_Part,
                        item.MR_Worker,
                        MR_Date = item.MR_Date?.ToString("yy.MM.dd"),
                        item.MR_Writer
                    };
                }).ToList();

                LogHelper.WriteLog("BYPASSVALVEMaintenanceController.List", $"조회된 데이터: {formattedData.Count}건");
                return Json(formattedData);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("BYPASSVALVEMaintenanceController.List", $"GetTotalBYPASSVALVEMaintenanceListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}