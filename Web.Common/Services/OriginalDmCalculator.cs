using System;

namespace Web.Common
{
    /// <summary>
    /// 숭실대 DM모듈 배포본의 CoF/NPV 공식을 AMS에서 사용하기 위한 계산기.
    /// 원본: dm_cof_model.py, dm_npv_calculator.py, config.yaml
    /// </summary>
    public sealed class OriginalDmCalculator
    {
        private const double VollKrwPerKwh = 11000d;
        private const double MvdcCapacityMw = 20d;
        private const double AverageOutageHours = 4d;
        private const double ElectricityPriceKrwPerKwh = 100d;
        private const double InjuryCompensationBase = 50000000d;
        private const double LegalPenaltyBase = 100000000d;
        private const double OilDisposalCostPerTon = 1000000d;

        private const double DiscountRate = 0.0457d;
        private const double InflationRate = 0.01d;
        private const int EvaluationPeriodYears = 30;
        private const double InstallationCostRate = 0.10d;
        private const double DisposalCostRate = 0.05d;

        public OriginalDmResult Calculate(string equipmentKey, double pofRatio, int ageYears)
        {
            OriginalDmAssetParameter parameter = GetParameter(equipmentKey);
            double pof = Math.Max(0d, Math.Min(1d, pofRatio));
            double age = Math.Max(0d, ageYears);
            double baseCost = parameter.ReplacementCostBase;

            // CoF_financial = emergency replacement + revenue loss + property damage
            double emergencyReplacementCost = baseCost * parameter.EmergencyMultiplier;
            double outageCapacityMw = MvdcCapacityMw * parameter.SystemCriticality;
            double outageEnergyKwh = outageCapacityMw * 1000d * AverageOutageHours;
            double revenueLoss = outageEnergyKwh * ElectricityPriceKrwPerKwh;
            double propertyDamage = baseCost * parameter.PropertyRisk * 0.30d;
            double cofFinancial = emergencyReplacementCost + revenueLoss + propertyDamage;

            // CoF_reliability = VOLL * affected customers * demand * outage hours
            const double affectedCustomers = 10000d;
            const double averageDemandKwPerCustomer = 2d;
            double actualAffectedCustomers = affectedCustomers * parameter.SystemCriticality;
            double outageVolumeKwh = actualAffectedCustomers
                * averageDemandKwPerCustomer
                * AverageOutageHours;
            double cofReliability = outageVolumeKwh * VollKrwPerKwh;

            // CoF_safety = injury compensation + legal penalties
            double cofSafety = (InjuryCompensationBase + LegalPenaltyBase)
                * parameter.InjuryRisk;

            // 원본은 Transformer에만 절연유 폐기 비용을 적용한다.
            double cofEnvironmental = parameter.AssetType == "Transformer"
                ? (baseCost / 800000000d) * OilDisposalCostPerTon
                : 0d;

            double cofTotal = cofFinancial + cofReliability + cofSafety + cofEnvironmental;
            double risk = pof * cofTotal;

            // NPV 편익: 위험 경감 + 유지보수 절감 + 효율 개선 + 수명 연장
            double riskMitigation = risk;
            double annualMaintenanceOld = baseCost * 0.02d;
            double annualMaintenanceSaving = annualMaintenanceOld
                * parameter.MaintenanceReductionRate;

            const double annualHours = 8760d;
            const double utilizationRate = 0.5d;
            double annualEnergyKwh = MvdcCapacityMw * 1000d * annualHours * utilizationRate;
            double annualEfficiencyBenefit = annualEnergyKwh
                * parameter.EfficiencyGainRate
                * ElectricityPriceKrwPerKwh;

            double remainingLifeOld = Math.Max(parameter.StatisticalLifetime - age, 0d);
            double lifetimeExtension = parameter.StatisticalLifetime - remainingLifeOld;
            double annualLifetimeValue = baseCost / parameter.StatisticalLifetime;
            double extendedLifetimeValue = annualLifetimeValue * lifetimeExtension;
            double totalAnnualBenefits = annualMaintenanceSaving + annualEfficiencyBenefit;

            // NPV 비용: 교체비 + 설치비(10%) + 폐기비(5%)
            double totalInitialCosts = baseCost
                + (baseCost * InstallationCostRate)
                + (baseCost * DisposalCostRate);

            double npv = -totalInitialCosts + riskMitigation + extendedLifetimeValue;
            for (int year = 1; year <= EvaluationPeriodYears; year++)
            {
                double discountFactor = 1d / Math.Pow(1d + DiscountRate, year);
                double inflationFactor = Math.Pow(1d + InflationRate, year);
                npv += totalAnnualBenefits * inflationFactor * discountFactor;
            }

            double roiPct = totalInitialCosts > 0d
                ? (npv / totalInitialCosts) * 100d
                : 0d;

            return new OriginalDmResult
            {
                AssetType = parameter.AssetType,
                ReplacementCost = baseCost,
                StatisticalLifetime = parameter.StatisticalLifetime,
                CofFinancial = cofFinancial,
                CofReliability = cofReliability,
                CofSafety = cofSafety,
                CofEnvironmental = cofEnvironmental,
                CofTotal = cofTotal,
                Risk = risk,
                NpvValue = npv,
                RoiPct = roiPct,
                RiskMitigation = riskMitigation,
                TotalBenefits = totalAnnualBenefits * EvaluationPeriodYears,
                TotalCosts = totalInitialCosts,
                AnnualBenefits = totalAnnualBenefits,
                AnnualMaintenanceSaving = annualMaintenanceSaving,
                AnnualEfficiencyBenefit = annualEfficiencyBenefit,
                ExtendedLifetimeValue = extendedLifetimeValue
            };
        }

