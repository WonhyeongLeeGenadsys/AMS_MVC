using System;
using System.Collections.Generic;
using System.Linq;

namespace Web.Common
{
    /// <summary>
    /// MVDC AMS v3.1.0 기준 4D CoF 및 BCR/NPV 계산기.
    /// CoF 차원 정규화는 단일 장비가 아니라 전체 장비 집합에서 수행한다.
    /// </summary>
    public sealed class OriginalDmCalculator
    {
        private readonly IDictionary<string, double> replacementCosts;

        public OriginalDmCalculator()
            : this(LoadReplacementCosts())
        {
        }

        public OriginalDmCalculator(IDictionary<string, double> replacementCosts)
        {
            this.replacementCosts = replacementCosts
                ?? new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 기존 호출부 호환용 단일 장비 계산. 전체 장비 DM에서는 ApplyFleetCalculation을 사용한다.
        /// </summary>
        public OriginalDmResult Calculate(string equipmentKey, double pofRatio, int ageYears)
        {
            OriginalDmResult result = CalculateRaw(equipmentKey, pofRatio, ageYears, 35d, 25d);
            result.CofFinancial = AmsV31Config.CofWeightFinancial * result.RawCofFinancial;
            result.CofReliability = AmsV31Config.CofWeightReliability * result.RawCofReliability;
            result.CofSafety = AmsV31Config.CofWeightSafety * result.RawCofSafety;
            result.CofEnvironmental = AmsV31Config.CofWeightEnvironmental * result.RawCofEnvironmental;
            result.CofTotal = result.CofFinancial + result.CofReliability
                + result.CofSafety + result.CofEnvironmental;
            ApplyEconomics(result, pofRatio, ageYears);
            return result;
        }

        public OriginalDmResult CalculateRaw(
            string equipmentKey,
            double pofRatio,
            int ageYears,
            double voltageRatingKv,
            double currentRatingKa)
        {
            string key = AmsV31Config.NormalizeEquipmentKey(equipmentKey);
            AmsV31AssetConfig asset = AmsV31Config.GetAsset(key);
            double baseCost = GetReplacementCost(key, asset.ReplacementCost);
            double pof = AmsV31Config.Clamp(pofRatio, 0d, 1d);

            double outageCost = AmsV31Config.MvdcCapacityMw * 1000d
                * AmsV31Config.AverageOutageHours
                * AmsV31Config.VollKrwPerKwh
                * asset.SystemCriticality;
            double emergencyCost = baseCost * (asset.EmergencyMultiplier - 1d) * 0.1d;
            double laborCost = AmsV31Config.LaborCostPerHour
                * AmsV31Config.AverageOutageHours * 3d;
            double financial = outageCost + emergencyCost + laborCost;

            double cens = AmsV31Config.MvdcCapacityMw * 1000d
                * asset.RestorationHours
                * asset.SystemCriticality
                * AmsV31Config.VollKrwPerKwh;
            double customersAffected = AmsV31Config.TotalCustomers
                * asset.CustomerImpactRatio
                * AmsV31Config.StationACustomerFraction
                * AmsV31Config.NetworkRedundancyFactor;
            double restorationMinutes = asset.RestorationHours * 60d;
            double saidiPenalty = restorationMinutes * customersAffected
                * AmsV31Config.SaidiPenaltyPerCustomerMinute;
            double saifiPenalty = customersAffected
                * AmsV31Config.SaifiPenaltyPerCustomerEvent;
            double reliability = cens + saidiPenalty + saifiPenalty;
            double saidiContribution = pof * restorationMinutes
                * (customersAffected / AmsV31Config.TotalCustomers);
            double saifiContribution = pof
                * (customersAffected / AmsV31Config.TotalCustomers);

            double safety = asset.InjuryRisk
                * AmsV31Config.WorkersAtRisk
                * AmsV31Config.InjuryCompensationPerPerson;
            if (key == "DCCB" || key == "DCCABLE")
            {
                double normalizedVoltageKv = voltageRatingKv > 1000d
                    ? voltageRatingKv / 1000d
                    : Math.Max(voltageRatingKv, 0d);
                double normalizedCurrentKa = currentRatingKa > 100d
                    ? currentRatingKa / 1000d
                    : Math.Max(currentRatingKa, 0d);
                double arcEnergyKj = normalizedVoltageKv * normalizedCurrentKa
                    * (20d / 1000d) * 10d;
                safety += arcEnergyKj * 100000d;
            }

            double environmental = 0d;
            if (asset.OilVolumeTons > 0d)
            {
                environmental += asset.OilVolumeTons
                    * AmsV31Config.OilDisposalCostPerTon
                    + (AmsV31Config.EnvironmentalCleanupBase * 0.5d);
            }
            if (asset.Sf6MassKg > 0d)
            {
                environmental += asset.Sf6MassKg * 500000d;
            }
            environmental += asset.BatteryHazmatCost;

            double legacyWeighted =
                AmsV31Config.CofWeightFinancial * financial
                + AmsV31Config.CofWeightReliability * reliability
                + AmsV31Config.CofWeightSafety * safety
                + AmsV31Config.CofWeightEnvironmental * environmental;

            return new OriginalDmResult
            {
                AssetType = asset.AssetType,
                ReplacementCost = baseCost,
                StatisticalLifetime = asset.StatisticalLifetime,
                Criticality = asset.SystemCriticality,
                RawCofFinancial = financial,
                RawCofReliability = reliability,
                RawCofSafety = safety,
                RawCofEnvironmental = environmental,
                CofCens = cens,
                CofSaidiPenalty = saidiPenalty,
                CofSaifiPenalty = saifiPenalty,
                SaidiContribution = saidiContribution,
                SaifiContribution = saifiContribution,
                CustomersAffected = customersAffected,
                CofTotalKrw = legacyWeighted,
                DiscountRatePct = AmsV31Config.DiscountRate * 100d,
                InflationRatePct = AmsV31Config.InflationRate * 100d,
                EvaluationPeriodYears = AmsV31Config.EvaluationPeriodYears
            };
        }

        /// <summary>
        /// 기준본처럼 각 CoF 차원을 fleet min-max 정규화한 뒤 평균 금액 규모를 보존한다.
        /// </summary>
        public void ApplyFleetCalculation(IList<DmDecisionInfo> candidates)
        {
            if (candidates == null || candidates.Count == 0)
            {
                return;
            }

            var dimensions = new[]
            {
                candidates.Select(x => x.RawCofFinancial).ToArray(),
                candidates.Select(x => x.RawCofReliability).ToArray(),
                candidates.Select(x => x.RawCofSafety).ToArray(),
                candidates.Select(x => x.RawCofEnvironmental).ToArray()
            };
            double[] weights =
            {
                AmsV31Config.CofWeightFinancial,
                AmsV31Config.CofWeightReliability,
                AmsV31Config.CofWeightSafety,
                AmsV31Config.CofWeightEnvironmental
            };

            double[][] normalized = dimensions
                .Select(values => MinMax(values))
                .ToArray();
            double[] severity = Enumerable.Range(0, candidates.Count)
                .Select(row => Enumerable.Range(0, weights.Length)
                    .Sum(col => weights[col] * normalized[col][row]))
                .ToArray();
            double severityMean = severity.Average();
            double legacyMean = candidates.Average(x => x.CofTotalKrw);
            double scale = severityMean > 1e-12d
                ? legacyMean / severityMean
                : 0d;
            bool normalizedApplied = candidates.Count >= 2 && scale > 0d;

            for (int i = 0; i < candidates.Count; i++)
            {
                DmDecisionInfo candidate = candidates[i];
                if (normalizedApplied)
                {
                    candidate.CofFinancial = weights[0] * normalized[0][i] * scale;
                    candidate.CofReliability = weights[1] * normalized[1][i] * scale;
                    candidate.CofSafety = weights[2] * normalized[2][i] * scale;
                    candidate.CofEnvironmental = weights[3] * normalized[3][i] * scale;
                }
                else
                {
                    candidate.CofFinancial = weights[0] * candidate.RawCofFinancial;
                    candidate.CofReliability = weights[1] * candidate.RawCofReliability;
                    candidate.CofSafety = weights[2] * candidate.RawCofSafety;
                    candidate.CofEnvironmental = weights[3] * candidate.RawCofEnvironmental;
                }

                candidate.DmCofKrw = candidate.CofFinancial + candidate.CofReliability
                    + candidate.CofSafety + candidate.CofEnvironmental;
                // 운영 Risk Matrix의 CoF(목포대 저장값)가 있으면 유지한다.
                // 저장값이 없는 기존 단일 계산/테스트 호출만 4D CoF를 대체값으로 사용한다.
                if (candidate.CoF <= 0d)
                {
                    candidate.CoF = candidate.DmCofKrw;
                }
                ApplyEconomics(candidate);
            }
        }

        private static void ApplyEconomics(DmDecisionInfo candidate)
        {
            AmsV31AssetConfig asset = AmsV31Config.GetAsset(candidate.EquipmentKey);
            double pof = AmsV31Config.Clamp(candidate.PoFRatio, 0d, 1d);
            double age = Math.Max(0d, candidate.UsageYears);
            double baseCost = candidate.ReplacementCost;

            // Risk는 목포대 RISKMATRIX의 CoF(USD)를 기준으로 표시·순위 계산한다.
            candidate.Risk = pof * candidate.CoF;
            // NPV/BCR 계산은 교체비와 통화를 맞추기 위해 숭실대 4D CoF(KRW)를 사용한다.
            double economicCofKrw = candidate.DmCofKrw > 0d
                ? candidate.DmCofKrw
                : candidate.CoF;
            candidate.RiskMitigation = pof * economicCofKrw * 0.50d;
            candidate.AnnualMaintenanceSaving = baseCost * 0.02d
                * asset.MaintenanceReductionRate;
            double annualEnergyKwh = AmsV31Config.MvdcCapacityMw * 1000d
                * AmsV31Config.AnnualHours * AmsV31Config.UtilizationRate;
            candidate.AnnualEfficiencyBenefit = annualEnergyKwh
                * asset.EfficiencyGainRate
                * AmsV31Config.ElectricityPriceKrwPerKwh;
            candidate.AnnualBenefits = candidate.RiskMitigation
                + candidate.AnnualMaintenanceSaving
                + candidate.AnnualEfficiencyBenefit;

            candidate.ExtendedLifetimeValue = asset.StatisticalLifetime > 0d
                ? (baseCost / asset.StatisticalLifetime)
                    * Math.Min(age, asset.StatisticalLifetime)
                : 0d;
            candidate.InstallationCost = baseCost * AmsV31Config.InstallationCostRate;
            candidate.DisposalCost = baseCost * AmsV31Config.DisposalCostRate;
            candidate.TotalCosts = baseCost + candidate.InstallationCost + candidate.DisposalCost;
            candidate.NpvCosts = candidate.TotalCosts;

            double presentValueFactor = GetPresentValueFactor();
            candidate.DiscountedAnnualBenefits = candidate.AnnualBenefits * presentValueFactor;
            candidate.NpvBenefits = candidate.ExtendedLifetimeValue
                + candidate.DiscountedAnnualBenefits;
            candidate.TotalBenefits = candidate.NpvBenefits;
            candidate.NpvValue = candidate.NpvBenefits - candidate.NpvCosts;
            candidate.Bcr = candidate.NpvCosts > 0d
                ? candidate.NpvBenefits / candidate.NpvCosts
                : 0d;
            candidate.RoiPct = candidate.NpvCosts > 0d
                ? candidate.NpvValue / candidate.NpvCosts * 100d
                : 0d;
            candidate.DiscountRatePct = AmsV31Config.DiscountRate * 100d;
            candidate.InflationRatePct = AmsV31Config.InflationRate * 100d;
            candidate.EvaluationPeriodYears = AmsV31Config.EvaluationPeriodYears;
        }

        private static void ApplyEconomics(OriginalDmResult result, double pofRatio, int ageYears)
        {
            var candidate = new DmDecisionInfo
            {
                EquipmentKey = result.AssetType,
                ReplacementCost = result.ReplacementCost,
                PoFRatio = pofRatio,
                CoF = result.CofTotal,
                UsageYears = ageYears
            };
            ApplyEconomics(candidate);
            result.Risk = candidate.Risk;
            result.NpvValue = candidate.NpvValue;
            result.NpvBenefits = candidate.NpvBenefits;
            result.NpvCosts = candidate.NpvCosts;
            result.Bcr = candidate.Bcr;
            result.RoiPct = candidate.RoiPct;
            result.RiskMitigation = candidate.RiskMitigation;
            result.TotalBenefits = candidate.TotalBenefits;
            result.TotalCosts = candidate.TotalCosts;
            result.InstallationCost = candidate.InstallationCost;
            result.DisposalCost = candidate.DisposalCost;
            result.AnnualBenefits = candidate.AnnualBenefits;
            result.AnnualMaintenanceSaving = candidate.AnnualMaintenanceSaving;
            result.AnnualEfficiencyBenefit = candidate.AnnualEfficiencyBenefit;
            result.ExtendedLifetimeValue = candidate.ExtendedLifetimeValue;
            result.DiscountedAnnualBenefits = candidate.DiscountedAnnualBenefits;
        }

        private double GetReplacementCost(string equipmentKey, double defaultCost)
        {
            double configuredCost;
            return replacementCosts.TryGetValue(equipmentKey, out configuredCost)
                && configuredCost > 0d
                    ? configuredCost
                    : defaultCost;
        } 

        private static double[] MinMax(double[] values)
        {
            double minimum = values.Min();
            double maximum = values.Max();
            double range = maximum - minimum;
            return range > 0d
                ? values.Select(x => (x - minimum) / range).ToArray()
                : values.Select(x => 0d).ToArray();
        }

        private static double GetPresentValueFactor()
        {
            double total = 0d;
            for (int year = 1; year <= AmsV31Config.EvaluationPeriodYears; year++)
            {
                total += Math.Pow(1d + AmsV31Config.InflationRate, year)
                    / Math.Pow(1d + AmsV31Config.DiscountRate, year);
            }
            return total;
        }

        private static IDictionary<string, double> LoadReplacementCosts()
        {
            try
            {
                return new DmEquipmentCostRepository().GetActiveReplacementCosts();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(
                    "OriginalDmCalculator",
                    "TB_DM_EQUIPMENT_COST 조회 실패, v3.1 기본 교체비 사용: " + ex.Message);
                return new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
            }
        }
    }

