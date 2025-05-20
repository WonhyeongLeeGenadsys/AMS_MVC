using AMS_MVC.Models;
using AMS_MVC.Repositories;
using AMS_MVC.Utlity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Maintenance.ARMREACTOR
{
    public partial class ARMREACTORMaintenanceController : Controller
    {
        // GET: ARMREACTORMaintenance
        public ActionResult ARMREACTORMaintenanceList(string ARMREACTOR_Code)
        {
            var basicInfo = armreactorBasicInfoRepository.GetARMREACTORBasicInfoByCode(ARMREACTOR_Code);
            ViewBag.ARMREACTORCode = ARMREACTOR_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Maintenance/etc/AC Yard/ARMREACTOR/ARMREACTORMaintenanceList.cshtml");
        }

        public ActionResult ARMREACTORMaintenanceTotalList()
        {
            return View("~/Views/Maintenance/Total/etc/AC Yard/ARMREACTORMaintenanceTotalList.cshtml");
        }

        /// <summary>
        /// ARMREACTOR 유지보수 데이터 가져오기
        /// </summary>
        [HttpPost]
        public JsonResult GetARMREACTORMRByARMREACTORCode(string armreactorCode)
        {
            Result res = new Result(true);
            List<ARMREACTORMaintenanceHistory> armreactorMR = new List<ARMREACTORMaintenanceHistory>();

            try
            {
                LogHelper.WriteLog("ARMREACTORMRController.cs", "GetARMREACTORMRByARMREACTORCode 실행");

                res = armreactorMaintenanceRepository.GetARMREACTORMRByARMREACTORCode(armreactorCode, out armreactorMR);

                if (!res.IsSuccess)
                {
                    LogHelper.WriteLog("ARMREACTORMRController.cs", res.Message);
                    return Json(new { success = false, message = res.Message });
                }

                if (armreactorMR.Count == 0)
                {
                    LogHelper.WriteLog("ARMREACTORMRController.cs", "조회된 데이터가 없습니다.");
                    return Json(new { success = true, data = new List<ARMREACTORMaintenanceHistory>() });
                }

                LogHelper.WriteLog("ARMREACTORMRController.cs", $"조회된 데이터: {armreactorMR.Count}건");

                return Json(new { success = true, data = armreactorMR });
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("ARMREACTORMRController.cs", "오류 발생: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalARMREACTORMaintenanceListData()
        {
            try
            {
                LogHelper.WriteLog("TotalARMREACTORMaintenanceController.List", "GetTotalARMREACTORMaintenanceListData 실행");

                // 1) 전체 유지보수 이력 조회
                List<ARMREACTORMaintenanceHistory> armreactorMaintenance;
                var repoResult = armreactorMaintenanceRepository.GetTotalARMREACTORMaintenance(out armreactorMaintenance);
                if (!repoResult.IsSuccess)
                    return Json(new { success = false, message = repoResult.Message });

                // 2) 기본정보 전체 조회 → 코드별 매핑
                armreactorBasicInfoRepository.GetAllARMREACTORBasicInfoRepo(out var basics);
                var basicMap = basics.ToDictionary(b => b.ARMREACTOR_Code, b => b);

                // 3) JSON에 Name, Serial_No 포함
                var formattedData = armreactorMaintenance.Select(item =>
                {
                    basicMap.TryGetValue(item.ARMREACTOR_Code, out var basic);
                    return new
                    {
                        item.Tbl_Idx,
                        item.ARMREACTOR_Code,
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

                LogHelper.WriteLog("ARMREACTORMaintenanceController.List", $"조회된 데이터: {formattedData.Count}건");
                return Json(formattedData);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("ARMREACTORMaintenanceController.List", $"GetTotalARMREACTORMaintenanceListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}