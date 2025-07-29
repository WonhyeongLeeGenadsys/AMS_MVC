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
    public class ESDeviceController : Controller
    {
        private readonly RiskmatrixRepository _riskmatrixRepo = new RiskmatrixRepository();
        private readonly PriorityInfoRepository _priorityRepo = new PriorityInfoRepository();
        private readonly MaintenanceRepository _maintenanceRepo = new MaintenanceRepository();
        private readonly GojangRepository _gojangRepo = new GojangRepository();
        private readonly ESChkRepository _esChkRepo = new ESChkRepository();

        // ESBasicInfoRepository 사용
        private readonly ESBasicInfoRepository _esBasicInfoRepo = new ESBasicInfoRepository();

        // ESDeviceInfo 페이지
        public ActionResult Index()
        {
            ViewBag.MenuType = "DeviceInfo";
            return View("~/Views/Device/etc/Grounding/ES/ESDevice.cshtml");
        }

        // ESDeviceController (또는 ESDeviceDetailController)에 추가
        [HttpPost]
        public JsonResult GetRiskmatrixData(IEnumerable<string> prefix)
        {
            try
            {
                // _riskmatrixRepo.GetAggregatedHI(prefix) 는 { "1": count1, "2": count2, ... } 형식의 Dictionary를 반환
                var riskData = _riskmatrixRepo.GetAggregatedHI(prefix);
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
        public JsonResult GetPriorityES()
        {
            try
            {
                var priorityData = _priorityRepo.GetPriority(
                    "ES_BASICINFO", // ES 기본정보 테이블
                    "ES_CODE",      // ES 코드 필드
                    "ES",           // 표시용 장치 이름
                    "ES",           // 별칭
                    "GROUNDING"
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
        /// 전체 ES의 월별 점검 데이터 가져오기 (JSON)
        /// </summary>
        public JsonResult GetMonthlyAllESChkData()
        {
            try
            {
                var data = _esChkRepo.GetMonthlyAllESChkCounts();
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
                var data = _maintenanceRepo.GetMonthlyMaintenanceCounts("ES_MAINTENANCE_HISTORY", "ES");
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        /// <summary>
        /// ES 단독 - 고장 테이블 정보 가져오기
        /// </summary>
        [HttpPost]
        public JsonResult GetGojangESList()
        {
            try
            {
                // ES만 조회
                var gojangData = _gojangRepo.GetGojangData(
                    "ES_FAILURE_HISTORY", // 고장 이력 테이블
                    "ES_BASICINFO",       // 기본 정보 테이블
                    "ES_CODE",            // 매칭할 컬럼명
                    "ES",                 // 별칭
                    "GROUNDING"                  // EntityName (Grid에 표시용)
                );
                return Json(gojangData);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        /// <summary>
        /// 기본정보 (ES 설비 목록) 데이터 가져오기
        /// </summary>
        public JsonResult GetBasicInfoList()
        {
            try
            {
                List<dynamic> infoWithRisk;
                var result = _esBasicInfoRepo.GetAllESBasicInfoWithRiskMatrixRepo(out infoWithRisk);

                var formatted = infoWithRisk.Select(b => new
                {
                    ES_Code = b.ES_Code,
                    Serial_No = b.Serial_No,
                    Install_Date = b.Install_Date != null ? ((DateTime)b.Install_Date).ToString("yyyy-MM-dd") : "",
                    Operating_Date = b.Operating_Date != null ? ((DateTime)b.Operating_Date).ToString("yyyy-MM-dd") : "",
                    UsagePeriod = b.Operating_Date != null ? (DateTime.Now.Year - ((DateTime)b.Operating_Date).Year).ToString() + "년" : "",
                    HI = b.HI  
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