    public sealed class OriginalDmResult
    {
        public string AssetType { get; set; }
        public double ReplacementCost { get; set; }
        public double StatisticalLifetime { get; set; }
        public double Criticality { get; set; }
        public double RawCofFinancial { get; set; }
        public double RawCofReliability { get; set; }
        public double RawCofSafety { get; set; }
        public double RawCofEnvironmental { get; set; }
        public double CofFinancial { get; set; }
        public double CofReliability { get; set; }
        public double CofSafety { get; set; }
        public double CofEnvironmental { get; set; }
        public double CofCens { get; set; }
        public double CofSaidiPenalty { get; set; }
        public double CofSaifiPenalty { get; set; }
        public double SaidiContribution { get; set; }
        public double SaifiContribution { get; set; }
        public double CustomersAffected { get; set; }
        public double CofTotalKrw { get; set; }
        public double CofTotal { get; set; }
        public double Risk { get; set; }
        public double NpvValue { get; set; }
        public double NpvBenefits { get; set; }
        public double NpvCosts { get; set; }
        public double Bcr { get; set; }
        public double RoiPct { get; set; }
        public double RiskMitigation { get; set; }
        public double TotalBenefits { get; set; }
        public double TotalCosts { get; set; }
        public double InstallationCost { get; set; }
        public double DisposalCost { get; set; }
        public double AnnualBenefits { get; set; }
        public double AnnualMaintenanceSaving { get; set; }
        public double AnnualEfficiencyBenefit { get; set; }
        public double ExtendedLifetimeValue { get; set; }
        public double DiscountedAnnualBenefits { get; set; }
        public double DiscountRatePct { get; set; }
        public double InflationRatePct { get; set; }
        public int EvaluationPeriodYears { get; set; }
    }
}