        private static OriginalDmAssetParameter GetParameter(string equipmentKey)
        {
            switch ((equipmentKey ?? "").Trim().ToUpperInvariant())
            {
                case "ITR":
                    return new OriginalDmAssetParameter(
                        "Transformer", 800000000d, 32d, 1.5d, 0.6d, 0.7d, 0.8d, 0.005d, 0.20d);
                case "SUBMODULE":
                    return new OriginalDmAssetParameter(
                        "SubModule", 1000000000d, 35d, 1.8d, 0.7d, 0.6d, 0.6d, 0.025d, 0.25d);
                case "DCCB":
                    return new OriginalDmAssetParameter(
                        "DC_CircuitBreaker", 1200000000d, 35d, 2.0d, 0.9d, 0.7d, 0.9d, 0.010d, 0.30d);
                case "DCCABLE":
                    return new OriginalDmAssetParameter(
                        "DC_Cable", 300000000d, 40d, 1.4d, 0.3d, 0.2d, 0.4d, 0.002d, 0.15d);
                case "VCB":
                default:
                    return new OriginalDmAssetParameter(
                        "VCB", 200000000d, 35d, 1.5d, 0.5d, 0.4d, 0.7d, 0.003d, 0.20d);
            }
        }

        private sealed class OriginalDmAssetParameter
        {
            public OriginalDmAssetParameter(
                string assetType,
                double replacementCostBase,
                double statisticalLifetime,
                double emergencyMultiplier,
                double injuryRisk,
                double propertyRisk,
                double systemCriticality,
                double efficiencyGainRate,
                double maintenanceReductionRate)
            {
                AssetType = assetType;
                ReplacementCostBase = replacementCostBase;
                StatisticalLifetime = statisticalLifetime;
                EmergencyMultiplier = emergencyMultiplier;
                InjuryRisk = injuryRisk;
                PropertyRisk = propertyRisk;
                SystemCriticality = systemCriticality;
                EfficiencyGainRate = efficiencyGainRate;
                MaintenanceReductionRate = maintenanceReductionRate;
            }

            public string AssetType { get; private set; }
            public double ReplacementCostBase { get; private set; }
            public double StatisticalLifetime { get; private set; }
            public double EmergencyMultiplier { get; private set; }
            public double InjuryRisk { get; private set; }
            public double PropertyRisk { get; private set; }
            public double SystemCriticality { get; private set; }
            public double EfficiencyGainRate { get; private set; }
            public double MaintenanceReductionRate { get; private set; }
        }
    }

    public sealed class OriginalDmResult
    {
        public string AssetType { get; set; }
        public double ReplacementCost { get; set; }
        public double StatisticalLifetime { get; set; }
        public double CofFinancial { get; set; }
        public double CofReliability { get; set; }
        public double CofSafety { get; set; }
        public double CofEnvironmental { get; set; }
        public double CofTotal { get; set; }
        public double Risk { get; set; }
        public double NpvValue { get; set; }
        public double RoiPct { get; set; }
        public double RiskMitigation { get; set; }
        public double TotalBenefits { get; set; }
        public double TotalCosts { get; set; }
        public double AnnualBenefits { get; set; }
        public double AnnualMaintenanceSaving { get; set; }
        public double AnnualEfficiencyBenefit { get; set; }
        public double ExtendedLifetimeValue { get; set; }
    }
}
