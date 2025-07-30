using AMS_MVC.Database;
using AMS_MVC.Models;
using AMS_MVC.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AMS_MVC.Controllers
{
    public class BYPASSVALVEDeviceController : Controller
    {
        private readonly RiskmatrixRepository _riskmatrixRepo = new RiskmatrixRepository();
        private readonly PriorityInfoRepository _priorityRepo = new PriorityInfoRepository();
        private readonly MaintenanceRepository _maintenanceRepo = new MaintenanceRepository();
        private readonly GojangRepository _gojangRepo = new GojangRepository();
        private readonly BYPASSVALVEChkRepository _bypassvalveChkRepo = new BYPASSVALVEChkRepository();

        // BYPASSVALVEBasicInfoRepository 사용
        private readonly BYPASSVALVEBasicInfoRepository _bypassvalveBasicInfoRepo = new BYPASSVALVEBasicInfoRepository();

        // BYPASSVALVEDeviceInfo 페이지
        public ActionResult Index()
        {
            ViewBag.MenuType = "DeviceInfo";
            return View("~/Views/Device/etc/Cooling System/BYPASSVALVE/BYPASSVALVEDevice.cshtml");
        }

        // BYPASSVALVEDeviceController (또는 BYPASSVALVEDeviceDetailController)에 추가
        [HttpPost]
        public JsonResult GetRiskmatrixData(IEnumerable<string> prefix)
        {
            try
            {
                // _riskmatrixRepo.GetAggregatedHI(prefix) 는 { "1": count1, "2": count2, ... } 형식의 Dictionary를 반환
                var riskData = _riskmatrixRepo.GetLatestHIByCode(prefix);
                return Json(riskData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Riskmatrix PoF, CoF 데이터 가져오기
        /// </summary>
        public JsonResult GetRiskMatrixPofCof(string prefix)
        {
            var PofCof = _riskmatrixRepo.GetRiskMatrixPofCof(prefix);
            return Json(PofCof);
        }

        /// <summary>
        /// 우선순위 데이터 가져오기 (날짜 형식 변환 포함)
        /// </summary>
        [HttpPost]
        public JsonResult GetPriorityBYPASSVALVE()
        {
            try
            {
                var priorityData = _priorityRepo.GetPriority(
                    "BYPASSVALVE_BASICINFO", // BYPASSVALVE 기본정보 테이블
                    "BYPASSVALVE_CODE",      // BYPASSVALVE 코드 필드
                    "BYPASSVALVE",           // 표시용 장치 이름
                    "BYPASSVALVE",           // 별칭
                    "COOLING SYSTEM"
                );

                var formattedData = priorityData.Select(item => new
                {
                    item.Priority,
                    item.Sort,
                    item.Code,
                    item.Serial_No,
                    item.Name,
                    Install_Date = item.Install_Date.ToString("yy.MM.dd"),
                    Operating_Date = item.Operating_Date.ToString("yy.MM.dd"),
                    item.UsagePeriod,
                    item.Price,
                    item.Rated_V,
                    item.Rated_A,
                    item.Make_Company,
                    item.Writer,
                    item.CoF,
                    item.PoF,
                    item.HI
                }).ToList();

                return Json(formattedData);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        /// <summary>
        /// 전체 BYPASSVALVE의 월별 점검 데이터 가져오기 (JSON)
        /// </summary>
        public JsonResult GetMonthlyAllBYPASSVALVEChkData()
        {
            try
            {
                var data = _bypassvalveChkRepo.GetMonthlyAllBYPASSVALVEChkCounts();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (System.Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        /// <summary>
        /// 유지보수 한 달 간격 데이터 가져오기
        /// </summary>
        public JsonResult GetMonthlyMaintenanceData()
        {
            try
            {
                var data = _maintenanceRepo.GetMonthlyMaintenanceCounts("BYPASSVALVE_MAINTENANCE_HISTORY", "BYPASSVALVE");
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        /// <summary>
        /// BYPASSVALVE 단독 - 고장 테이블 정보 가져오기
        /// </summary>
        [HttpPost]
        public JsonResult GetGojangBYPASSVALVEList()
        {
            try
            {
                // BYPASSVALVE만 조회
                var gojangData = _gojangRepo.GetGojangData(
                    "BYPASSVALVE_FAILURE_HISTORY", // 고장 이력 테이블
                    "BYPASSVALVE_BASICINFO",       // 기본 정보 테이블
                    "BYPASSVALVE_CODE",            // 매칭할 컬럼명
                    "BYPASSVALVE",                 // 별칭
                    "COOLING SYSTEM"                  // EntityName (Grid에 표시용)
                );
                return Json(gojangData);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        /// <summary>
        /// 기본정보 (BYPASSVALVE 설비 목록) 데이터 가져오기
        /// </summary>
        public JsonResult GetBasicInfoList()
        {
            try
            {
                List<dynamic> infoWithRisk;
                var result = _bypassvalveBasicInfoRepo.GetAllBYPASSVALVEBasicInfoWithRiskMatrixRepo(out infoWithRisk);

                var formatted = infoWithRisk.Select(b => new
                {
                    BYPASSVALVE_Code = b.BYPASSVALVE_Code,
                    Serial_No = b.Serial_No,
                    Install_Date = b.Install_Date != null ? ((DateTime)b.Install_Date).ToString("yyyy-MM-dd") : "",
                    Operating_Date = b.Operating_Date != null ? ((DateTime)b.Operating_Date).ToString("yyyy-MM-dd") : "",
                    UsagePeriod = b.Operating_Date != null ? (DateTime.Now.Year - ((DateTime)b.Operating_Date).Year).ToString() + "년" : "",
                    HI = b.HI,
                    Remain_Life = b.Remain_Life

                }).ToList();

                return Json(formatted, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

    }
}
