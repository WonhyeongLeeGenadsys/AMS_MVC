using AMS_MVC.Repositories;

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AMSMVC.Controllers
{
    public class DCCABLEDeviceController : Controller
    {
        private readonly RiskmatrixRepository riskmatrixRepo = new RiskmatrixRepository();
        private readonly PriorityInfoRepository priorityRepo = new PriorityInfoRepository();
        private readonly MaintenanceRepository maintenanceRepo = new MaintenanceRepository();
        private readonly GojangRepository gojangRepo = new GojangRepository();
        private readonly DCCABLEChkRepository dccableChkRepo = new DCCABLEChkRepository();

        // DCCABLEBasicInfoRepository 사용
        private readonly DCCABLEBasicInfoRepository dccableBasicInfoRepo = new DCCABLEBasicInfoRepository();

        // DCCABLEDeviceInfo 페이지
        public ActionResult Index()
        {
            ViewBag.MenuType = "DeviceInfo";
            return View("~/Views/Device/DCCABLE/DCCABLEDevice.cshtml");
        }

        // DCCABLEDeviceController (또는 DCCABLEDeviceDetailController)에 추가
        [HttpPost]
        public JsonResult GetRiskmatrixData(string prefix)
        {
            try
            {
                // riskmatrixRepo.GetAggregatedHI(prefix) 는 { "1": count1, "2": count2, ... } 형식의 Dictionary를 반환함
                var riskData = riskmatrixRepo.GetAggregatedHI(prefix);
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
            var PofCof = riskmatrixRepo.GetRiskMatrixPofCof(prefix);
            return Json(PofCof);
        }

        /// <summary>
        /// 우선순위 데이터 가져오기 (날짜 형식 변환 포함)
        /// </summary>
        [HttpPost]
        public JsonResult GetPriorityDCCABLE()
        {
            try
            {
                var priorityData = priorityRepo.GetPriority(
                    "DCCABLEBASICINFO", // DCCABLE 기본정보 테이블
                    "DCCABLECODE",      // DCCABLE 코드 필드
                    "DCCABLE",           // 표시용 장치 이름
                    "DCCABLE"            // 별칭
                );

                // InstallDate와 OperatingDate를 "yy.MM.dd" 형식의 문자열로 변환합니다.
                var formattedData = priorityData.Select(item => new
                {
                    item.Priority,
                    item.Sort,
                    item.Code,
                    item.SerialNo,
                    item.Name,
                    InstallDate = item.InstallDate.ToString("yy.MM.dd"),
                    OperatingDate = item.OperatingDate.ToString("yy.MM.dd"),
                    item.UsagePeriod,
                    item.Price,
                    item.RatedV,
                    item.RatedA,
                    item.MakeCompany,
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
        /// 전체 DCCABLE의 월별 점검 데이터 가져오기 (JSON)
        /// </summary>
        public JsonResult GetMonthlyAllDCCABLEChkData()
        {
            try
            {
                var data = dccableChkRepo.GetMonthlyAllDCCABLEChkCounts();
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
                var data = maintenanceRepo.GetMonthlyMaintenanceCounts("DCCABLEMAINTENANCEHISTORY", "DCCABLE");
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        /// <summary>
        /// DCCABLE 단독 - 고장 테이블 정보 가져오기
        /// </summary>
        [HttpPost]
        public JsonResult GetGojangDCCABLEList()
        {
            try
            {
                // DCCABLE만 조회
                var gojangData = gojangRepo.GetGojangData(
                    "DCCABLEFAILUREHISTORY", // 고장 이력 테이블
                    "DCCABLEBASICINFO",       // 기본 정보 테이블
                    "DCCABLECODE",            // 매칭할 컬럼명
                    "DCCABLE",                 // 별칭
                    "DCCABLE"                  // EntityName (Grid에 표시용)
                );
                return Json(gojangData);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        /// <summary>
        /// 기본정보 (DCCABLE 설비 목록) 데이터 가져오기
        /// </summary>
        public JsonResult GetBasicInfoList()
        {
            try
            {
                List<dynamic> infoWithRisk;
                var result = dccableBasicInfoRepo.GetAllDCCABLEBasicInfoWithRiskMatrixRepo(out infoWithRisk);

                var formatted = infoWithRisk.Select(b => new
                {
                    DCCABLECode = b.DCCABLECode,
                    SerialNo = b.SerialNo,
                    InstallDate = b.InstallDate != null ? ((DateTime)b.InstallDate).ToString("yyyy-MM-dd") : "",
                    OperatingDate = b.OperatingDate != null ? ((DateTime)b.OperatingDate).ToString("yyyy-MM-dd") : "",
                    UsagePeriod = b.OperatingDate != null ? (DateTime.Now.Year - ((DateTime)b.OperatingDate).Year).ToString() + "년" : "",
                    HI = b.HI  // RiskMatrix 테이블에서 가져온 HI 값
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
