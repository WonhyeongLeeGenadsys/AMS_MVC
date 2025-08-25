
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class PUMPMaintenanceController : Controller
    {
        // GET: PUMPMaintenance
        public ActionResult PUMPMaintenanceList(string PUMP_Code)
        {
            var basicInfo = pumpBasicInfoRepository.GetPUMPBasicInfoByCode(PUMP_Code);
            ViewBag.PUMPCode = PUMP_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Maintenance/etc/Cooling System/PUMP/PUMPMaintenanceList.cshtml");
        }

        public ActionResult PUMPMaintenanceTotalList()
        {
            return View("~/Views/Maintenance/Total/etc/Cooling System/PUMPMaintenanceTotalList.cshtml");
        }

        /// <summary>
        /// PUMP 유지보수 데이터 가져오기
        /// </summary>
        [HttpPost]
        public JsonResult GetPUMPMRByPUMPCode(string pumpCode)
        {
            Result res = new Result(true);
            List<PUMPMaintenanceHistory> pumpMR = new List<PUMPMaintenanceHistory>();

            try
            {
                LogHelper.WriteLog("PUMPMRController.cs", "GetPUMPMRByPUMPCode 실행");

                res = pumpMaintenanceRepository.GetPUMPMRByPUMPCode(pumpCode, out pumpMR);

                if (!res.IsSuccess)
                {
                    LogHelper.WriteLog("PUMPMRController.cs", res.Message);
                    return Json(new { success = false, message = res.Message });
                }

                if (pumpMR.Count == 0)
                {
                    LogHelper.WriteLog("PUMPMRController.cs", "조회된 데이터가 없습니다.");
                    return Json(new { success = true, data = new List<PUMPMaintenanceHistory>() });
                }

                LogHelper.WriteLog("PUMPMRController.cs", $"조회된 데이터: {pumpMR.Count}건");

                return Json(new { success = true, data = pumpMR });
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("PUMPMRController.cs", "오류 발생: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalPUMPMaintenanceListData()
        {
            try
            {
                LogHelper.WriteLog("TotalPUMPMaintenanceController.List", "GetTotalPUMPMaintenanceListData 실행");

                // 1) 전체 유지보수 이력 조회
                List<PUMPMaintenanceHistory> pumpMaintenance;
                var repoResult = pumpMaintenanceRepository.GetTotalPUMPMaintenance(out pumpMaintenance);
                if (!repoResult.IsSuccess)
                    return Json(new { success = false, message = repoResult.Message });

                // 2) 기본정보 전체 조회 → 코드별 매핑
                pumpBasicInfoRepository.GetAllPUMPBasicInfoRepo(out var basics);
                var basicMap = basics.ToDictionary(b => b.PUMP_Code, b => b);

                // 3) JSON에 Name, Serial_No 포함
                var formattedData = pumpMaintenance.Select(item =>
                {
                    basicMap.TryGetValue(item.PUMP_Code, out var basic);
                    return new
                    {
                        item.Tbl_Idx,
                        item.PUMP_Code,
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

                LogHelper.WriteLog("PUMPMaintenanceController.List", $"조회된 데이터: {formattedData.Count}건");
                return Json(formattedData);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("PUMPMaintenanceController.List", $"GetTotalPUMPMaintenanceListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}