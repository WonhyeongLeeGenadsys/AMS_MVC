using System;
using System.Collections.Generic;

namespace Web.Common
{
    /// <summary>
    /// MVDC AMS v3.1.0 배포본(config/config.yaml)의 계산 기준.
    /// 화면과 배치 계산이 동일한 기준을 사용하도록 한 곳에서 관리한다.
    /// </summary>
    public static class AmsV31Config
    {
        public const double DiagnosticAlpha = 0.70d;
        public const double LogisticK = 5.0d;
        public const double LogisticX0 = 4.0d;
        public const double FrequencyCoefficient = 0.99d;
        public const double RulTargetPof = 0.50d;

        public const double MvdcCapacityMw = 20d;
        public const double AnnualHours = 8760d;
        public const double UtilizationRate = 0.50d;
        public const double ElectricityPriceKrwPerKwh = 100d;
        public const double VollKrwPerKwh = 11000d;
        public const double AverageOutageHours = 4d;
        public const double LaborCostPerHour = 100000d;

        public const double DiscountRate = 0.0457d;
        public const double InflationRate = 0.01d;
        public const int EvaluationPeriodYears = 30;
        public const double InstallationCostRate = 0.10d;
        public const double DisposalCostRate = 0.05d;

        public const double CofWeightFinancial = 0.40d;
        public const double CofWeightReliability = 0.30d;
        public const double CofWeightSafety = 0.20d;
        public const double CofWeightEnvironmental = 0.10d;

        public const int TotalCustomers = 50000;
        public const double StationACustomerFraction = 0.30d;
        public const double NetworkRedundancyFactor = 0.02d;
        public const double SaidiPenaltyPerCustomerMinute = 200d;
        public const double SaifiPenaltyPerCustomerEvent = 5000d;
        public const double InjuryCompensationPerPerson = 50000000d;
        public const int WorkersAtRisk = 2;
        public const double EnvironmentalCleanupBase = 100000000d;
        public const double OilDisposalCostPerTon = 1000000d;

