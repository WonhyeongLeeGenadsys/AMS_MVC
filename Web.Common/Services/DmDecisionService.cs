using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Web.Common
{
    /// <summary>
    /// 목포대 RISKMATRIX PoF/CoF → Risk, 숭실대 4D CoF → BCR/NPV,
    /// RUL → AHP-TOPSIS를 한 번에 계산한다.
    /// AC/DC 조회도 전체 fleet 정규화와 순위를 먼저 계산한 뒤 결과만 필터링한다.
    /// </summary>
    public sealed class DmDecisionService
    {
        public List<DmDecisionInfo> GetDecisions(string prefix = "")
        {
            var priorityRepo = new PriorityInfoRepository();
            var calculator = new OriginalDmCalculator();
            var allCandidates = priorityRepo.GetPriorityInfo()
                .Select(item => BuildCandidate(item, calculator))
                .ToList();

            calculator.ApplyFleetCalculation(allCandidates);
            ApplyTopsis(allCandidates);

            string normalizedPrefix = (prefix ?? string.Empty).Trim().ToUpperInvariant();
            IEnumerable<DmDecisionInfo> filtered = allCandidates;
            if (normalizedPrefix == "AC" || normalizedPrefix == "DC")
            {
                filtered = filtered.Where(x => x.Sort == normalizedPrefix);
            }

            return filtered
                .OrderBy(x => x.TopsisRank == 0 ? int.MaxValue : x.TopsisRank)
                .ThenByDescending(x => x.Risk)
                .ToList();
        }

        private static DmDecisionInfo BuildCandidate(
            PriorityInfo item,
            OriginalDmCalculator calculator)
        {
            double hi;
            bool hasHi = TryParseDouble(item.HI, out hi) && hi >= 1d && hi <= 5d;
            string equipmentKey = GetEquipmentKey(item.Code, item.Name);

            double storedPof;
            bool hasStoredPof = TryParseDouble(item.PoF, out storedPof);
            double pofRatio = hasStoredPof
                ? AmsV31Config.Clamp(storedPof > 1d ? storedPof / 100d : storedPof, 0d, 1d)
                : 0d;

            double storedCof;
            bool hasStoredCof = TryParseDouble(item.CoF, out storedCof) && storedCof >= 0d;

            double voltageKv = item.Rated_V > 0f ? item.Rated_V : 35d;
            double currentKa = item.Rated_A > 0f ? item.Rated_A : 25d;
            OriginalDmResult raw = calculator.CalculateRaw(
                equipmentKey,
                pofRatio,
                item.UsagePeriod,
                voltageKv,
                currentKa);

            return new DmDecisionInfo
            {
                Sort = item.Sort,
                Code = item.Code,
                SerialNo = item.Serial_No,
                Name = item.Name,
                ProductName = string.IsNullOrWhiteSpace(item.ProductName) ? item.Name : item.ProductName,
                AssetType = raw.AssetType,
                EquipmentKey = equipmentKey,
                UsageYears = Math.Max(0, item.UsagePeriod),
                HI = hasHi ? hi : 0d,
                PoFRatio = pofRatio,
                ReplacementCost = raw.ReplacementCost,
                CoF = hasStoredCof ? storedCof : 0d,
                CofTotalKrw = raw.CofTotalKrw,
                RawCofFinancial = raw.RawCofFinancial,
                RawCofReliability = raw.RawCofReliability,
                RawCofSafety = raw.RawCofSafety,
                RawCofEnvironmental = raw.RawCofEnvironmental,
                CofCens = raw.CofCens,
                CofSaidiPenalty = raw.CofSaidiPenalty,
                CofSaifiPenalty = raw.CofSaifiPenalty,
                SaidiContribution = raw.SaidiContribution,
                SaifiContribution = raw.SaifiContribution,
                CustomersAffected = raw.CustomersAffected,
                Criticality = raw.Criticality,
                RULYears = AmsV31Config.CalculateRulYears(equipmentKey, item.UsagePeriod),
                // E9 상태보정 RUL.
                // 저장된 PoF는 목포대 RISKMATRIX 원본값이라 등급 경계에서 100%로 굳어 있는 경우가 있어
                // 그대로 쓰면 상태보정 결과가 전부 0년으로 눌린다. 그래서 HI로부터 v3.1.0 진단 PoF를
                // 산출해 상태보정에만 사용한다(기존 PoFRatio·Risk·TOPSIS 계산은 그대로 둔다).
                DiagnosticPofPct = hasHi ? AmsV31Config.CalculateDiagnosticPof(hi, equipmentKey) * 100d : 0d,
                RULStateCorrectedYears = hasHi
                    ? AmsV31Config.CalculateStateCorrectedRulYears(
                        equipmentKey,
                        item.UsagePeriod,
                        AmsV31Config.CalculateDiagnosticPof(hi, equipmentKey))
                    : (double?)null,
                DiscountRatePct = raw.DiscountRatePct,
                InflationRatePct = raw.InflationRatePct,
                EvaluationPeriodYears = raw.EvaluationPeriodYears,
                Severity = hasHi && hasStoredPof && hasStoredCof ? 1 : 0
            };
        }

        private static void ApplyTopsis(IList<DmDecisionInfo> candidates)
        {
            if (candidates == null || candidates.Count == 0)
            {
                return;
            }

            var matrix = new double[candidates.Count, 5];
            for (int i = 0; i < candidates.Count; i++)
            {
                DmDecisionInfo candidate = candidates[i];
                matrix[i, 0] = candidate.Risk;
                matrix[i, 1] = candidate.Bcr;
                matrix[i, 2] = candidate.HI;
                matrix[i, 3] = candidate.RULYears.GetValueOrDefault(10d);
                matrix[i, 4] = candidate.Criticality;
            }

            IList<AmsV31TopsisResult> ranks = AmsV31DecisionMath.Rank(matrix);
            double consistencyRatio = AmsV31DecisionMath.CalculateConsistencyRatio(
                AmsV31DecisionMath.CalculateAhpWeights());
            foreach (AmsV31TopsisResult rank in ranks)
            {
                DmDecisionInfo candidate = candidates[rank.Index];
                candidate.TopsisScore = rank.Score;
                candidate.TopsisRank = rank.Rank;
                candidate.Priority = candidate.Severity == 0 ? (int?)null : rank.Rank;
                candidate.DMScore = rank.Score;
                candidate.AhpConsistencyRatio = consistencyRatio;
                SetDecision(candidate);
            }
        }

        private static void SetDecision(DmDecisionInfo candidate)
        {
            if (candidate.Severity == 0)
            {
                candidate.Decision = "데이터 확인 필요";
                candidate.Urgency = "확인 필요";
                candidate.RecommendedAction = "HI 산정 필요";
                return;
            }

            if (candidate.PoFRatio > 0.8d || candidate.HI >= 4.8d
                || candidate.TopsisScore > 0.75d)
            {
                candidate.Severity = 5;
                candidate.Decision = "즉시 교체";
                candidate.Urgency = "매우 높음";
                candidate.RecommendedAction = "즉시";
            }
            else if ((candidate.PoFRatio > 0.6d && candidate.HI >= 4d)
                || candidate.TopsisScore > 0.55d)
            {
                candidate.Severity = 4;
                candidate.Decision = "긴급 유지보수";
                candidate.Urgency = "높음";
                candidate.RecommendedAction = "1~3개월";
            }
            else if (candidate.TopsisScore > 0.35d)
            {
                candidate.Severity = 3;
                candidate.Decision = "예방 유지보수";
                candidate.Urgency = "보통";
                candidate.RecommendedAction = "6~12개월";
            }
            else if (candidate.TopsisScore > 0.20d)
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
        }

        private static string GetEquipmentKey(string code, string name)
        {
            string keyFromCode = AmsV31Config.NormalizeEquipmentKey(code);
            if (keyFromCode == "DCCABLE" || keyFromCode == "SUBMODULE"
                || keyFromCode == "DCCB" || keyFromCode == "ITR" || keyFromCode == "VCB")
            {
                return keyFromCode;
            }

            string keyFromName = AmsV31Config.NormalizeEquipmentKey(name);
            return string.IsNullOrWhiteSpace(keyFromName) ? "VCB" : keyFromName;
        }

        private static bool TryParseDouble(string value, out double result)
        {
            return double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result)
                || double.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out result);
        }
    }
}
