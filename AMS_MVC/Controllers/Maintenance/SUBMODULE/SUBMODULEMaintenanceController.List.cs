
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class SUBMODULEMaintenanceController : Controller
    {
        // GET: SUBMODULEMaintenance
        public ActionResult SUBMODULEMaintenanceList(string SUBMODULE_Code)
        {
            var basicInfo = submoduleBasicInfoRepository.GetSUBMODULEBasicInfoByCode(SUBMODULE_Code);
            ViewBag.SUBMODULECode = SUBMODULE_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Maintenance/SUBMODULE/SUBMODULEMaintenanceList.cshtml");
        }

        public ActionResult SUBMODULEMaintenanceTotalList()
        {
            return View("~/Views/Maintenance/Total/SUBMODULEMaintenanceTotalList.cshtml");
        }

        /// <summary>
        /// SUBMODULE 유지보수 데이터 가져오기
        /// </summary>
        [HttpPost]
        public JsonResult GetSUBMODULEMRBySUBMODULECode(string submoduleCode)
        {
            Result res = new Result(true);
            List<SUBMODULEMaintenanceHistory> submoduleMR = new List<SUBMODULEMaintenanceHistory>();

            try
            {
                LogHelper.WriteLog("SUBMODULEMRController.cs", "GetSUBMODULEMRBySUBMODULECode 실행");

                res = submoduleMaintenanceRepository.GetSUBMODULEMRBySUBMODULECode(submoduleCode, out submoduleMR);

                if (!res.IsSuccess)
                {
                    LogHelper.WriteLog("SUBMODULEMRController.cs", res.Message);
                    return Json(new { success = false, message = res.Message });
                }

                if (submoduleMR.Count == 0)
                {
                    LogHelper.WriteLog("SUBMODULEMRController.cs", "조회된 데이터가 없습니다.");
                    return Json(new { success = true, data = new List<SUBMODULEMaintenanceHistory>() });
                }

                LogHelper.WriteLog("SUBMODULEMRController.cs", $"조회된 데이터: {submoduleMR.Count}건");

                return Json(new { success = true, data = submoduleMR });
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("SUBMODULEMRController.cs", "오류 발생: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalSUBMODULEMaintenanceListData()
        {
            try
            {
                LogHelper.WriteLog("TotalSUBMODULEMaintenanceController.List", "GetTotalSUBMODULEMaintenanceListData 실행");

                List<SUBMODULEMaintenanceHistory> submoduleMaintenance;
                var repoResult = submoduleMaintenanceRepository.GetTotalSUBMODULEMaintenance(out submoduleMaintenance);
                if (!repoResult.IsSuccess)
                    return Json(new { success = false, message = repoResult.Message });

                submoduleBasicInfoRepository.GetAllSUBMODULEBasicInfoRepo(out var basics);
                var basicMap = basics.ToDictionary(b => b.SUBMODULE_Code, b => b);

                var formattedData = submoduleMaintenance.Select(item =>
                {
                    basicMap.TryGetValue(item.SUBMODULE_Code, out var basic);
                    return new
                    {
                        item.Tbl_Idx,
                        item.SUBMODULE_Code,
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

                LogHelper.WriteLog("SUBMODULEMaintenanceController.List", $"조회된 데이터: {formattedData.Count}건");
                return Json(formattedData);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("SUBMODULEMaintenanceController.List", $"GetTotalSUBMODULEMaintenanceListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}