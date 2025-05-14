using AMS_MVC.Models;
using AMS_MVC.Repositories;
using AMS_MVC.Utlity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Maintenance.VCB
{
    public partial class VCBMaintenanceController : Controller
    {
        // GET: VCBMaintenance
        public ActionResult VCBMaintenanceList(string VCB_Code)
        {
            var basicInfo = vcbBasicInfoRepository.GetVCBBasicInfoByCode(VCB_Code);
            ViewBag.VCBCode = VCB_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Maintenance/VCB/VCBMaintenanceList.cshtml");
        }

        public ActionResult VCBMaintenanceTotalList()
        {
            return View("~/Views/Maintenance/Total/VCBMaintenanceTotalList.cshtml");
        }

        /// <summary>
        /// VCB 유지보수 데이터 가져오기
        /// </summary>
        [HttpPost]
        public JsonResult GetVCBMRByVCBCode(string vcbCode)
        {
            Result res = new Result(true);
            List<VCBMaintenanceHistory> vcbMR = new List<VCBMaintenanceHistory>();

            try
            {
                LogHelper.WriteLog("VCBMRController.cs", "GetVCBMRByVCBCode 실행");

                res = vcbMaintenanceRepository.GetVCBMRByVCBCode(vcbCode, out vcbMR);

                if (!res.IsSuccess)
                {
                    LogHelper.WriteLog("VCBMRController.cs", res.Message);
                    return Json(new { success = false, message = res.Message });
                }

                if (vcbMR.Count == 0)
                {
                    LogHelper.WriteLog("VCBMRController.cs", "조회된 데이터가 없습니다.");
                    return Json(new { success = true, data = new List<VCBMaintenanceHistory>() });
                }

                LogHelper.WriteLog("VCBMRController.cs", $"조회된 데이터: {vcbMR.Count}건");

                return Json(new { success = true, data = vcbMR });
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("VCBMRController.cs", "오류 발생: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalVCBMaintenanceListData()
        {
            try
            {
                LogHelper.WriteLog("TotalVCBMaintenanceController.List", "GetTotalVCBMaintenanceListData 실행");

                // 1) 전체 유지보수 이력 조회
                List<VCBMaintenanceHistory> vcbMaintenance;
                var repoResult = vcbMaintenanceRepository.GetTotalVCBMaintenance(out vcbMaintenance);
                if (!repoResult.IsSuccess)
                    return Json(new { success = false, message = repoResult.Message });

                // 2) 기본정보 전체 조회 → 코드별 매핑
                vcbBasicInfoRepository.GetAllVCBBasicInfoRepo(out var basics);
                var basicMap = basics.ToDictionary(b => b.VCB_Code, b => b);

                // 3) JSON에 Name, Serial_No 포함
                var formattedData = vcbMaintenance.Select(item =>
                {
                    basicMap.TryGetValue(item.VCB_Code, out var basic);
                    return new
                    {
                        item.Tbl_Idx,
                        item.VCB_Code,
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

                LogHelper.WriteLog("VCBMaintenanceController.List", $"조회된 데이터: {formattedData.Count}건");
                return Json(formattedData);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("VCBMaintenanceController.List", $"GetTotalVCBMaintenanceListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}