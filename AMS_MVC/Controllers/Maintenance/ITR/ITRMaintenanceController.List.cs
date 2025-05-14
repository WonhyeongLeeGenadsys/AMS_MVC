using AMS_MVC.Models;
using AMS_MVC.Utlity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Maintenance.ITR
{
    public partial class ITRMaintenanceController : Controller
    {
        // GET: ITRMaintenance
        public ActionResult ITRMaintenanceList(string ITR_Code)
        {
            var basicInfo = itrBasicInfoRepository.GetITRBasicInfoByITRCode(ITR_Code);
            ViewBag.ITRCode = ITR_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Maintenance/ITR/ITRMaintenanceList.cshtml");
        }

        public ActionResult ITRMaintenanceTotalList()
        {
            return View("~/Views/Maintenance/Total/ITRMaintenanceTotalList.cshtml");
        }

        /// <summary>
        /// ITR 유지보수 데이터 가져오기
        /// </summary>
        [HttpPost]
        public JsonResult GetITRMRByITRCode(string itrCode)
        {
            Result res = new Result(true);
            List<ITRMaintenanceHistory> itrMR = new List<ITRMaintenanceHistory>();

            try
            {
                LogHelper.WriteLog("ITRMRController.cs", "GetITRMRByITRCode 실행");

                res = itrMaintenanceRepository.GetITRMRByITRCode(itrCode, out itrMR);

                if (!res.IsSuccess)
                {
                    LogHelper.WriteLog("ITRMRController.cs", res.Message);
                    return Json(new { success = false, message = res.Message });
                }

                if (itrMR.Count == 0)
                {
                    LogHelper.WriteLog("ITRMRController.cs", "조회된 데이터가 없습니다.");
                    return Json(new { success = true, data = new List<ITRMaintenanceHistory>() });
                }

                LogHelper.WriteLog("ITRMRController.cs", $"조회된 데이터: {itrMR.Count}건");

                return Json(new { success = true, data = itrMR });
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("ITRMRController.cs", "오류 발생: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalITRMaintenanceListData()
        {
            try
            {
                LogHelper.WriteLog("TotalITRMaintenanceController.List", "GetTotalITRMaintenanceListData 실행");

                // 1) 전체 유지보수 이력 조회
                var repoResult = itrMaintenanceRepository.GetTotalITRMaintenance(out List<ITRMaintenanceHistory> itrMaintenance);
                if (!repoResult.IsSuccess)
                    return Json(new { success = false, message = repoResult.Message });

                // 2) 기본정보 전체 조회 및 코드→기본정보 맵 생성
                itrBasicInfoRepository.GetAllITRBasicInfoRepo(out var basics);
                var basicMap = basics.ToDictionary(b => b.ITR_Code, b => b);

                // 3) 결과에 Name, Serial_No 추가
                var formattedData = itrMaintenance.Select(item =>
                {
                    basicMap.TryGetValue(item.ITR_Code, out var basic);
                    return new
                    {
                        item.Tbl_Idx,
                        item.ITR_Code,
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

                LogHelper.WriteLog("ITRMaintenanceController.List", $"조회된 데이터: {formattedData.Count}건");
                return Json(formattedData);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("ITRMaintenanceController.List", $"GetTotalITRMaintenanceListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}