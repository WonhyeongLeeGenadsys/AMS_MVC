
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class DSMaintenanceController : Controller
    {
        // GET: DSMaintenance
        public ActionResult DSMaintenanceList(string DS_Code)
        {
            var basicInfo = dsBasicInfoRepository.GetDSBasicInfoByCode(DS_Code);
            ViewBag.DSCode = DS_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Maintenance/etc/AC Yard/DS/DSMaintenanceList.cshtml");
        }

        public ActionResult DSMaintenanceTotalList()
        {
            return View("~/Views/Maintenance/Total/etc/AC Yard/DSMaintenanceTotalList.cshtml");
        }

        /// <summary>
        /// DS 유지보수 데이터 가져오기
        /// </summary>
        [HttpPost]
        public JsonResult GetDSMRByDSCode(string dsCode)
        {
            Result res = new Result(true);
            List<DSMaintenanceHistory> dsMR = new List<DSMaintenanceHistory>();

            try
            {
                LogHelper.WriteLog("DSMRController.cs", "GetDSMRByDSCode 실행");

                res = dsMaintenanceRepository.GetDSMRByDSCode(dsCode, out dsMR);

                if (!res.IsSuccess)
                {
                    LogHelper.WriteLog("DSMRController.cs", res.Message);
                    return Json(new { success = false, message = res.Message });
                }

                if (dsMR.Count == 0)
                {
                    LogHelper.WriteLog("DSMRController.cs", "조회된 데이터가 없습니다.");
                    return Json(new { success = true, data = new List<DSMaintenanceHistory>() });
                }

                LogHelper.WriteLog("DSMRController.cs", $"조회된 데이터: {dsMR.Count}건");

                return Json(new { success = true, data = dsMR });
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("DSMRController.cs", "오류 발생: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalDSMaintenanceListData()
        {
            try
            {
                LogHelper.WriteLog("TotalDSMaintenanceController.List", "GetTotalDSMaintenanceListData 실행");

                // 1) 전체 유지보수 이력 조회
                List<DSMaintenanceHistory> dsMaintenance;
                var repoResult = dsMaintenanceRepository.GetTotalDSMaintenance(out dsMaintenance);
                if (!repoResult.IsSuccess)
                    return Json(new { success = false, message = repoResult.Message });

                // 2) 기본정보 전체 조회 → 코드별 매핑
                dsBasicInfoRepository.GetAllDSBasicInfoRepo(out var basics);
                var basicMap = basics.ToDictionary(b => b.DS_Code, b => b);

                // 3) JSON에 Name, Serial_No 포함
                var formattedData = dsMaintenance.Select(item =>
                {
                    basicMap.TryGetValue(item.DS_Code, out var basic);
                    return new
                    {
                        item.Tbl_Idx,
                        item.DS_Code,
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

                LogHelper.WriteLog("DSMaintenanceController.List", $"조회된 데이터: {formattedData.Count}건");
                return Json(formattedData);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("DSMaintenanceController.List", $"GetTotalDSMaintenanceListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}