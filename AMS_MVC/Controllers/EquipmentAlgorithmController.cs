using AMS_MVC.Models;
using AMS_MVC.Repositories;
using LaModule;  // LaAlgorithm
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AMS_MVC.Controllers
{
    public class EquipmentAlgorithmController : Controller
    {
        private EquipmentWeibullRepository _weibullRepo = new EquipmentWeibullRepository();
        private VCBBasicInfoRepository _vcbBasicRepo = new VCBBasicInfoRepository();

        [HttpGet]
        public ActionResult GetB3HistogramVCB()
        {
            // 1) EquipmentWeibull 테이블에서 VCB 데이터 가져오기
            var eqList = _weibullRepo.GetAll();
            var vcbList = eqList.Where(eq => eq.EquipmentName.ToUpper().Contains("VCB")).ToList();
            if (vcbList.Count == 0)
            {
                return Json(new { error = "해당 장비가 없습니다." }, JsonRequestBehavior.AllowGet);
            }

            // 2) 첫 번째 VCB의 shape/scale 혹은 FailureRate를 사용하여 B3 계산
            var first = vcbList.FirstOrDefault(eq => eq.ShapeParam.HasValue && eq.ScaleParam.HasValue)
                        ?? vcbList.FirstOrDefault(eq => eq.FailureRate.HasValue);
            if (first == null)
            {
                return Json(new { error = "Weibull이나 고장률 데이터가 없습니다." }, JsonRequestBehavior.AllowGet);
            }

            double b3 = 0;
            var algo = new LaAlgorithm();
            if (first.ShapeParam.HasValue && first.ScaleParam.HasValue)
            {
                algo.SetWeibull(first.ShapeParam.Value, first.ScaleParam.Value, 10);
                b3 = algo.B3Life;
            }
            else if (first.FailureRate.HasValue)
            {
                algo.SetFailureRate(first.FailureRate.Value);
                b3 = algo.B3Life;
            }

            // 3) VCBBasicInfo 테이블에서 VCB 기본정보 가져오기 (가동일 등)
            List<VCBBasicInfo> vcbBasicList;
            var resBasic = _vcbBasicRepo.GetAllVCBBasicInfoRepo(out vcbBasicList);
            if (!resBasic.IsSuccess || vcbBasicList == null || vcbBasicList.Count == 0)
            {
                return Json(new { error = "VCB_BASICINFO에 VCB가 없습니다." }, JsonRequestBehavior.AllowGet);
            }

            // 4) 각 VCB의 사용 기간(년) 계산 (Operating_Date 기반)
            var usageYears = new List<double>();
            foreach (var vcb in vcbBasicList)
            {
                double used = 0;
                if (vcb.Operating_Date.HasValue)
                {
                    used = DateTime.Now.Year - vcb.Operating_Date.Value.Year;
                }
                usageYears.Add(used);
            }

            // 5) 0부터 TimeMax까지, binSize=10 단위 bin 생성
            //    TimeMax는 알고리즘에서 생성한 시간 축의 최대값(예: filteredT.Max())를 사용합니다.
            double timeMax = algo.TimeValues.Max();  // 전체 TimeValues의 최대값
            int binSize = 10;
            var bins = new List<dynamic>();
            for (int start = 0; start < (int)timeMax; start += binSize)
            {
                bins.Add(new { binStart = start, count = 0 });
            }

            // 6) 각 VCB의 사용 기간을 해당 bin에 할당
            foreach (double usedYear in usageYears)
            {
                int index = (int)(usedYear / binSize);
                if (index >= bins.Count) index = bins.Count - 1;
                var oldItem = bins[index];
                bins[index] = new { binStart = oldItem.binStart, count = (int)oldItem.count + 1 };
            }

            // 7) 반환 데이터에 TimeMax 추가
            var result = new
            {
                B3 = b3,
                BinSize = binSize,
                TimeMax = timeMax,
                Histogram = bins
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetAlgorithmData(string equipmentType = "VCB")
        {
            // EquipmentWeibull 테이블에서 해당 equipmentType(예: "VCB") 데이터 가져오기
            var eqList = _weibullRepo.GetAll()
                                     .Where(eq => eq.EquipmentName.ToUpper().Contains(equipmentType.ToUpper()))
                                     .ToList();
            if (eqList.Count == 0)
            {
                return Json(new { error = "해당 장비가 없습니다." }, JsonRequestBehavior.AllowGet);
            }

            var resultList = new List<object>();

            foreach (var eq in eqList)
            {
                if (!eq.ShapeParam.HasValue && !eq.FailureRate.HasValue)
                    continue;

                var algo = new LaAlgorithm();
                if (eq.ShapeParam.HasValue && eq.ScaleParam.HasValue)
                {
                    algo.SetWeibull(eq.ShapeParam.Value, eq.ScaleParam.Value, 10);
                }
                else if (eq.FailureRate.HasValue)
                {
                    algo.SetFailureRate(eq.FailureRate.Value);
                }

                int length = algo.TimeValues.Length;
                var reliabilitySeries = new List<object>();
                var hazardSeries = new List<object>();
                var pdfSeries = new List<object>(); // 확률 밀도 함수 데이터를 담을 리스트

                for (int i = 0; i < length; i++)
                {
                    reliabilitySeries.Add(new
                    {
                        time = algo.TimeValues[i],
                        value = algo.Reliability[i]
                    });
                    hazardSeries.Add(new
                    {
                        time = algo.TimeValues[i],
                        value = algo.HazardNormalized[i]
                    });
                    pdfSeries.Add(new
                    {
                        time = algo.TimeValues[i],
                        value = algo.PdfNormalized[i]
                    });
                }

                resultList.Add(new
                {
                    EquipmentName = eq.EquipmentName,
                    B3Life = algo.B3Life,
                    ReliabilitySeries = reliabilitySeries,
                    HazardSeries = hazardSeries,
                    PdfNormalized = pdfSeries  // 추가된 확률 밀도 함수 데이터
                });
            }

            return Json(resultList, JsonRequestBehavior.AllowGet);
        }
    }
}
