
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public class ITRDeviceController : Controller
    {
        private readonly RiskmatrixRepository _riskmatrixRepo = new RiskmatrixRepository();
        private readonly PriorityInfoRepository _priorityRepo = new PriorityInfoRepository();
        private readonly MaintenanceRepository _maintenanceRepo = new MaintenanceRepository();
        private readonly GojangRepository _gojangRepo = new GojangRepository();
        private readonly ITRChk1Repository _itrChk1Repo = new ITRChk1Repository();
        private readonly ITRChk2Repository _itrChk2Repo = new ITRChk2Repository();

        // ITRBasicInfoRepository 사용
        private readonly ITRBasicInfoRepository _itrBasicInfoRepo = new ITRBasicInfoRepository();

        // ITRDeviceInfo 페이지
        public ActionResult Index()
        {
            ViewBag.MenuType = "DeviceInfo";
            return View("~/Views/Device/ITR/ITRDevice.cshtml");
        }

        // ITRDeviceController (또는 ITRDeviceDetailController)에 추가
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
        public JsonResult GetPriorityITR()
        {
            try
            {
                var priorityData = _priorityRepo.GetPriority(
                    "ITR_BASICINFO", // ITR 기본정보 테이블
                    "ITR_CODE",      // ITR 코드 필드
                    "ITR",           // 표시용 장치 이름
                    "ITR",            // 별칭
                    "AC"
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
        /// 전체 ITR의 월별 점검 데이터 가져오기 (JSON)
        /// </summary>
        /// <summary>
        /// ITR 전체(Chk1 + Chk2) 월별 검사 횟수 가져오기
        /// </summary>
        [HttpGet]
        public JsonResult GetMonthlyAllITRChkData()
        {
            try
            {
                // 1) 두 리포지토리에서 각각 데이터를 가져옵니다.
                var chk1 = _itrChk1Repo.GetMonthlyAllITRChk1Counts();
                var chk2 = _itrChk2Repo.GetMonthlyAllITRChk2Counts();

                // 2) 합치고 같은 Month끼리 묶어서 Count를 합산
                var merged = chk1
                    .Concat(chk2)
                    .GroupBy(x => x.Month)
                    .Select(g => new {
                        Month = g.Key,           // 예: "2024-07"
                Count = g.Sum(x => x.Count)
                    })
                    .OrderBy(x => x.Month)
                    .ToList();

                return Json(merged, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// 유지보수 한 달 간격 데이터 가져오기
        /// </summary>
        public JsonResult GetMonthlyMaintenanceData()
        {
            try
            {
                var data = _maintenanceRepo.GetMonthlyMaintenanceCounts("ITR_MAINTENANCE_HISTORY", "ITR");
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        /// <summary>
        /// ITR 단독 - 고장 테이블 정보 가져오기
        /// </summary>
        [HttpPost]
        public JsonResult GetGojangITRList()
        {
            try
            {
                // ITR만 조회
                var gojangData = _gojangRepo.GetGojangData(
                    "ITR_FAILURE_HISTORY", // 고장 이력 테이블
                    "ITR_BASICINFO",       // 기본 정보 테이블
                    "ITR_CODE",            // 매칭할 컬럼명
                    "ITR",                 // 별칭
                    "AC"                  // EntityName (Grid에 표시용)
                );
                return Json(gojangData);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        /// <summary>
        /// 기본정보 (ITR 설비 목록) 데이터 가져오기
        /// </summary>
        public JsonResult GetBasicInfoList()
        {
            try
            {
                List<dynamic> infoWithRisk;
                var result = _itrBasicInfoRepo.GetAllITRBasicInfoWithRiskMatrixRepo(out infoWithRisk);

                var formatted = infoWithRisk.Select(b => new
                {
                    ITR_Code = b.ITR_Code,
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
