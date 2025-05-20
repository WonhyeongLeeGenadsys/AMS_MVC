using AMS_MVC.Models;
using AMS_MVC.Repositories;
using AMS_MVC.Utlity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Maintenance.ES
{
    public partial class ESMaintenanceController : Controller
    {
        // GET: ESMaintenance
        public ActionResult ESMaintenanceList(string ES_Code)
        {
            var basicInfo = esBasicInfoRepository.GetESBasicInfoByCode(ES_Code);
            ViewBag.ESCode = ES_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Maintenance/etc/Grounding/ES/ESMaintenanceList.cshtml");
        }

        public ActionResult ESMaintenanceTotalList()
        {
            return View("~/Views/Maintenance/Total/etc/Grounding/ESMaintenanceTotalList.cshtml");
        }

        /// <summary>
        /// ES 유지보수 데이터 가져오기
        /// </summary>
        [HttpPost]
        public JsonResult GetESMRByESCode(string esCode)
        {
            Result res = new Result(true);
            List<ESMaintenanceHistory> esMR = new List<ESMaintenanceHistory>();

            try
            {
                LogHelper.WriteLog("ESMRController.cs", "GetESMRByESCode 실행");

                res = esMaintenanceRepository.GetESMRByESCode(esCode, out esMR);

                if (!res.IsSuccess)
                {
                    LogHelper.WriteLog("ESMRController.cs", res.Message);
                    return Json(new { success = false, message = res.Message });
                }

                if (esMR.Count == 0)
                {
                    LogHelper.WriteLog("ESMRController.cs", "조회된 데이터가 없습니다.");
                    return Json(new { success = true, data = new List<ESMaintenanceHistory>() });
                }

                LogHelper.WriteLog("ESMRController.cs", $"조회된 데이터: {esMR.Count}건");

                return Json(new { success = true, data = esMR });
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("ESMRController.cs", "오류 발생: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalESMaintenanceListData()
        {
            try
            {
                LogHelper.WriteLog("TotalESMaintenanceController.List", "GetTotalESMaintenanceListData 실행");

                // 1) 전체 유지보수 이력 조회
                List<ESMaintenanceHistory> esMaintenance;
                var repoResult = esMaintenanceRepository.GetTotalESMaintenance(out esMaintenance);
                if (!repoResult.IsSuccess)
                    return Json(new { success = false, message = repoResult.Message });

                // 2) 기본정보 전체 조회 → 코드별 매핑
                esBasicInfoRepository.GetAllESBasicInfoRepo(out var basics);
                var basicMap = basics.ToDictionary(b => b.ES_Code, b => b);

                // 3) JSON에 Name, Serial_No 포함
                var formattedData = esMaintenance.Select(item =>
                {
                    basicMap.TryGetValue(item.ES_Code, out var basic);
                    return new
                    {
                        item.Tbl_Idx,
                        item.ES_Code,
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

                LogHelper.WriteLog("ESMaintenanceController.List", $"조회된 데이터: {formattedData.Count}건");
                return Json(formattedData);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("ESMaintenanceController.List", $"GetTotalESMaintenanceListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}