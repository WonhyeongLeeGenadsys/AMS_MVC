
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class DCCTMaintenanceController : Controller
    {
        // GET: DCCTMaintenance
        public ActionResult DCCTMaintenanceList(string DCCT_Code)
        {
            var basicInfo = dcctBasicInfoRepository.GetDCCTBasicInfoByCode(DCCT_Code);
            ViewBag.DCCTCode = DCCT_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Maintenance/etc/DC Yard/DCCT/DCCTMaintenanceList.cshtml");
        }

        public ActionResult DCCTMaintenanceTotalList()
        {
            return View("~/Views/Maintenance/Total/etc/DC Yard/DCCTMaintenanceTotalList.cshtml");
        }

        /// <summary>
        /// DCCT 유지보수 데이터 가져오기
        /// </summary>
        [HttpPost]
        public JsonResult GetDCCTMRByDCCTCode(string dcctCode)
        {
            Result res = new Result(true);
            List<DCCTMaintenanceHistory> dcctMR = new List<DCCTMaintenanceHistory>();

            try
            {
                LogHelper.WriteLog("DCCTMRController.cs", "GetDCCTMRByDCCTCode 실행");

                res = dcctMaintenanceRepository.GetDCCTMRByDCCTCode(dcctCode, out dcctMR);

                if (!res.IsSuccess)
                {
                    LogHelper.WriteLog("DCCTMRController.cs", res.Message);
                    return Json(new { success = false, message = res.Message });
                }

                if (dcctMR.Count == 0)
                {
                    LogHelper.WriteLog("DCCTMRController.cs", "조회된 데이터가 없습니다.");
                    return Json(new { success = true, data = new List<DCCTMaintenanceHistory>() });
                }

                LogHelper.WriteLog("DCCTMRController.cs", $"조회된 데이터: {dcctMR.Count}건");

                return Json(new { success = true, data = dcctMR });
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("DCCTMRController.cs", "오류 발생: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalDCCTMaintenanceListData()
        {
            try
            {
                LogHelper.WriteLog("TotalDCCTMaintenanceController.List", "GetTotalDCCTMaintenanceListData 실행");

                // 1) 전체 유지보수 이력 조회
                List<DCCTMaintenanceHistory> dcctMaintenance;
                var repoResult = dcctMaintenanceRepository.GetTotalDCCTMaintenance(out dcctMaintenance);
                if (!repoResult.IsSuccess)
                    return Json(new { success = false, message = repoResult.Message });

                // 2) 기본정보 전체 조회 → 코드별 매핑
                dcctBasicInfoRepository.GetAllDCCTBasicInfoRepo(out var basics);
                var basicMap = basics.ToDictionary(b => b.DCCT_Code, b => b);

                // 3) JSON에 Name, Serial_No 포함
                var formattedData = dcctMaintenance.Select(item =>
                {
                    basicMap.TryGetValue(item.DCCT_Code, out var basic);
                    return new
                    {
                        item.Tbl_Idx,
                        item.DCCT_Code,
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

                LogHelper.WriteLog("DCCTMaintenanceController.List", $"조회된 데이터: {formattedData.Count}건");
                return Json(formattedData);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("DCCTMaintenanceController.List", $"GetTotalDCCTMaintenanceListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}