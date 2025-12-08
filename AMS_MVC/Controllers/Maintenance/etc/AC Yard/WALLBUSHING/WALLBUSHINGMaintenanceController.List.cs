
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class WALLBUSHINGMaintenanceController : Controller
    {
        // GET: WALLBUSHINGMaintenance
        public ActionResult WALLBUSHINGMaintenanceList(string WALLBUSHING_Code)
        {
            var basicInfo = wallbushingBasicInfoRepository.GetWALLBUSHINGBasicInfoByCode(WALLBUSHING_Code);
            ViewBag.WALLBUSHINGCode = WALLBUSHING_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Maintenance/etc/AC Yard/WALLBUSHING/WALLBUSHINGMaintenanceList.cshtml");
        }

        public ActionResult WALLBUSHINGMaintenanceTotalList()
        {
            return View("~/Views/Maintenance/Total/etc/AC Yard/WALLBUSHINGMaintenanceTotalList.cshtml");
        }

        /// <summary>
        /// WALLBUSHING 유지보수 데이터 가져오기
        /// </summary>
        [HttpPost]
        public JsonResult GetWALLBUSHINGMRByWALLBUSHINGCode(string wallbushingCode)
        {
            Result res = new Result(true);
            List<WALLBUSHINGMaintenanceHistory> wallbushingMR = new List<WALLBUSHINGMaintenanceHistory>();

            try
            {
                LogHelper.WriteLog("WALLBUSHINGMRController.cs", "GetWALLBUSHINGMRByWALLBUSHINGCode 실행");

                res = wallbushingMaintenanceRepository.GetWALLBUSHINGMRByWALLBUSHINGCode(wallbushingCode, out wallbushingMR);

                if (!res.IsSuccess)
                {
                    LogHelper.WriteLog("WALLBUSHINGMRController.cs", res.Message);
                    return Json(new { success = false, message = res.Message });
                }

                if (wallbushingMR.Count == 0)
                {
                    LogHelper.WriteLog("WALLBUSHINGMRController.cs", "조회된 데이터가 없습니다.");
                    return Json(new { success = true, data = new List<WALLBUSHINGMaintenanceHistory>() });
                }

                LogHelper.WriteLog("WALLBUSHINGMRController.cs", $"조회된 데이터: {wallbushingMR.Count}건");

                return Json(new { success = true, data = wallbushingMR });
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("WALLBUSHINGMRController.cs", "오류 발생: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalWALLBUSHINGMaintenanceListData()
        {
            try
            {
                LogHelper.WriteLog("TotalWALLBUSHINGMaintenanceController.List", "GetTotalWALLBUSHINGMaintenanceListData 실행");

                // 1) 전체 유지보수 이력 조회
                List<WALLBUSHINGMaintenanceHistory> wallbushingMaintenance;
                var repoResult = wallbushingMaintenanceRepository.GetTotalWALLBUSHINGMaintenance(out wallbushingMaintenance);
                if (!repoResult.IsSuccess)
                    return Json(new { success = false, message = repoResult.Message });

                // 2) 기본정보 전체 조회 → 코드별 매핑
                wallbushingBasicInfoRepository.GetAllWALLBUSHINGBasicInfoRepo(out var basics);
                var basicMap = basics.ToDictionary(b => b.WALLBUSHING_Code, b => b);

                // 3) JSON에 Name, Serial_No 포함
                var formattedData = wallbushingMaintenance.Select(item =>
                {
                    basicMap.TryGetValue(item.WALLBUSHING_Code, out var basic);
                    return new
                    {
                        item.Tbl_Idx,
                        item.WALLBUSHING_Code,
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

                LogHelper.WriteLog("WALLBUSHINGMaintenanceController.List", $"조회된 데이터: {formattedData.Count}건");
                return Json(formattedData);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("WALLBUSHINGMaintenanceController.List", $"GetTotalWALLBUSHINGMaintenanceListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}