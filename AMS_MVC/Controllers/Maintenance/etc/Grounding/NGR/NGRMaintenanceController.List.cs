using AMS_MVC.Models;
using AMS_MVC.Repositories;
using AMS_MVC.Utlity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Maintenance.NGR
{
    public partial class NGRMaintenanceController : Controller
    {
        // GET: NGRMaintenance
        public ActionResult NGRMaintenanceList(string NGR_Code)
        {
            var basicInfo = ngrBasicInfoRepository.GetNGRBasicInfoByCode(NGR_Code);
            ViewBag.NGRCode = NGR_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Maintenance/etc/Grounding/NGR/NGRMaintenanceList.cshtml");
        }

        public ActionResult NGRMaintenanceTotalList()
        {
            return View("~/Views/Maintenance/Total/etc/Grounding/NGRMaintenanceTotalList.cshtml");
        }

        /// <summary>
        /// NGR 유지보수 데이터 가져오기
        /// </summary>
        [HttpPost]
        public JsonResult GetNGRMRByNGRCode(string ngrCode)
        {
            Result res = new Result(true);
            List<NGRMaintenanceHistory> ngrMR = new List<NGRMaintenanceHistory>();

            try
            {
                LogHelper.WriteLog("NGRMRController.cs", "GetNGRMRByNGRCode 실행");

                res = ngrMaintenanceRepository.GetNGRMRByNGRCode(ngrCode, out ngrMR);

                if (!res.IsSuccess)
                {
                    LogHelper.WriteLog("NGRMRController.cs", res.Message);
                    return Json(new { success = false, message = res.Message });
                }

                if (ngrMR.Count == 0)
                {
                    LogHelper.WriteLog("NGRMRController.cs", "조회된 데이터가 없습니다.");
                    return Json(new { success = true, data = new List<NGRMaintenanceHistory>() });
                }

                LogHelper.WriteLog("NGRMRController.cs", $"조회된 데이터: {ngrMR.Count}건");

                return Json(new { success = true, data = ngrMR });
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("NGRMRController.cs", "오류 발생: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalNGRMaintenanceListData()
        {
            try
            {
                LogHelper.WriteLog("TotalNGRMaintenanceController.List", "GetTotalNGRMaintenanceListData 실행");

                // 1) 전체 유지보수 이력 조회
                List<NGRMaintenanceHistory> ngrMaintenance;
                var repoResult = ngrMaintenanceRepository.GetTotalNGRMaintenance(out ngrMaintenance);
                if (!repoResult.IsSuccess)
                    return Json(new { success = false, message = repoResult.Message });

                // 2) 기본정보 전체 조회 → 코드별 매핑
                ngrBasicInfoRepository.GetAllNGRBasicInfoRepo(out var basics);
                var basicMap = basics.ToDictionary(b => b.NGR_Code, b => b);

                // 3) JSON에 Name, Serial_No 포함
                var formattedData = ngrMaintenance.Select(item =>
                {
                    basicMap.TryGetValue(item.NGR_Code, out var basic);
                    return new
                    {
                        item.Tbl_Idx,
                        item.NGR_Code,
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

                LogHelper.WriteLog("NGRMaintenanceController.List", $"조회된 데이터: {formattedData.Count}건");
                return Json(formattedData);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("NGRMaintenanceController.List", $"GetTotalNGRMaintenanceListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}