        private static readonly IDictionary<string, AmsV31AssetConfig> Assets =
            new Dictionary<string, AmsV31AssetConfig>(StringComparer.OrdinalIgnoreCase)
            {
                { "ITR", new AmsV31AssetConfig("ITR", "Transformer", 800000000d, 35d, 2.5d, 35d, 0.000078d, 1.087d, 1.5d, 72d, 0.80d, 0.60d, 0.85d, 0.005d, 0.20d, 5d, 0d, 0d) },
                { "VCB", new AmsV31AssetConfig("VCB", "VCB", 200000000d, 35d, 3.0d, 30d, 0.000041d, 1.087d, 1.5d, 24d, 0.50d, 0.50d, 0.70d, 0.003d, 0.20d, 0d, 0d, 0d) },
                { "SUBMODULE", new AmsV31AssetConfig("SUBMODULE", "MMC_Submodule", 50000000d, 10d, 2.2d, 10d, 0.000078d, 1.087d, 2.2d, 12d, 0.10d, 0.75d, 0.50d, 0.025d, 0.35d, 0d, 0d, 0d) },
                { "DCCB", new AmsV31AssetConfig("DCCB", "DC_Breaker", 1200000000d, 22d, 3.0d, 22d, 0.000041d, 1.087d, 2.5d, 36d, 0.90d, 0.95d, 0.95d, 0.005d, 0.25d, 0d, 0d, 0d) },
                { "DCCABLE", new AmsV31AssetConfig("DCCABLE", "DC_Cable", 300000000d, 32d, 2.5d, 32d, 0.020944d, 1.087d, 1.6d, 48d, 0.60d, 0.25d, 0.40d, 0.003d, 0.12d, 0d, 0d, 0d) },

                { "CONVERTER", new AmsV31AssetConfig("CONVERTER", "Converter", 1500000000d, 20d, 2.8d, 20d, 0d, 0d, 2.0d, 48d, 1.00d, 0.80d, 1.00d, 0.030d, 0.30d, 0d, 0d, 0d) },
                { "CIRCUIT_BREAKER", new AmsV31AssetConfig("CIRCUIT_BREAKER", "Circuit_Breaker", 150000000d, 30d, 3.0d, 30d, 0d, 0d, 1.5d, 24d, 0.50d, 0.50d, 0.70d, 0.003d, 0.20d, 0.1d, 5d, 0d) },
                { "CABLE", new AmsV31AssetConfig("CABLE", "Cable", 250000000d, 40d, 2.2d, 40d, 0d, 0d, 1.3d, 36d, 0.30d, 0.20d, 0.40d, 0.001d, 0.10d, 0d, 0d, 0d) },
                { "SWITCHGEAR", new AmsV31AssetConfig("SWITCHGEAR", "Switchgear", 180000000d, 25d, 2.6d, 25d, 0d, 0d, 1.4d, 24d, 0.40d, 0.40d, 0.60d, 0.002d, 0.15d, 0d, 3d, 0d) },
                { "PROTECTION_RELAY", new AmsV31AssetConfig("PROTECTION_RELAY", "Protection_Relay", 80000000d, 15d, 2.4d, 15d, 0d, 0d, 1.6d, 8d, 0.60d, 0.10d, 0.80d, 0d, 0.25d, 0d, 0d, 0d) },
                { "COOLING_SYSTEM", new AmsV31AssetConfig("COOLING_SYSTEM", "Cooling_System", 120000000d, 12d, 2.0d, 12d, 0d, 0d, 1.5d, 16d, 0.70d, 0.30d, 0.70d, 0.020d, 0.30d, 0d, 0d, 0d) },
                { "SCADA", new AmsV31AssetConfig("SCADA", "SCADA", 200000000d, 10d, 1.8d, 10d, 0d, 0d, 1.7d, 8d, 0.50d, 0.05d, 0.90d, 0d, 0.35d, 0d, 0d, 0d) },
                { "ENERGY_STORAGE", new AmsV31AssetConfig("ENERGY_STORAGE", "Energy_Storage", 800000000d, 12d, 2.0d, 12d, 0d, 0d, 2.0d, 24d, 0.20d, 0.85d, 0.60d, 0.050d, 0.40d, 0d, 0d, 50000000d) }
            };

        public static AmsV31AssetConfig GetAsset(string equipmentKey)
        {
            AmsV31AssetConfig result;
            return Assets.TryGetValue(NormalizeEquipmentKey(equipmentKey), out result)
                ? result
                : Assets["VCB"];
        }

        public static IEnumerable<AmsV31AssetConfig> GetAllAssets()
        {
            return Assets.Values;
        }

        public static string NormalizeEquipmentKey(string value)
        {
            string key = (value ?? string.Empty)
                .Trim()
                .Replace(" ", "")
                .Replace("-", "")
                .ToUpperInvariant();

            if (key.StartsWith("DCCABLE") || key.Contains("DCCABLE")) return "DCCABLE";
            if (key.StartsWith("SUBMODULE") || key.Contains("MMCSUBMODULE")) return "SUBMODULE";
            if (key.StartsWith("DCCB") || key.Contains("DCBREAKER")) return "DCCB";
            if (key.StartsWith("ITR") || key.Contains("INTERFACETR") || key == "TRANSFORMER") return "ITR";
            if (key.StartsWith("VCB")) return "VCB";
            if (key.Contains("CONVERTER")) return "CONVERTER";
            if (key.Contains("CIRCUITBREAKER")) return "CIRCUIT_BREAKER";
            if (key == "CABLE") return "CABLE";
            if (key.Contains("SWITCHGEAR")) return "SWITCHGEAR";
            if (key.Contains("PROTECTIONRELAY")) return "PROTECTION_RELAY";
            if (key.Contains("COOLINGSYSTEM")) return "COOLING_SYSTEM";
            if (key.Contains("SCADA")) return "SCADA";
            if (key.Contains("ENERGYSTORAGE")) return "ENERGY_STORAGE";
            return key;
        }

