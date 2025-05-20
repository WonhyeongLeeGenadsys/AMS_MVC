using AMS_MVC.Models;
using AMS_MVC.Repositories;
using AMS_MVC.Utlity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Maintenance.CT
{
    public partial class CTMaintenanceController : Controller
    {
        // GET: CTMaintenance
        public ActionResult CTMaintenanceList(string CT_Code)
        {
            var basicInfo = ctBasicInfoRepository.GetCTBasicInfoByCode(CT_Code);
            ViewBag.CTCode = CT_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Maintenance/etc/AC Yard/CT/CTMaintenanceList.cshtml");
        }

        public ActionResult CTMaintenanceTotalList()
        {
            return View("~/Views/Maintenance/Total/etc/AC Yard/CTMaintenanceTotalList.cshtml");
        }

        /// <summary>
        /// CT 유지보수 데이터 가져오기
        /// </summary>
        [HttpPost]
        public JsonResult GetCTMRByCTCode(string ctCode)
        {
            Result res = new Result(true);
            List<CTMaintenanceHistory> ctMR = new List<CTMaintenanceHistory>();

            try
            {
                LogHelper.WriteLog("CTMRController.cs", "GetCTMRByCTCode 실행");

                res = ctMaintenanceRepository.GetCTMRByCTCode(ctCode, out ctMR);

                if (!res.IsSuccess)
                {
                    LogHelper.WriteLog("CTMRController.cs", res.Message);
                    return Json(new { success = false, message = res.Message });
                }

                if (ctMR.Count == 0)
                {
                    LogHelper.WriteLog("CTMRController.cs", "조회된 데이터가 없습니다.");
                    return Json(new { success = true, data = new List<CTMaintenanceHistory>() });
                }

                LogHelper.WriteLog("CTMRController.cs", $"조회된 데이터: {ctMR.Count}건");

                return Json(new { success = true, data = ctMR });
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("CTMRController.cs", "오류 발생: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalCTMaintenanceListData()
        {
            try
            {
                LogHelper.WriteLog("TotalCTMaintenanceController.List", "GetTotalCTMaintenanceListData 실행");

                // 1) 전체 유지보수 이력 조회
                List<CTMaintenanceHistory> ctMaintenance;
                var repoResult = ctMaintenanceRepository.GetTotalCTMaintenance(out ctMaintenance);
                if (!repoResult.IsSuccess)
                    return Json(new { success = false, message = repoResult.Message });

                // 2) 기본정보 전체 조회 → 코드별 매핑
                ctBasicInfoRepository.GetAllCTBasicInfoRepo(out var basics);
                var basicMap = basics.ToDictionary(b => b.CT_Code, b => b);

                // 3) JSON에 Name, Serial_No 포함
                var formattedData = ctMaintenance.Select(item =>
                {
                    basicMap.TryGetValue(item.CT_Code, out var basic);
                    return new
                    {
                        item.Tbl_Idx,
                        item.CT_Code,
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

                LogHelper.WriteLog("CTMaintenanceController.List", $"조회된 데이터: {formattedData.Count}건");
                return Json(formattedData);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("CTMaintenanceController.List", $"GetTotalCTMaintenanceListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}