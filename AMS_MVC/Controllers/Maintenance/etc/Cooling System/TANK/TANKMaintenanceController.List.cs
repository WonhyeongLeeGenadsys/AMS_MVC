using AMS_MVC.Models;
using AMS_MVC.Repositories;
using AMS_MVC.Utlity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Maintenance.TANK
{
    public partial class TANKMaintenanceController : Controller
    {
        // GET: TANKMaintenance
        public ActionResult TANKMaintenanceList(string TANK_Code)
        {
            var basicInfo = tankBasicInfoRepository.GetTANKBasicInfoByCode(TANK_Code);
            ViewBag.TANKCode = TANK_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Maintenance/etc/Cooling System/TANK/TANKMaintenanceList.cshtml");
        }

        public ActionResult TANKMaintenanceTotalList()
        {
            return View("~/Views/Maintenance/Total/etc/Cooling System/TANKMaintenanceTotalList.cshtml");
        }

        /// <summary>
        /// TANK 유지보수 데이터 가져오기
        /// </summary>
        [HttpPost]
        public JsonResult GetTANKMRByTANKCode(string tankCode)
        {
            Result res = new Result(true);
            List<TANKMaintenanceHistory> tankMR = new List<TANKMaintenanceHistory>();

            try
            {
                LogHelper.WriteLog("TANKMRController.cs", "GetTANKMRByTANKCode 실행");

                res = tankMaintenanceRepository.GetTANKMRByTANKCode(tankCode, out tankMR);

                if (!res.IsSuccess)
                {
                    LogHelper.WriteLog("TANKMRController.cs", res.Message);
                    return Json(new { success = false, message = res.Message });
                }

                if (tankMR.Count == 0)
                {
                    LogHelper.WriteLog("TANKMRController.cs", "조회된 데이터가 없습니다.");
                    return Json(new { success = true, data = new List<TANKMaintenanceHistory>() });
                }

                LogHelper.WriteLog("TANKMRController.cs", $"조회된 데이터: {tankMR.Count}건");

                return Json(new { success = true, data = tankMR });
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("TANKMRController.cs", "오류 발생: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalTANKMaintenanceListData()
        {
            try
            {
                LogHelper.WriteLog("TotalTANKMaintenanceController.List", "GetTotalTANKMaintenanceListData 실행");

                // 1) 전체 유지보수 이력 조회
                List<TANKMaintenanceHistory> tankMaintenance;
                var repoResult = tankMaintenanceRepository.GetTotalTANKMaintenance(out tankMaintenance);
                if (!repoResult.IsSuccess)
                    return Json(new { success = false, message = repoResult.Message });

                // 2) 기본정보 전체 조회 → 코드별 매핑
                tankBasicInfoRepository.GetAllTANKBasicInfoRepo(out var basics);
                var basicMap = basics.ToDictionary(b => b.TANK_Code, b => b);

                // 3) JSON에 Name, Serial_No 포함
                var formattedData = tankMaintenance.Select(item =>
                {
                    basicMap.TryGetValue(item.TANK_Code, out var basic);
                    return new
                    {
                        item.Tbl_Idx,
                        item.TANK_Code,
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

                LogHelper.WriteLog("TANKMaintenanceController.List", $"조회된 데이터: {formattedData.Count}건");
                return Json(formattedData);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("TANKMaintenanceController.List", $"GetTotalTANKMaintenanceListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}