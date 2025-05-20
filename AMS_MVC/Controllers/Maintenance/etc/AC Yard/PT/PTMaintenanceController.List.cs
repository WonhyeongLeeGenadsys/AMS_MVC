using AMS_MVC.Models;
using AMS_MVC.Repositories;
using AMS_MVC.Utlity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Maintenance.PT
{
    public partial class PTMaintenanceController : Controller
    {
        // GET: PTMaintenance
        public ActionResult PTMaintenanceList(string PT_Code)
        {
            var basicInfo = ptBasicInfoRepository.GetPTBasicInfoByCode(PT_Code);
            ViewBag.PTCode = PT_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Maintenance/etc/AC Yard/PT/PTMaintenanceList.cshtml");
        }

        public ActionResult PTMaintenanceTotalList()
        {
            return View("~/Views/Maintenance/Total/etc/AC Yard/PTMaintenanceTotalList.cshtml");
        }

        /// <summary>
        /// PT 유지보수 데이터 가져오기
        /// </summary>
        [HttpPost]
        public JsonResult GetPTMRByPTCode(string ptCode)
        {
            Result res = new Result(true);
            List<PTMaintenanceHistory> ptMR = new List<PTMaintenanceHistory>();

            try
            {
                LogHelper.WriteLog("PTMRController.cs", "GetPTMRByPTCode 실행");

                res = ptMaintenanceRepository.GetPTMRByPTCode(ptCode, out ptMR);

                if (!res.IsSuccess)
                {
                    LogHelper.WriteLog("PTMRController.cs", res.Message);
                    return Json(new { success = false, message = res.Message });
                }

                if (ptMR.Count == 0)
                {
                    LogHelper.WriteLog("PTMRController.cs", "조회된 데이터가 없습니다.");
                    return Json(new { success = true, data = new List<PTMaintenanceHistory>() });
                }

                LogHelper.WriteLog("PTMRController.cs", $"조회된 데이터: {ptMR.Count}건");

                return Json(new { success = true, data = ptMR });
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("PTMRController.cs", "오류 발생: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalPTMaintenanceListData()
        {
            try
            {
                LogHelper.WriteLog("TotalPTMaintenanceController.List", "GetTotalPTMaintenanceListData 실행");

                // 1) 전체 유지보수 이력 조회
                List<PTMaintenanceHistory> ptMaintenance;
                var repoResult = ptMaintenanceRepository.GetTotalPTMaintenance(out ptMaintenance);
                if (!repoResult.IsSuccess)
                    return Json(new { success = false, message = repoResult.Message });

                // 2) 기본정보 전체 조회 → 코드별 매핑
                ptBasicInfoRepository.GetAllPTBasicInfoRepo(out var basics);
                var basicMap = basics.ToDictionary(b => b.PT_Code, b => b);

                // 3) JSON에 Name, Serial_No 포함
                var formattedData = ptMaintenance.Select(item =>
                {
                    basicMap.TryGetValue(item.PT_Code, out var basic);
                    return new
                    {
                        item.Tbl_Idx,
                        item.PT_Code,
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

                LogHelper.WriteLog("PTMaintenanceController.List", $"조회된 데이터: {formattedData.Count}건");
                return Json(formattedData);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("PTMaintenanceController.List", $"GetTotalPTMaintenanceListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}