
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class DCCABLEMaintenanceController : Controller
    {
        // GET: DCCABLEMaintenance
        public ActionResult DCCABLEMaintenanceList(string DCCABLE_Code)
        {
            var basicInfo = dccableBasicInfoRepository.GetDCCABLEBasicInfoByCode(DCCABLE_Code);
            ViewBag.DCCABLECode = DCCABLE_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Maintenance/DCCABLE/DCCABLEMaintenanceList.cshtml");
        }

        public ActionResult DCCABLEMaintenanceTotalList()
        {
            return View("~/Views/Maintenance/Total/DCCABLEMaintenanceTotalList.cshtml");
        }

        /// <summary>
        /// DCCABLE 유지보수 데이터 가져오기
        /// </summary>
        [HttpPost]
        public JsonResult GetDCCABLEMRByDCCABLECode(string dccableCode)
        {
            Result res = new Result(true);
            List<DCCABLEMaintenanceHistory> dccableMR = new List<DCCABLEMaintenanceHistory>();

            try
            {
                LogHelper.WriteLog("DCCABLEMRController.cs", "GetDCCABLEMRByDCCABLECode 실행");

                res = dccableMaintenanceRepository.GetDCCABLEMRByDCCABLECode(dccableCode, out dccableMR);

                if (!res.IsSuccess)
                {
                    LogHelper.WriteLog("DCCABLEMRController.cs", res.Message);
                    return Json(new { success = false, message = res.Message });
                }

                if (dccableMR.Count == 0)
                {
                    LogHelper.WriteLog("DCCABLEMRController.cs", "조회된 데이터가 없습니다.");
                    return Json(new { success = true, data = new List<DCCABLEMaintenanceHistory>() });
                }

                LogHelper.WriteLog("DCCABLEMRController.cs", $"조회된 데이터: {dccableMR.Count}건");

                return Json(new { success = true, data = dccableMR });
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("DCCABLEMRController.cs", "오류 발생: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalDCCABLEMaintenanceListData()
        {
            try
            {
                LogHelper.WriteLog("TotalDCCABLEMaintenanceController.List", "GetTotalDCCABLEMaintenanceListData 실행");

                // 1) 전체 유지보수 이력 조회
                var repoResult = dccableMaintenanceRepository.GetTotalDCCABLEMaintenance(out List<DCCABLEMaintenanceHistory> dccableMaintenance);
                if (!repoResult.IsSuccess)
                    return Json(new { success = false, message = repoResult.Message });

                // 2) 기본정보 전체 조회 및 코드→기본정보 맵 생성
                dccableBasicInfoRepository.GetAllDCCABLEBasicInfoRepo(out var basics);
                var basicMap = basics.ToDictionary(b => b.DCCABLE_Code, b => b);

                // 3) 결과에 Name, Serial_No 추가
                var formattedData = dccableMaintenance.Select(item =>
                {
                    basicMap.TryGetValue(item.DCCABLE_Code, out var basic);
                    return new
                    {
                        item.Tbl_Idx,
                        item.DCCABLE_Code,
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

                LogHelper.WriteLog("DCCABLEMaintenanceController.List", $"조회된 데이터: {formattedData.Count}건");
                return Json(formattedData);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("DCCABLEMaintenanceController.List", $"GetTotalDCCABLEMaintenanceListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}