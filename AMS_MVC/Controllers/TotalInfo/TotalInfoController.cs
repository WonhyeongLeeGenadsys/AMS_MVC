
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class TotalInfoController : Controller
    {
        private RiskmatrixRepository _riskRepo = new RiskmatrixRepository();

        // GET: TotalInfo/Index
        public ActionResult Index()
        {
            ViewBag.MenuType = "TotalInfo";
            ViewBag.Title = "종합정보";
            return View();
        }

        /// <summary>
        /// 전체 5대장비, AC(VCB, ITR), DC(DCCB, DCCABLE, SUBMODULE) 3가지 분류!
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetRiskmatrixData(string prefix)
        {
            string[] codePrefixes;
            if (string.IsNullOrEmpty(prefix))
                codePrefixes = new[] { "VCB", "ITR", "DCCB", "DCCABLE", "SUBMODULE" };
            else if (prefix == "AC")
                codePrefixes = new[] { "VCB", "ITR" };
            else if (prefix == "DC")
                codePrefixes = new[] { "DCCB", "DCCABLE", "SUBMODULE" };
            else
                codePrefixes = new[] { prefix };

            var hiData = _riskRepo.GetAllHIByCode(codePrefixes);
            return Json(hiData);
        }

        [HttpPost]
        public JsonResult GetHIList(string prefix)
        {
            // 각 장비의 HI 값(정수)만 순서대로 리스트화
            var list = _riskRepo.GetHIList(prefix);
            return Json(list);
        }
        [HttpPost]
        public JsonResult GetRiskMatrixPofCof()
        {
            try
            {
                var repository = new RiskmatrixRepository();
                var pofCof = repository.GetRiskMatrixPofCof();
                return Json(pofCof);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetPriorityInfo()
        {
            try
            {
                var priorityRepo = new PriorityInfoRepository();
                var priorityData = priorityRepo.GetPriorityInfo();

                var formattedData = priorityData.Select(item =>
                {
                    return new
                    {
                    Priority = (int?)item.Priority,
                    item.Sort,
                    item.Code,
                    item.Serial_No,
                    item.Name,
                    item.ProductName,
                    Install_Date = item.Install_Date.ToString("yy.MM.dd"),
                    Operating_Date = item.Operating_Date.ToString("yy.MM.dd"),
                    item.UsagePeriod,
                    item.Price,
                    item.Rated_V,
                    item.Rated_A,
                    item.Make_Company,
                    item.Writer,
                    // 종합정보 기본값은 CoF 등록 시 RISKMATRIX에 저장된 목포대 값이다.
                    item.CoF,
                    item.PoF,
                    item.HI
                    };
                }).ToList();

                return Json(formattedData);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetMonthlyMaintenanceData()
        {
            try
            {
                var repo = new MaintenanceRepository();
                var data = repo.GetMonthlyMaintenanceCounts();


                return Json(data);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("TotalInfoController", $"GetMonthlyMaintenanceData Error: {ex.Message}");
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetGojangList()
        {
            try
            {
                var gojangRepo = new GojangRepository();
                var gojangData = gojangRepo.GetGojangAll();

                return Json(gojangData);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("TotalInfoController", $"GetGojangList Error: {ex.Message}");
                return Json(new { error = "데이터를 가져오는 중 오류 발생", details = ex.Message });
            }
        }

        [HttpGet]
        public ActionResult SearchByDateRange(string dateType, DateTime startDate, DateTime endDate)
        {
            if (string.IsNullOrEmpty(dateType))
            {
                return Json(new { error = "dateType이 비어있습니다." }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                // Repository 호출
                var data = _riskRepo.GetDevicesByDateRange(dateType, startDate, endDate);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = "검색 중 오류 발생", details = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// riskmatrix 값 전체, AC, DC 구별
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetRiskMapPoints(string prefix = "all")
        {
            prefix = (prefix ?? "").ToLower();
            string dmPrefix = prefix == "ac" ? "AC" : prefix == "dc" ? "DC" : string.Empty;
            IEnumerable<DmDecisionInfo> filtered = new DmDecisionService().GetDecisions(dmPrefix);
            if (new[] { "vcb", "itr", "dccb", "dccable", "submodule" }.Contains(prefix))
            {
                filtered = filtered.Where(r => r.Code != null
                    && r.Code.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
            }

            var points = filtered.Select(r => new {
                x = Math.Round(r.CoF, 2),
                y = Math.Round(r.PoFRatio * 100d, 4),
                name = r.Code,
                hi = Math.Round(r.HI, 2),
                group = r.EquipmentKey,
                risk = Math.Round(r.Risk, 2),
                bcr = Math.Round(r.Bcr, 3),
                decision = r.Decision
            });

            return Json(points, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetHIPofCofHistory(string prefix)
        {
            var list = _riskRepo.GetRiskMatrixHistory(prefix ?? "")
                .OrderBy(r => r.LastTime)
                .Select(r => new
                {
                    code = r.Code,
                    time = r.LastTime != DateTime.MinValue ? r.LastTime.ToString("o") : "N/A", 
                    hi = int.TryParse(r.HI, out int hiValue) ? hiValue : 0,
                    pof = r.Pof,
                    cof = r.Cof
                })
                .ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 최신 HI/PoF와 설비별 Weibull 정보를 숭실대 원본 DM의 CoF/NPV 공식에 연결해
        /// 유지보수 의사결정 결과를 반환한다. DB 저장 없이 조회 시점에 계산한다.
        /// </summary>
        [HttpGet]
        public JsonResult GetDMDecisionInfo(string prefix = "")
        {
            try
            {
                var ordered = new DmDecisionService().GetDecisions(prefix);

                var rows = ordered.Select(item => new
                {
                    item.Priority,
                    item.Sort,
                    item.Code,
                    Serial_No = item.SerialNo,
                    item.Name,
                    item.ProductName,
                    item.AssetType,
                    item.UsageYears,
                    item.HI,
                    PoF = Math.Round(item.PoFRatio * 100d, 2),
                    ReplacementCost = Math.Round(item.ReplacementCost, 0),
                    CoF = Math.Round(item.CoF, 2),
                    DmCofKrw = Math.Round(item.DmCofKrw, 0),
                    CofTotalKrw = Math.Round(item.CofTotalKrw, 0),
                    RawCofFinancial = Math.Round(item.RawCofFinancial, 0),
                    RawCofReliability = Math.Round(item.RawCofReliability, 0),
                    RawCofSafety = Math.Round(item.RawCofSafety, 0),
                    RawCofEnvironmental = Math.Round(item.RawCofEnvironmental, 0),
                    CofFinancial = Math.Round(item.CofFinancial, 0),
                    CofReliability = Math.Round(item.CofReliability, 0),
                    CofSafety = Math.Round(item.CofSafety, 0),
                    CofEnvironmental = Math.Round(item.CofEnvironmental, 0),
                    CofCens = Math.Round(item.CofCens, 0),
                    CofSaidiPenalty = Math.Round(item.CofSaidiPenalty, 0),
                    CofSaifiPenalty = Math.Round(item.CofSaifiPenalty, 0),
                    SaidiContribution = Math.Round(item.SaidiContribution, 6),
                    SaifiContribution = Math.Round(item.SaifiContribution, 6),
                    CustomersAffected = Math.Round(item.CustomersAffected, 0),
                    Risk = Math.Round(item.Risk, 2),
                    NPV = Math.Round(item.NpvValue, 0),
                    NpvBenefits = Math.Round(item.NpvBenefits, 0),
                    NpvCosts = Math.Round(item.NpvCosts, 0),
                    BCR = Math.Round(item.Bcr, 3),
                    ROI = Math.Round(item.RoiPct, 2),
                    RiskMitigation = Math.Round(item.RiskMitigation, 0),
                    AnnualMaintenanceSaving = Math.Round(item.AnnualMaintenanceSaving, 0),
                    AnnualEfficiencyBenefit = Math.Round(item.AnnualEfficiencyBenefit, 0),
                    AnnualBenefits = Math.Round(item.AnnualBenefits, 0),
                    TotalBenefits = Math.Round(item.TotalBenefits, 0),
                    DiscountedAnnualBenefits = Math.Round(item.DiscountedAnnualBenefits, 0),
                    ExtendedLifetimeValue = Math.Round(item.ExtendedLifetimeValue, 0),
                    InstallationCost = Math.Round(item.InstallationCost, 0),
                    DisposalCost = Math.Round(item.DisposalCost, 0),
                    TotalCosts = Math.Round(item.TotalCosts, 0),
                    DiscountRate = Math.Round(item.DiscountRatePct, 2),
                    InflationRate = Math.Round(item.InflationRatePct, 2),
                    item.EvaluationPeriodYears,
                    RUL = item.RULYears.HasValue
                        ? (double?)Math.Round(item.RULYears.Value, 2)
                        : null,
                    // E9 상태보정 RUL (진단 상태 반영). RUL과 나란히 표시한다.
                    RulStateCorrected = item.RULStateCorrectedYears.HasValue
                        ? (double?)Math.Round(item.RULStateCorrectedYears.Value, 2)
                        : null,
                    DiagnosticPof = Math.Round(item.DiagnosticPofPct, 2),
                    item.Decision,
                    item.Urgency,
                    item.RecommendedAction,
                    Criticality = Math.Round(item.Criticality, 2),
                    TopsisScore = Math.Round(item.TopsisScore, 4),
                    item.TopsisRank,
                    AhpConsistencyRatio = Math.Round(item.AhpConsistencyRatio, 4),
                    DMScore = Math.Round(item.DMScore, 4)
                }).ToList();

                var summary = new
                {
                    Total = ordered.Count,
                    ReplaceImmediate = ordered.Count(x => x.Severity == 5),
                    UrgentMaintenance = ordered.Count(x => x.Severity == 4),
                    PreventiveMaintenance = ordered.Count(x => x.Severity == 3),
                    ScheduledInspection = ordered.Count(x => x.Severity == 2),
                    ContinueMonitoring = ordered.Count(x => x.Severity == 1),
                    DataRequired = ordered.Count(x => x.Severity == 0),
                    TopCode = ordered.Any() ? ordered[0].Code : "",
                    TopName = ordered.Any() ? ordered[0].ProductName : "",
                    TopDecision = ordered.Any() ? ordered[0].Decision : "",
                    TopAction = ordered.Any() ? ordered[0].RecommendedAction : "",
                    TopNPV = ordered.Any() ? Math.Round(ordered[0].NpvValue, 0) : 0d,
                    TopROI = ordered.Any() ? Math.Round(ordered[0].RoiPct, 2) : 0d,
                    AverageHI = ordered.Any() ? Math.Round(ordered.Average(x => x.HI), 2) : 0d,
                    AveragePoF = ordered.Any() ? Math.Round(ordered.Average(x => x.PoFRatio) * 100d, 2) : 0d,
                    AverageBCR = ordered.Any() ? Math.Round(ordered.Average(x => x.Bcr), 2) : 0d,
                    TotalRisk = Math.Round(ordered.Sum(x => x.Risk), 0),
                    TotalNPV = Math.Round(ordered.Sum(x => x.NpvValue), 0),
                    SystemSAIDI = Math.Round(ordered.Sum(x => x.SaidiContribution), 4),
                    SystemSAIFI = Math.Round(ordered.Sum(x => x.SaifiContribution), 4),
                    SAIDITarget = 200d,
                    SAIFITarget = 0.50d,
                    AHPConsistencyRatio = ordered.Any()
                        ? Math.Round(ordered[0].AhpConsistencyRatio, 4)
                        : 0d
                };

                return Json(new { success = true, rows, summary }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("TotalInfoController", $"GetDMDecisionInfo Error: {ex.Message}");
                return Json(new
                {
                    success = false,
                    error = "의사결정 데이터를 계산하는 중 오류가 발생했습니다.",
                    details = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        private static DmCandidate BuildDmCandidate(
            PriorityInfo item,
            IEnumerable<EquipmentWeibull> equipmentWeibulls)
        {
            double pofRaw;
            int hi;
            bool hasPof = TryParseDouble(item.PoF, out pofRaw);
            bool hasHi = int.TryParse(item.HI, out hi) && hi >= 1 && hi <= 5;

            double pofRatio = hasPof
                ? Clamp(pofRaw > 1d ? pofRaw / 100d : pofRaw, 0d, 1d)
                : 0d;
            string equipmentKey = GetEquipmentKey(item.Code, item.Name);
            var lifeModel = equipmentWeibulls.FirstOrDefault(x =>
                string.Equals(x.EquipmentName, equipmentKey, StringComparison.OrdinalIgnoreCase));

            double? rulYears = CalculateRulYears(lifeModel, hasPof ? (double?)pofRatio : null, item.UsagePeriod);
            var economicResult = new OriginalDmCalculator().Calculate(
                equipmentKey,
                pofRatio,
                item.UsagePeriod);

            var candidate = new DmCandidate
            {
                Sort = item.Sort,
                Code = item.Code,
                SerialNo = item.Serial_No,
                Name = item.Name,
                ProductName = string.IsNullOrWhiteSpace(item.ProductName) ? item.Name : item.ProductName,
                AssetType = economicResult.AssetType,
                HI = hasHi ? hi : 0,
                PoFRatio = pofRatio,
                ReplacementCost = economicResult.ReplacementCost,
                CoF = economicResult.CofTotal,
                Risk = hasPof ? economicResult.Risk : 0d,
                NpvValue = economicResult.NpvValue,
                RoiPct = economicResult.RoiPct,
                RULYears = rulYears
            };

            SetDecision(candidate, hasHi && hasPof);
            return candidate;
        }

        private static void SetDecision(DmCandidate candidate, bool hasRequiredRiskData)
        {
            if (!hasRequiredRiskData)
            {
                candidate.Severity = 0;
                candidate.Decision = "데이터 확인 필요";
                candidate.Urgency = "확인 필요";
                candidate.RecommendedAction = "HI·PoF 산정 필요";
                return;
            }

            bool hasRul = candidate.RULYears.HasValue;
            double rul = candidate.RULYears.GetValueOrDefault(double.MaxValue);

            if ((candidate.PoFRatio > 0.8d && candidate.CoF > 2000000000d)
                || candidate.HI >= 5
                || (hasRul && rul < 0.5d))
            {
                candidate.Severity = 5;
                candidate.Decision = "즉시 교체";
                candidate.Urgency = "매우 높음";
                candidate.RecommendedAction = "즉시";
            }
            else if ((candidate.PoFRatio > 0.6d && candidate.CoF > 1000000000d)
                || candidate.HI >= 4
                || (hasRul && rul < 1d))
            {
                candidate.Severity = 4;
                candidate.Decision = "긴급 유지보수";
                candidate.Urgency = "높음";
                candidate.RecommendedAction = "1~3개월";
            }
            else if ((candidate.PoFRatio > 0.4d && candidate.CoF > 500000000d)
                || candidate.HI >= 3
                || (hasRul && rul < 2d))
            {
                candidate.Severity = 3;
                candidate.Decision = "예방 유지보수";
                candidate.Urgency = "보통";
                candidate.RecommendedAction = "6~12개월";
            }
            else if (candidate.PoFRatio > 0.2d
                || candidate.HI >= 2
                || (hasRul && rul < 3d))
            {
                candidate.Severity = 2;
                candidate.Decision = "정기점검";
                candidate.Urgency = "낮음";
                candidate.RecommendedAction = "12~24개월";
            }
            else
            {
                candidate.Severity = 1;
                candidate.Decision = "계속 감시";
                candidate.Urgency = "관찰";
                candidate.RecommendedAction = "24~36개월";
            }

            // 원본 Value Framework: 즉시 교체라도 ROI가 10% 미만이면 한 단계 하향한다.
            if (candidate.RoiPct < 10d && candidate.Severity == 5)
            {
                candidate.Severity = 4;
                candidate.Decision = "긴급 유지보수";
                candidate.Urgency = "높음";
                candidate.RecommendedAction = "1~3개월";
            }
        }

        private static double CalculateDmScore(DmCandidate candidate, double maxRisk, double maxNpv)
        {
            if (candidate.Severity == 0)
            {
                return 0d;
            }

            double riskNorm = maxRisk > 0d ? candidate.Risk / maxRisk : 0d;
            double npvNorm = maxNpv > 0d ? candidate.NpvValue / maxNpv : 0d;
            double urgencyNorm;
            switch (candidate.Severity)
            {
                case 5:
                    urgencyNorm = 1.0d;
                    break;
                case 4:
                    urgencyNorm = 0.8d;
                    break;
                case 3:
                    urgencyNorm = 0.5d;
                    break;
                case 2:
                    urgencyNorm = 0.3d;
                    break;
                default:
                    urgencyNorm = 0.1d;
                    break;
            }

            // 원본: Risk 50% + NPV 30% + Urgency 20% (0~1 정규화 점수)
            return (riskNorm * 0.5d) + (npvNorm * 0.3d) + (urgencyNorm * 0.2d);
        }

        private static double? CalculateRulYears(EquipmentWeibull lifeModel, double? currentPof, int usageYears)
        {
            if (lifeModel == null)
            {
                return null;
            }

            const double targetPof = 0.95d;
            double currentAge;
            double targetAge;

            if (lifeModel.ShapeParam.HasValue && lifeModel.ShapeParam.Value > 0d
                && lifeModel.ScaleParam.HasValue && lifeModel.ScaleParam.Value > 0d)
            {
                double shape = lifeModel.ShapeParam.Value;
                double scale = lifeModel.ScaleParam.Value;
                targetAge = scale * Math.Pow(-Math.Log(1d - targetPof), 1d / shape);
                currentAge = currentPof.HasValue
                    ? scale * Math.Pow(-Math.Log(Math.Max(1d - currentPof.Value, 0.000001d)), 1d / shape)
                    : Math.Max(0d, usageYears);
            }
            else if (lifeModel.FailureRate.HasValue && lifeModel.FailureRate.Value > 0d)
            {
                double failureRate = lifeModel.FailureRate.Value;
                targetAge = -Math.Log(1d - targetPof) / failureRate;
                currentAge = currentPof.HasValue
                    ? -Math.Log(Math.Max(1d - currentPof.Value, 0.000001d)) / failureRate
                    : Math.Max(0d, usageYears);
            }
            else
            {
                return null;
            }

            return Math.Max(0d, targetAge - currentAge);
        }

        private static string GetEquipmentKey(string code, string name)
        {
            string normalizedCode = (code ?? "").ToUpperInvariant();
            if (normalizedCode.StartsWith("DCCABLE")) return "DCCABLE";
            if (normalizedCode.StartsWith("SUBMODULE")) return "SUBMODULE";
            if (normalizedCode.StartsWith("DCCB")) return "DCCB";
            if (normalizedCode.StartsWith("ITR")) return "ITR";
            if (normalizedCode.StartsWith("VCB")) return "VCB";

            string normalizedName = (name ?? "")
                .Replace(" ", "")
                .Replace("-", "")
                .ToUpperInvariant();
            if (normalizedName.Contains("DCCABLE")) return "DCCABLE";
            if (normalizedName.Contains("SUBMODULE")) return "SUBMODULE";
            if (normalizedName.Contains("DCCB")) return "DCCB";
            if (normalizedName.Contains("INTERFACETR") || normalizedName.Contains("ITR")) return "ITR";
            return "VCB";
        }

        private static bool TryParseDouble(string value, out double result)
        {
            return double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result)
                || double.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out result);
        }

        private static double Clamp(double value, double minimum, double maximum)
        {
            return Math.Max(minimum, Math.Min(maximum, value));
        }

        private sealed class DmCandidate
        {
            public string Sort { get; set; }
            public string Code { get; set; }
            public string SerialNo { get; set; }
            public string Name { get; set; }
            public string ProductName { get; set; }
            public string AssetType { get; set; }
            public int HI { get; set; }
            public double PoFRatio { get; set; }
            public double ReplacementCost { get; set; }
            public double CoF { get; set; }
            public double Risk { get; set; }
            public double NpvValue { get; set; }
            public double RoiPct { get; set; }
            public double? RULYears { get; set; }
            public int Severity { get; set; }
            public string Decision { get; set; }
            public string Urgency { get; set; }
            public string RecommendedAction { get; set; }
            public double DMScore { get; set; }
        }

    }
}
