using AMS_MVC.Models;
using AMS_MVC.Repositories;
using AMS_MVC.Utlity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Maintenance.HEATEXCHANGER
{
    public partial class HEATEXCHANGERMaintenanceController : Controller
    {
        // GET: HEATEXCHANGERMaintenance
        public ActionResult HEATEXCHANGERMaintenanceList(string HEATEXCHANGER_Code)
        {
            var basicInfo = heatexchangerBasicInfoRepository.GetHEATEXCHANGERBasicInfoByCode(HEATEXCHANGER_Code);
            ViewBag.HEATEXCHANGERCode = HEATEXCHANGER_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Maintenance/etc/Cooling System/HEATEXCHANGER/HEATEXCHANGERMaintenanceList.cshtml");
        }

        public ActionResult HEATEXCHANGERMaintenanceTotalList()
        {
            return View("~/Views/Maintenance/Total/etc/Cooling System/HEATEXCHANGERMaintenanceTotalList.cshtml");
        }

        /// <summary>
        /// HEATEXCHANGER 유지보수 데이터 가져오기
        /// </summary>
        [HttpPost]
        public JsonResult GetHEATEXCHANGERMRByHEATEXCHANGERCode(string heatexchangerCode)
        {
            Result res = new Result(true);
            List<HEATEXCHANGERMaintenanceHistory> heatexchangerMR = new List<HEATEXCHANGERMaintenanceHistory>();

            try
            {
                LogHelper.WriteLog("HEATEXCHANGERMRController.cs", "GetHEATEXCHANGERMRByHEATEXCHANGERCode 실행");

                res = heatexchangerMaintenanceRepository.GetHEATEXCHANGERMRByHEATEXCHANGERCode(heatexchangerCode, out heatexchangerMR);

                if (!res.IsSuccess)
                {
                    LogHelper.WriteLog("HEATEXCHANGERMRController.cs", res.Message);
                    return Json(new { success = false, message = res.Message });
                }

                if (heatexchangerMR.Count == 0)
                {
                    LogHelper.WriteLog("HEATEXCHANGERMRController.cs", "조회된 데이터가 없습니다.");
                    return Json(new { success = true, data = new List<HEATEXCHANGERMaintenanceHistory>() });
                }

                LogHelper.WriteLog("HEATEXCHANGERMRController.cs", $"조회된 데이터: {heatexchangerMR.Count}건");

                return Json(new { success = true, data = heatexchangerMR });
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("HEATEXCHANGERMRController.cs", "오류 발생: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalHEATEXCHANGERMaintenanceListData()
        {
            try
            {
                LogHelper.WriteLog("TotalHEATEXCHANGERMaintenanceController.List", "GetTotalHEATEXCHANGERMaintenanceListData 실행");

                // 1) 전체 유지보수 이력 조회
                List<HEATEXCHANGERMaintenanceHistory> heatexchangerMaintenance;
                var repoResult = heatexchangerMaintenanceRepository.GetTotalHEATEXCHANGERMaintenance(out heatexchangerMaintenance);
                if (!repoResult.IsSuccess)
                    return Json(new { success = false, message = repoResult.Message });

                // 2) 기본정보 전체 조회 → 코드별 매핑
                heatexchangerBasicInfoRepository.GetAllHEATEXCHANGERBasicInfoRepo(out var basics);
                var basicMap = basics.ToDictionary(b => b.HEATEXCHANGER_Code, b => b);

                // 3) JSON에 Name, Serial_No 포함
                var formattedData = heatexchangerMaintenance.Select(item =>
                {
                    basicMap.TryGetValue(item.HEATEXCHANGER_Code, out var basic);
                    return new
                    {
                        item.Tbl_Idx,
                        item.HEATEXCHANGER_Code,
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

                LogHelper.WriteLog("HEATEXCHANGERMaintenanceController.List", $"조회된 데이터: {formattedData.Count}건");
                return Json(formattedData);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("HEATEXCHANGERMaintenanceController.List", $"GetTotalHEATEXCHANGERMaintenanceListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}