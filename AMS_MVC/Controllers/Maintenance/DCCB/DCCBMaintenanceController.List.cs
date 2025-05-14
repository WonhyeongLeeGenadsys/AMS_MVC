using AMS_MVC.Models;
using AMS_MVC.Repositories;
using AMS_MVC.Utlity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Maintenance.DCCB
{
    public partial class DCCBMaintenanceController : Controller
    {
        // GET: DCCBMaintenance
        public ActionResult DCCBMaintenanceList(string DCCB_Code)
        {
            var basicInfo = dccbBasicInfoRepository.GetDCCBBasicInfoByCode(DCCB_Code);
            ViewBag.DCCBCode = DCCB_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Maintenance/DCCB/DCCBMaintenanceList.cshtml");
        }

        public ActionResult DCCBMaintenanceTotalList()
        {
            return View("~/Views/Maintenance/Total/DCCBMaintenanceTotalList.cshtml");
        }

        /// <summary>
        /// DCCB 유지보수 데이터 가져오기
        /// </summary>
        [HttpPost]
        public JsonResult GetDCCBMRByDCCBCode(string dccbCode)
        {
            Result res = new Result(true);
            List<DCCBMaintenanceHistory> dccbMR = new List<DCCBMaintenanceHistory>();

            try
            {
                LogHelper.WriteLog("DCCBMRController.cs", "GetDCCBMRByDCCBCode 실행");

                res = dccbMaintenanceRepository.GetDCCBMRByDCCBCode(dccbCode, out dccbMR);

                if (!res.IsSuccess)
                {
                    LogHelper.WriteLog("DCCBMRController.cs", res.Message);
                    return Json(new { success = false, message = res.Message });
                }

                if (dccbMR.Count == 0)
                {
                    LogHelper.WriteLog("DCCBMRController.cs", "조회된 데이터가 없습니다.");
                    return Json(new { success = true, data = new List<DCCBMaintenanceHistory>() });
                }

                LogHelper.WriteLog("DCCBMRController.cs", $"조회된 데이터: {dccbMR.Count}건");

                return Json(new { success = true, data = dccbMR });
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("DCCBMRController.cs", "오류 발생: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalDCCBMaintenanceListData()
        {
            try
            {
                LogHelper.WriteLog("TotalDCCBMaintenanceController.List", "GetTotalDCCBMaintenanceListData 실행");

                // 1) 전체 유지보수 이력 조회
                var repoResult = dccbMaintenanceRepository.GetTotalDCCBMaintenance(out List<DCCBMaintenanceHistory> dccbMaintenance);
                if (!repoResult.IsSuccess)
                    return Json(new { success = false, message = repoResult.Message });

                // 2) 기본정보 전체 조회 및 코드→기본정보 맵 생성
                dccbBasicInfoRepository.GetAllDCCBBasicInfoRepo(out var basics);
                var basicMap = basics.ToDictionary(b => b.DCCB_Code, b => b);

                // 3) 결과에 Name, Serial_No 추가
                var formattedData = dccbMaintenance.Select(item =>
                {
                    basicMap.TryGetValue(item.DCCB_Code, out var basic);
                    return new
                    {
                        item.Tbl_Idx,
                        item.DCCB_Code,
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

                LogHelper.WriteLog("DCCBMaintenanceController.List", $"조회된 데이터: {formattedData.Count}건");
                return Json(formattedData);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("DCCBMaintenanceController.List", $"GetTotalDCCBMaintenanceListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}