        public static double CalculateDiagnosticPof(double hi, string equipmentKey)
        {
            var asset = GetAsset(equipmentKey);
            double clippedHi = Clamp(hi, 1d, 5d);
            double pofCondition = 1d / (1d + Math.Exp(-LogisticK * (clippedHi - LogisticX0)));
            double x = asset.ExponentialC * clippedHi;
            double pofAging = asset.ExponentialK > 0d
                ? asset.ExponentialK * (1d + x + (x * x / 2d) + (x * x * x / 6d))
                : CalculateWeibullPof(asset, 0d);
            pofAging = Clamp(pofAging, 0d, 1d);
            return Clamp((DiagnosticAlpha * pofCondition) + ((1d - DiagnosticAlpha) * pofAging), 0d, 1d);
        }

        public static double CalculateWeibullPof(AmsV31AssetConfig asset, double ageYears)
        {
            if (asset == null || ageYears <= 0d || asset.WeibullScale <= 0d || asset.WeibullShape <= 0d)
            {
                return 0d;
            }

            return Clamp(1d - Math.Exp(-Math.Pow(ageYears / asset.WeibullScale, asset.WeibullShape)), 0d, 1d);
        }

        public static double CalculateRulYears(string equipmentKey, double ageYears)
        {
            var asset = GetAsset(equipmentKey);
            if (asset.WeibullScale <= 0d || asset.WeibullShape <= 0d)
            {
                return 0d;
            }

            double targetAge = asset.WeibullScale
                * Math.Pow(-Math.Log(1d - RulTargetPof), 1d / asset.WeibullShape);
            return Math.Max(0d, targetAge - Math.Max(0d, ageYears));
        }

        public static double Clamp(double value, double minimum, double maximum)
        {
            return Math.Max(minimum, Math.Min(maximum, value));
        }
    }

    public sealed class AmsV31AssetConfig
    {
        public AmsV31AssetConfig(
            string equipmentKey,
            string assetType,
            double replacementCost,
            double statisticalLifetime,
            double weibullShape,
            double weibullScale,
            double exponentialK,
            double exponentialC,
            double emergencyMultiplier,
            double restorationHours,
            double customerImpactRatio,
            double injuryRisk,
            double systemCriticality,
            double efficiencyGainRate,
            double maintenanceReductionRate,
            double oilVolumeTons,
            double sf6MassKg,
            double batteryHazmatCost)
        {
            EquipmentKey = equipmentKey;
            AssetType = assetType;
            ReplacementCost = replacementCost;
            StatisticalLifetime = statisticalLifetime;
            WeibullShape = weibullShape;
            WeibullScale = weibullScale;
            ExponentialK = exponentialK;
            ExponentialC = exponentialC;
            EmergencyMultiplier = emergencyMultiplier;
            RestorationHours = restorationHours;
            CustomerImpactRatio = customerImpactRatio;
            InjuryRisk = injuryRisk;
            SystemCriticality = systemCriticality;
            EfficiencyGainRate = efficiencyGainRate;
            MaintenanceReductionRate = maintenanceReductionRate;
            OilVolumeTons = oilVolumeTons;
            Sf6MassKg = sf6MassKg;
            BatteryHazmatCost = batteryHazmatCost;
        }

        public string EquipmentKey { get; private set; }
        public string AssetType { get; private set; }
        public double ReplacementCost { get; private set; }
        public double StatisticalLifetime { get; private set; }
        public double WeibullShape { get; private set; }
        public double WeibullScale { get; private set; }
        public double ExponentialK { get; private set; }
        public double ExponentialC { get; private set; }
        public double EmergencyMultiplier { get; private set; }
        public double RestorationHours { get; private set; }
        public double CustomerImpactRatio { get; private set; }
        public double InjuryRisk { get; private set; }
        public double SystemCriticality { get; private set; }
        public double EfficiencyGainRate { get; private set; }
        public double MaintenanceReductionRate { get; private set; }
        public double OilVolumeTons { get; private set; }
        public double Sf6MassKg { get; private set; }
        public double BatteryHazmatCost { get; private set; }
    }
}
