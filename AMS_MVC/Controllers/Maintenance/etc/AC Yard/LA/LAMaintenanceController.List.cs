
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class LAMaintenanceController : Controller
    {
        // GET: LAMaintenance
        public ActionResult LAMaintenanceList(string LA_Code)
        {
            var basicInfo = laBasicInfoRepository.GetLABasicInfoByCode(LA_Code);
            ViewBag.LACode = LA_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Maintenance/etc/AC Yard/LA/LAMaintenanceList.cshtml");
        }

        public ActionResult LAMaintenanceTotalList()
        {
            return View("~/Views/Maintenance/Total/etc/AC Yard/LAMaintenanceTotalList.cshtml");
        }

        /// <summary>
        /// LA 유지보수 데이터 가져오기
        /// </summary>
        [HttpPost]
        public JsonResult GetLAMRByLACode(string laCode)
        {
            Result res = new Result(true);
            List<LAMaintenanceHistory> laMR = new List<LAMaintenanceHistory>();

            try
            {
                LogHelper.WriteLog("LAMRController.cs", "GetLAMRByLACode 실행");

                res = laMaintenanceRepository.GetLAMRByLACode(laCode, out laMR);

                if (!res.IsSuccess)
                {
                    LogHelper.WriteLog("LAMRController.cs", res.Message);
                    return Json(new { success = false, message = res.Message });
                }

                if (laMR.Count == 0)
                {
                    LogHelper.WriteLog("LAMRController.cs", "조회된 데이터가 없습니다.");
                    return Json(new { success = true, data = new List<LAMaintenanceHistory>() });
                }

                LogHelper.WriteLog("LAMRController.cs", $"조회된 데이터: {laMR.Count}건");

                return Json(new { success = true, data = laMR });
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("LAMRController.cs", "오류 발생: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalLAMaintenanceListData()
        {
            try
            {
                LogHelper.WriteLog("TotalLAMaintenanceController.List", "GetTotalLAMaintenanceListData 실행");

                // 1) 전체 유지보수 이력 조회
                List<LAMaintenanceHistory> laMaintenance;
                var repoResult = laMaintenanceRepository.GetTotalLAMaintenance(out laMaintenance);
                if (!repoResult.IsSuccess)
                    return Json(new { success = false, message = repoResult.Message });

                // 2) 기본정보 전체 조회 → 코드별 매핑
                laBasicInfoRepository.GetAllLABasicInfoRepo(out var basics);
                var basicMap = basics.ToDictionary(b => b.LA_Code, b => b);

                // 3) JSON에 Name, Serial_No 포함
                var formattedData = laMaintenance.Select(item =>
                {
                    basicMap.TryGetValue(item.LA_Code, out var basic);
                    return new
                    {
                        item.Tbl_Idx,
                        item.LA_Code,
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

                LogHelper.WriteLog("LAMaintenanceController.List", $"조회된 데이터: {formattedData.Count}건");
                return Json(formattedData);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("LAMaintenanceController.List", $"GetTotalLAMaintenanceListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}