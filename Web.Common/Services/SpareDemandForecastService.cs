using System;
using System.Collections.Generic;
using System.Linq;

namespace Web.Common
{
    /// <summary>
    /// 원본 DM의 예비품 수요예측 공식을 AMS의 장비·예비품 매핑 데이터에 적용한다.
    /// </summary>
    public sealed class SpareDemandForecastService
    {
        public SpareDemandForecastResult Calculate(
            IEnumerable<DmDecisionInfo> decisions,
            IEnumerable<SpareDemandInput> demandInputs)
        {
            var validDecisions = (decisions ?? Enumerable.Empty<DmDecisionInfo>())
                .Where(x => x.Severity > 0)
                .ToList();
            var inputs = (demandInputs ?? Enumerable.Empty<SpareDemandInput>()).ToList();
            var rows = new List<SpareDemandForecastRow>();

            foreach (var partGroup in inputs.GroupBy(x => x.SPARE_ID))
            {
                var part = partGroup.First();
                double expectedDemand = 0d;
                int highestSeverity = 0;
                string topDecision = "-";
                var affectedAssets = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var assetTypeNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (var map in partGroup)
                {
                    assetTypeNames.Add(GetAssetTypeName(map.ASSET_TYPE_ID));
                    var mappedDecisions = validDecisions
                        .Where(x => GetAssetTypeId(x.EquipmentKey) == map.ASSET_TYPE_ID)
                        .ToList();

                    foreach (var decision in mappedDecisions)
                    {
                        int requiredQty = map.REQUIRED_QTY > 0 ? map.REQUIRED_QTY : 1;
                        double replacementRatio = GetReplacementRatio(decision.Severity);
                        double pof = Clamp(decision.PoFRatio, 0d, 1d);
                        double baseDemand = requiredQty * replacementRatio * pof;
                        double safetyFactor = decision.Severity >= 4
                            ? 1d + (Math.Max(0, map.LEAD_TIME_DAYS) / 365d)
                            : 1d;

                        expectedDemand += baseDemand * safetyFactor;
                        if (!string.IsNullOrWhiteSpace(decision.Code))
                        {
                            affectedAssets.Add(decision.Code);
                        }

                        if (decision.Severity > highestSeverity)
                        {
                            highestSeverity = decision.Severity;
                            topDecision = decision.Decision;
                        }
                    }
                }

                int recommendedQty = expectedDemand > 0d
                    ? (int)Math.Ceiling(expectedDemand)
                    : 0;
                int shortageQty = Math.Max(recommendedQty - part.CURRENT_QTY, 0);

                rows.Add(new SpareDemandForecastRow
                {
                    SPARE_ID = part.SPARE_ID,
                    PART_NUMBER = part.PART_NUMBER,
                    PART_NAME = part.PART_NAME,
                    ASSET_TYPE_NAME = string.Join(", ", assetTypeNames.OrderBy(x => x)),
                    CRITICALITY_GRADE = part.CRITICALITY_GRADE,
                    UNIT_PRICE = part.UNIT_PRICE,
                    LEAD_TIME_DAYS = part.LEAD_TIME_DAYS,
                    CURRENT_QTY = part.CURRENT_QTY,
                    AFFECTED_ASSET_COUNT = affectedAssets.Count,
                    TOP_DECISION = topDecision,
                    EXPECTED_DEMAND = Math.Round(expectedDemand, 4),
                    RECOMMENDED_QTY = recommendedQty,
                    SHORTAGE_QTY = shortageQty,
                    EXPECTED_COST = recommendedQty * part.UNIT_PRICE
                });
            }

            rows = rows
                .OrderByDescending(x => x.SHORTAGE_QTY)
                .ThenByDescending(x => x.EXPECTED_COST)
                .ThenBy(x => x.PART_NUMBER)
                .ToList();

            return new SpareDemandForecastResult
            {
                TOTAL_PART_COUNT = rows.Count,
                FORECAST_PART_COUNT = rows.Count(x => x.RECOMMENDED_QTY > 0),
                SHORTAGE_PART_COUNT = rows.Count(x => x.SHORTAGE_QTY > 0),
                TOTAL_RECOMMENDED_QTY = rows.Sum(x => x.RECOMMENDED_QTY),
                TOTAL_EXPECTED_DEMAND = Math.Round(rows.Sum(x => x.EXPECTED_DEMAND), 4),
                TOTAL_EXPECTED_COST = rows.Sum(x => x.EXPECTED_COST),
                Rows = rows
            };
        }

        private static double GetReplacementRatio(int severity)
        {
            switch (severity)
            {
                case 5: return 1.0d;
                case 4: return 0.5d;
                case 3: return 0.2d;
                case 2: return 0.05d;
                case 1: return 0.01d;
                default: return 0d;
            }
        }

        private static int GetAssetTypeId(string equipmentKey)
        {
            switch ((equipmentKey ?? "").Trim().ToUpperInvariant())
            {
                case "VCB": return 1;
                case "DCCB": return 2;
                case "SUBMODULE": return 3;
                case "DCCABLE": return 4;
                case "ITR": return 5;
                case "CONVERTER": return 6;
                case "CIRCUIT_BREAKER": return 7;
                case "CABLE": return 8;
                case "SWITCHGEAR": return 9;
                case "PROTECTION_RELAY": return 10;
                case "COOLING_SYSTEM": return 11;
                case "ENERGY_STORAGE": return 12;
                case "SCADA": return 13;
                default: return 0;
            }
        }

        private static string GetAssetTypeName(int assetTypeId)
        {
            switch (assetTypeId)
            {
                case 1: return "VCB";
                case 2: return "DCCB";
                case 3: return "SUBMODULE";
                case 4: return "DCCABLE";
                case 5: return "INTERFACETR";
                case 6: return "CONVERTER";
                case 7: return "CIRCUIT BREAKER";
                case 8: return "CABLE";
                case 9: return "SWITCHGEAR";
                case 10: return "PROTECTION RELAY";
                case 11: return "COOLING SYSTEM";
                case 12: return "ENERGY STORAGE";
                case 13: return "SCADA";
                default: return "UNKNOWN";
            }
        }

        private static double Clamp(double value, double minimum, double maximum)
        {
            return Math.Max(minimum, Math.Min(maximum, value));
        }
    }
}
