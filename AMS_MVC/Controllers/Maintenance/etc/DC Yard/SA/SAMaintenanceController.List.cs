
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class SAMaintenanceController : Controller
    {
        // GET: SAMaintenance
        public ActionResult SAMaintenanceList(string SA_Code)
        {
            var basicInfo = saBasicInfoRepository.GetSABasicInfoByCode(SA_Code);
            ViewBag.SACode = SA_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Maintenance/etc/DC Yard/SA/SAMaintenanceList.cshtml");
        }

        public ActionResult SAMaintenanceTotalList()
        {
            return View("~/Views/Maintenance/Total/etc/DC Yard/SAMaintenanceTotalList.cshtml");
        }

        /// <summary>
        /// SA 유지보수 데이터 가져오기
        /// </summary>
        [HttpPost]
        public JsonResult GetSAMRBySACode(string saCode)
        {
            Result res = new Result(true);
            List<SAMaintenanceHistory> saMR = new List<SAMaintenanceHistory>();

            try
            {
                LogHelper.WriteLog("SAMRController.cs", "GetSAMRBySACode 실행");

                res = saMaintenanceRepository.GetSAMRBySACode(saCode, out saMR);

                if (!res.IsSuccess)
                {
                    LogHelper.WriteLog("SAMRController.cs", res.Message);
                    return Json(new { success = false, message = res.Message });
                }

                if (saMR.Count == 0)
                {
                    LogHelper.WriteLog("SAMRController.cs", "조회된 데이터가 없습니다.");
                    return Json(new { success = true, data = new List<SAMaintenanceHistory>() });
                }

                LogHelper.WriteLog("SAMRController.cs", $"조회된 데이터: {saMR.Count}건");

                return Json(new { success = true, data = saMR });
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("SAMRController.cs", "오류 발생: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalSAMaintenanceListData()
        {
            try
            {
                LogHelper.WriteLog("TotalSAMaintenanceController.List", "GetTotalSAMaintenanceListData 실행");

                // 1) 전체 유지보수 이력 조회
                List<SAMaintenanceHistory> saMaintenance;
                var repoResult = saMaintenanceRepository.GetTotalSAMaintenance(out saMaintenance);
                if (!repoResult.IsSuccess)
                    return Json(new { success = false, message = repoResult.Message });

                // 2) 기본정보 전체 조회 → 코드별 매핑
                saBasicInfoRepository.GetAllSABasicInfoRepo(out var basics);
                var basicMap = basics.ToDictionary(b => b.SA_Code, b => b);

                // 3) JSON에 Name, Serial_No 포함
                var formattedData = saMaintenance.Select(item =>
                {
                    basicMap.TryGetValue(item.SA_Code, out var basic);
                    return new
                    {
                        item.Tbl_Idx,
                        item.SA_Code,
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

                LogHelper.WriteLog("SAMaintenanceController.List", $"조회된 데이터: {formattedData.Count}건");
                return Json(formattedData);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("SAMaintenanceController.List", $"GetTotalSAMaintenanceListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}