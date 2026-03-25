
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public class EquipmentAlgorithmController : Controller
    {
        private readonly EquipmentWeibullRepository _weibullRepo = new EquipmentWeibullRepository();

        // 기존 5대
        private readonly VCBBasicInfoRepository _vcbRepo = new VCBBasicInfoRepository();
        private readonly DCCBBasicInfoRepository _dccbRepo = new DCCBBasicInfoRepository();
        private readonly DCCABLEBasicInfoRepository _dccableRepo = new DCCABLEBasicInfoRepository();
        private readonly ITRBasicInfoRepository _itrRepo = new ITRBasicInfoRepository();
        private readonly SUBMODULEBasicInfoRepository _submoduleRepo = new SUBMODULEBasicInfoRepository();

        // 추가 15대
        private readonly SABasicInfoRepository _saRepo = new SABasicInfoRepository();
        private readonly DCCTBasicInfoRepository _dcctRepo = new DCCTBasicInfoRepository();
        private readonly ARMREACTORBasicInfoRepository _armReactorRepo = new ARMREACTORBasicInfoRepository();
        private readonly WALLBUSHINGBasicInfoRepository _wallBushingRepo = new WALLBUSHINGBasicInfoRepository();
        private readonly PTBasicInfoRepository _ptRepo = new PTBasicInfoRepository();
        private readonly CTBasicInfoRepository _ctRepo = new CTBasicInfoRepository();
        private readonly LABasicInfoRepository _laRepo = new LABasicInfoRepository();
        private readonly DSBasicInfoRepository _dsRepo = new DSBasicInfoRepository();
        private readonly TANKBasicInfoRepository _tankRepo = new TANKBasicInfoRepository();
        private readonly HEATEXCHANGERBasicInfoRepository _heatExchangerRepo = new HEATEXCHANGERBasicInfoRepository();
        private readonly BYPASSVALVEBasicInfoRepository _bypassValveRepo = new BYPASSVALVEBasicInfoRepository();
        private readonly PUMPBasicInfoRepository _pumpRepo = new PUMPBasicInfoRepository();
        private readonly ZIGZAGTRBasicInfoRepository _zigZagTRRepo = new ZIGZAGTRBasicInfoRepository();
        private readonly ESBasicInfoRepository _esRepo = new ESBasicInfoRepository();
        private readonly NGRBasicInfoRepository _ngrRepo = new NGRBasicInfoRepository();

        private readonly RiskmatrixRepository _riskmatrixRepo = new RiskmatrixRepository();

        private (List<dynamic> basicList, List<double> usageYears)
        GetBasicInfoAndUsage(string equipmentType)
        {
            List<dynamic> basicList;
            switch (equipmentType.Trim().ToUpper())
            {

                // 5대 주요설비
                case "VCB":
                    _vcbRepo.GetAllVCBBasicInfoRepo(out List<VCBBasicInfo> vcb);
                    basicList = vcb.Cast<dynamic>().ToList();
                    break;
                case "ITR":
                    _itrRepo.GetAllITRBasicInfoRepo(out List<ITRBasicInfo> itr);
                    basicList = itr.Cast<dynamic>().ToList();
                    break;
                case "DCCB":
                    _dccbRepo.GetAllDCCBBasicInfoRepo(out List<DCCBBasicInfo> dccb);
                    basicList = dccb.Cast<dynamic>().ToList();
                    break;
                case "DCCABLE":
                    _dccableRepo.GetAllDCCABLEBasicInfoRepo(out List<DCCABLEBasicInfo> dccable);
                    basicList = dccable.Cast<dynamic>().ToList();
                    break;
                case "SUBMODULE":
                    _submoduleRepo.GetAllSUBMODULEBasicInfoRepo(out List<SUBMODULEBasicInfo> submodule);
                    basicList = submodule.Cast<dynamic>().ToList();
                    break;

                // 15대 설비 
                case "SA":
                    _saRepo.GetAllSABasicInfoRepo(out List<SABasicInfo> sa);
                    basicList = sa.Cast<dynamic>().ToList();
                    break;
                case "DCCT":
                    _dcctRepo.GetAllDCCTBasicInfoRepo(out List<DCCTBasicInfo> dcct);
                    basicList = dcct.Cast<dynamic>().ToList();
                    break;
                case "ARMREACTOR":
                    _armReactorRepo.GetAllARMREACTORBasicInfoRepo(out List<ARMREACTORBasicInfo> armReactor);
                    basicList = armReactor.Cast<dynamic>().ToList();
                    break;
                case "WALLBUSHING":
                    _wallBushingRepo.GetAllWALLBUSHINGBasicInfoRepo(out List<WALLBUSHINGBasicInfo> wallbushing);
                    basicList = wallbushing.Cast<dynamic>().ToList();
                    break;
                case "PT":
                    _ptRepo.GetAllPTBasicInfoRepo(out List<PTBasicInfo> pt);
                    basicList = pt.Cast<dynamic>().ToList();
                    break;

                case "CT":
                    _ctRepo.GetAllCTBasicInfoRepo(out List<CTBasicInfo> ct);
                    basicList = ct.Cast<dynamic>().ToList();
                    break;
                case "LA":
                    _laRepo.GetAllLABasicInfoRepo(out List<LABasicInfo> la);
                    basicList = la.Cast<dynamic>().ToList();
                    break;
                case "DS":
                    _dsRepo.GetAllDSBasicInfoRepo(out List<DSBasicInfo> ds);
                    basicList = ds.Cast<dynamic>().ToList();
                    break;
                case "TANK":
                    _tankRepo.GetAllTANKBasicInfoRepo(out List<TANKBasicInfo> tank);
                    basicList = tank.Cast<dynamic>().ToList();
                    break;
                case "HEATEXCHANGER":
                    _heatExchangerRepo.GetAllHEATEXCHANGERBasicInfoRepo(out List<HEATEXCHANGERBasicInfo> heatExchanger);
                    basicList = heatExchanger.Cast<dynamic>().ToList();
                    break;

                case "BYPASSVALVE":
                    _bypassValveRepo.GetAllBYPASSVALVEBasicInfoRepo(out List<BYPASSVALVEBasicInfo> bypassValve);
                    basicList = bypassValve.Cast<dynamic>().ToList();
                    break;
                case "PUMP":
                    _pumpRepo.GetAllPUMPBasicInfoRepo(out List<PUMPBasicInfo> pump);
                    basicList = pump.Cast<dynamic>().ToList();
                    break;
                case "ZIGZAGTR":
                    _zigZagTRRepo.GetAllZIGZAGTRBasicInfoRepo(out List<ZIGZAGTRBasicInfo> zigZagTR);
                    basicList = zigZagTR.Cast<dynamic>().ToList();
                    break;
                case "ES":
                    _esRepo.GetAllESBasicInfoRepo(out List<ESBasicInfo> es);
                    basicList = es.Cast<dynamic>().ToList();
                    break;
                case "NGR":
                    _ngrRepo.GetAllNGRBasicInfoRepo(out List<NGRBasicInfo> ngr);                    
                    basicList = ngr.Cast<dynamic>().ToList();
                    break;

                    // 예외처리
                default:
                    basicList = new List<dynamic>();
                    break;
            }

            var usageYears = basicList
                .Select(b => b.Operating_Date is DateTime od
                                ? (double)(DateTime.Now.Year - od.Year)
                                : 0d)
                .ToList();

            return (basicList, usageYears);
        }

        /// <summary>
        /// B3 수명 기반 히스토그램 데이터
        /// </summary>
        [HttpGet]
        public ActionResult GetB3HistogramEquipment(string equipmentType = "VCB")
        {
            // 1) Weibull 데이터 필터링
            var eqList = _weibullRepo.GetAll()
                .Where(eq => eq.EquipmentName
                               .ToUpper()
                               .Contains(equipmentType.ToUpper()))
                .ToList();
            if (!eqList.Any())
                return Json(new { error = "해당 장비의 Weibull 데이터가 없습니다." },
                            JsonRequestBehavior.AllowGet);

            // 2) B3 수명 계산
            var first = eqList
                .FirstOrDefault(eq => eq.ShapeParam.HasValue && eq.ScaleParam.HasValue)
                ?? eqList.FirstOrDefault(eq => eq.FailureRate.HasValue);
            if (first == null)
                return Json(new { error = "Weibull 또는 고장률 데이터가 없습니다." },
                            JsonRequestBehavior.AllowGet);

            var algo = new LaAlgorithm();
            if (first.ShapeParam.HasValue && first.ScaleParam.HasValue)
                algo.SetWeibull((double)first.ShapeParam.Value,
                                (double)first.ScaleParam.Value,
                                10);
            else
                algo.SetFailureRate((double)first.FailureRate.Value);

            double b3 = algo.B3Life;
            double timeMax = algo.TimeValues.Max();
            int binSize = 10;

            // 3) 기본정보 + 사용기간 가져오기
            var (basicList, usageYears) = GetBasicInfoAndUsage(equipmentType);

            // 4) bin 초기화
            var binCount = (int)Math.Ceiling(timeMax / binSize);
            var bins = Enumerable.Range(0, binCount)
                .Select(i => new { binStart = i * binSize, count = 0 })
                .ToList();

            // 5) 카운팅
            foreach (var years in usageYears)
            {
                var idx = Math.Min((int)(years / binSize), bins.Count - 1);
                bins[idx] = new { bins[idx].binStart, count = bins[idx].count + 1 };
            }

            // 6) 결과 반환 (usageYears도 함께 반환하여 차트 위에 추가선 그리기 용이)
            return Json(new
            {
                B3 = b3,
                BinSize = binSize,
                TimeMax = timeMax,
                Histogram = bins,
                Usage = usageYears
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 신뢰도/고장률/PDF 시리즈 + 사용기간 데이터를 함께 반환
        /// </summary>
        [HttpGet]
        public ActionResult GetAlgorithmData(string equipmentType = "VCB")
        {
            // 1) Weibull 데이터 필터링
            var eqList = _weibullRepo.GetAll()
                .Where(eq => eq.EquipmentName
                               .ToUpper()
                               .Contains(equipmentType.ToUpper()))
                .ToList();
            if (!eqList.Any())
                return Json(new { error = "해당 장비의 Weibull 데이터가 없습니다." },
                            JsonRequestBehavior.AllowGet);

            // 2) 기본정보 + 사용기간
            var (_, usageYears) = GetBasicInfoAndUsage(equipmentType);

            var resultList = new List<object>();

            foreach (var eq in eqList)
            {
                // 2a) 알고리즘 초기화
                var algo = new LaAlgorithm();
                if (eq.ShapeParam.HasValue && eq.ScaleParam.HasValue)
                    algo.SetWeibull((double)eq.ShapeParam.Value,
                                    (double)eq.ScaleParam.Value,
                                    10);
                else if (eq.FailureRate.HasValue)
                    algo.SetFailureRate((double)eq.FailureRate.Value);
                else
                    continue;

                double timeMax = algo.TimeValues.Max();
                int length = algo.TimeValues.Length;
                var reliabilitySeries = new List<object>();
                var hazardSeries = new List<object>();
                var pdfSeries = new List<object>();

                // 2b) 시리즈 생성
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
                    TimeMax = timeMax,
                    UsagePeriod = usageYears,
                    ReliabilitySeries = reliabilitySeries,
                    HazardSeries = hazardSeries,
                    PdfNormalized = pdfSeries
                });
            }

            return Json(resultList, JsonRequestBehavior.AllowGet);
        }

        //그룹페이지 "차년도 유지보수 대상" 표기 
        [HttpGet]
        public JsonResult GetGroupStrategy(string equipmentType = "VCB")
        {
            // 1) B3 계산
            var list = _weibullRepo.GetAll()
                                   .Where(x => x.EquipmentName.ToUpper().Contains(equipmentType.ToUpper()))
                                   .ToList();
            if (!list.Any())
                return Json(new { error = "Weibull 데이터 없음" }, JsonRequestBehavior.AllowGet);

            var first = list.FirstOrDefault(x => x.ShapeParam.HasValue && x.ScaleParam.HasValue)
                        ?? list.FirstOrDefault(x => x.FailureRate.HasValue);
            var algo = new LaAlgorithm();
            if (first.ShapeParam.HasValue && first.ScaleParam.HasValue)
                algo.SetWeibull(first.ShapeParam.Value, first.ScaleParam.Value, 10);
            else
                algo.SetFailureRate(first.FailureRate.Value);

            double b3 = algo.B3Life;

            // 2) Riskmatrix 최신 HI 가져오기 (equipmentType 으로 필터링)
            var hiList = _riskmatrixRepo                .GetLatestRiskPoints()
                .Where(r => r.Code.StartsWith(equipmentType, StringComparison.OrdinalIgnoreCase))
                .Select(r => int.TryParse(r.HI, out var v) ? v : 0)
                .ToList();
            if (!hiList.Any())
                return Json(new { error = "Riskmatrix HI 데이터 없음" }, JsonRequestBehavior.AllowGet);

            // 3) 각 설비의 등급별 수명 매핑
            Func<int, double> map = hi =>
            {
                switch (hi)
                {
                    case 5: return 1;                    // POF 5등급 -> 1년
                    case 4: return b3 - 5;               // POF 4등급 -> B3Life - 5
                    case 3: return b3;                   // POF 3등급 -> B3Life
                    case 2: return b3 + 1;               // POF 2등급 -> B3Life + 1
                    case 1: return b3 + 2;               // POF 1등급 -> B3Life + 2
                    default: return 0;
                }
            };

            var life = hiList.Select(map).ToList();
            double avg = Math.Round(life.Average(), 1);
            int worst = hiList.Max();

            // 4) 그룹 전체의 전략 결정 (최저 등급을 기준으로 전략 설정)
            string avgHtml, msg;
            if (worst >= 4)
            {
                avgHtml = $"<strong style=\"color:red;\">{avg}</strong>";
                msg = $"긴급 유지보수 권장: 상태가 심각하게 악화된 것으로 판단됩니다. 평균 잔여수명은 {avgHtml}년입니다.";
            }
            else if (worst == 3)
            {
                avgHtml = $"<strong style=\"color:gold;\">{avg}</strong>";
                msg = $"유지보수 계획 권장: 상태가 악화되고 있어 계획적 유지보수가 필요합니다. 평균 잔여수명은 {avgHtml}년입니다.";
            }
            else
            {
                avgHtml = $"<strong style=\"color:green;\">{avg}</strong>";
                msg = $"정상 운영 가능: 현재 상태가 양호하여 유지보수가 필요하지 않습니다. 평균 잔여수명은 {avgHtml}년입니다.";

            }

            return Json(new
            {
                AverageLife = avg,
                StrategyMessage = msg
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetEquipmentStrategy(string code)
        {
            // 1) 해당 설비 HI 하나만 조회
            var latestRisk = _riskmatrixRepo.GetLatestRiskMatrixByCode(code);
            if (latestRisk == null || !int.TryParse(latestRisk.HI, out var hi))
                return Json(new { error = "HI 데이터 없음" }, JsonRequestBehavior.AllowGet);

            // 2) Weibull → B3 계산 (VCB 장비 기준)
            var first = _weibullRepo.GetAll()
                .FirstOrDefault(x => x.EquipmentName.ToUpper().Contains("VCB") &&
                                     (x.ShapeParam.HasValue && x.ScaleParam.HasValue || x.FailureRate.HasValue));
            if (first == null)
                return Json(new { error = "Weibull 데이터 없음" }, JsonRequestBehavior.AllowGet);

            var algo = new LaAlgorithm();
            if (first.ShapeParam.HasValue && first.ScaleParam.HasValue)
                algo.SetWeibull(first.ShapeParam.Value, first.ScaleParam.Value, 10);
            else
                algo.SetFailureRate(first.FailureRate.Value);

            double b3 = algo.B3Life;

            // 3) POF 등급 → 남은수명 매핑 
            double remaining;
            switch (hi)
            {
                case 5:
                    remaining = 1;
                    break;
                case 4:
                    remaining = b3 - 5;
                    break;
                case 3:
                    remaining = b3;
                    break;
                case 2:
                    remaining = b3 + 1;
                    break;
                case 1:
                    remaining = b3 + 2;
                    break;
                default:
                    remaining = 0;
                    break;
            }

            // 4) 개별 전략 메시지
            string avgHtml, msg;
            if (hi >= 4)
            {
                avgHtml = $"<strong style=\"color:red;\">{remaining}</strong>";
                msg = $"긴급 유지보수 권장: 상태가 심각하게 악화된 것으로 판단됩니다. 잔여수명은 {avgHtml}년입니다.";
            }
            else if (hi == 3)
            {
                avgHtml = $"<strong style=\"color:gold;\">{remaining}</strong>";
                msg = $"유지보수 계획 권장: 상태가 악화되고 있어 계획적 유지보수가 필요합니다. 잔여수명은 {avgHtml}년입니다.";
            }
            else
            {
                avgHtml = $"<strong style=\"color:green;\">{remaining}</strong>";
                msg = $"정상 운영 가능: 현재 상태가 양호하여 유지보수가 필요하지 않습니다. 잔여수명은 {avgHtml}년입니다.";
            }

            return Json(new
            {
                HI = hi,
                RemainingLife = remaining,
                StrategyMessage = msg
            }, JsonRequestBehavior.AllowGet);
        }



    }
}
