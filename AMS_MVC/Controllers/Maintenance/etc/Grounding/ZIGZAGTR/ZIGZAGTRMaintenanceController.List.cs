using AMS_MVC.Models;
using AMS_MVC.Repositories;
using AMS_MVC.Utlity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Maintenance.ZIGZAGTR
{
    public partial class ZIGZAGTRMaintenanceController : Controller
    {
        // GET: ZIGZAGTRMaintenance
        public ActionResult ZIGZAGTRMaintenanceList(string ZIGZAGTR_Code)
        {
            var basicInfo = zigzagtrBasicInfoRepository.GetZIGZAGTRBasicInfoByCode(ZIGZAGTR_Code);
            ViewBag.ZIGZAGTRCode = ZIGZAGTR_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Maintenance/etc/Grounding/ZIGZAGTR/ZIGZAGTRMaintenanceList.cshtml");
        }

        public ActionResult ZIGZAGTRMaintenanceTotalList()
        {
            return View("~/Views/Maintenance/Total/etc/Grounding/ZIGZAGTRMaintenanceTotalList.cshtml");
        }

        /// <summary>
        /// ZIGZAGTR 유지보수 데이터 가져오기
        /// </summary>
        [HttpPost]
        public JsonResult GetZIGZAGTRMRByZIGZAGTRCode(string zigzagtrCode)
        {
            Result res = new Result(true);
            List<ZIGZAGTRMaintenanceHistory> zigzagtrMR = new List<ZIGZAGTRMaintenanceHistory>();

            try
            {
                LogHelper.WriteLog("ZIGZAGTRMRController.cs", "GetZIGZAGTRMRByZIGZAGTRCode 실행");

                res = zigzagtrMaintenanceRepository.GetZIGZAGTRMRByZIGZAGTRCode(zigzagtrCode, out zigzagtrMR);

                if (!res.IsSuccess)
                {
                    LogHelper.WriteLog("ZIGZAGTRMRController.cs", res.Message);
                    return Json(new { success = false, message = res.Message });
                }

                if (zigzagtrMR.Count == 0)
                {
                    LogHelper.WriteLog("ZIGZAGTRMRController.cs", "조회된 데이터가 없습니다.");
                    return Json(new { success = true, data = new List<ZIGZAGTRMaintenanceHistory>() });
                }

                LogHelper.WriteLog("ZIGZAGTRMRController.cs", $"조회된 데이터: {zigzagtrMR.Count}건");

                return Json(new { success = true, data = zigzagtrMR });
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("ZIGZAGTRMRController.cs", "오류 발생: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalZIGZAGTRMaintenanceListData()
        {
            try
            {
                LogHelper.WriteLog("TotalZIGZAGTRMaintenanceController.List", "GetTotalZIGZAGTRMaintenanceListData 실행");

                // 1) 전체 유지보수 이력 조회
                List<ZIGZAGTRMaintenanceHistory> zigzagtrMaintenance;
                var repoResult = zigzagtrMaintenanceRepository.GetTotalZIGZAGTRMaintenance(out zigzagtrMaintenance);
                if (!repoResult.IsSuccess)
                    return Json(new { success = false, message = repoResult.Message });

                // 2) 기본정보 전체 조회 → 코드별 매핑
                zigzagtrBasicInfoRepository.GetAllZIGZAGTRBasicInfoRepo(out var basics);
                var basicMap = basics.ToDictionary(b => b.ZIGZAGTR_Code, b => b);

                // 3) JSON에 Name, Serial_No 포함
                var formattedData = zigzagtrMaintenance.Select(item =>
                {
                    basicMap.TryGetValue(item.ZIGZAGTR_Code, out var basic);
                    return new
                    {
                        item.Tbl_Idx,
                        item.ZIGZAGTR_Code,
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

                LogHelper.WriteLog("ZIGZAGTRMaintenanceController.List", $"조회된 데이터: {formattedData.Count}건");
                return Json(formattedData);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("ZIGZAGTRMaintenanceController.List", $"GetTotalZIGZAGTRMaintenanceListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}