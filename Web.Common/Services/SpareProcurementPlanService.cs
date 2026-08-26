using System;
using System.Collections.Generic;
using System.Linq;

namespace Web.Common
{
    /// <summary>
    /// 장비 RUL에 따라 예비품 수요를 1~3차년도 조달계획으로 배정한다.
    /// </summary>
    public sealed class SpareProcurementPlanService
    {
        private sealed class DemandDetail
        {
            public int FISCAL_YEAR { get; set; }
            public int YEAR_NO { get; set; }
            public SpareDemandInput Part { get; set; }
            public string ASSET_CODE { get; set; }
            public double RUL_YEARS { get; set; }
            public double ADJUSTED_DEMAND { get; set; }
        }

        public SpareProcurementPlanResult Calculate(
            IEnumerable<DmDecisionInfo> decisions,
            IEnumerable<SpareDemandInput> demandInputs,
            int baseYear)
        {
            var validDecisions = (decisions ?? Enumerable.Empty<DmDecisionInfo>())
                .Where(x => x.Severity > 0 && x.RULYears.HasValue)
                .ToList();
            var inputs = (demandInputs ?? Enumerable.Empty<SpareDemandInput>()).ToList();
            var details = new List<DemandDetail>();

            foreach (var map in inputs)
            {
                var mappedDecisions = validDecisions
                    .Where(x => GetAssetTypeId(x.EquipmentKey) == map.ASSET_TYPE_ID)
                    .ToList();

                foreach (var decision in mappedDecisions)
                {
                    double rulYears = Math.Max(0d, decision.RULYears.GetValueOrDefault());
                    int yearNo = GetProcurementYear(rulYears);
                    if (yearNo > 3)
                    {
                        continue;
                    }

                    int requiredQty = map.REQUIRED_QTY > 0 ? map.REQUIRED_QTY : 1;
                    double replacementRatio = GetReplacementRatio(decision.Severity);
                    double pof = Clamp(decision.PoFRatio, 0d, 1d);
                    double safetyFactor = decision.Severity >= 4
                        ? 1d + (Math.Max(0, map.LEAD_TIME_DAYS) / 365d)
                        : 1d;
                    double adjustedDemand = requiredQty
                        * replacementRatio
                        * pof
                        * safetyFactor;

                    if (adjustedDemand <= 0d)
                    {
                        continue;
                    }

                    details.Add(new DemandDetail
                    {
                        FISCAL_YEAR = baseYear + yearNo - 1,
                        YEAR_NO = yearNo,
                        Part = map,
                        ASSET_CODE = decision.Code,
                        RUL_YEARS = rulYears,
                        ADJUSTED_DEMAND = adjustedDemand
                    });
                }
            }

            var rows = details
                .GroupBy(x => new { x.FISCAL_YEAR, x.YEAR_NO, x.Part.SPARE_ID })
                .Select(group =>
                {
                    var first = group.First();
                    double expectedDemand = group.Sum(x => x.ADJUSTED_DEMAND);
                    int orderQty = (int)Math.Ceiling(expectedDemand);
                    var assetCodes = group
                        .Select(x => x.ASSET_CODE)
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .OrderBy(x => x)
                        .ToList();
                    var assetTypeNames = group
                        .Select(x => GetAssetTypeName(x.Part.ASSET_TYPE_ID))
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .OrderBy(x => x)
                        .ToList();

                    return new SpareProcurementPlanRow
                    {
                        FISCAL_YEAR = group.Key.FISCAL_YEAR,
                        YEAR_NO = group.Key.YEAR_NO,
                        SPARE_ID = group.Key.SPARE_ID,
                        PART_NUMBER = first.Part.PART_NUMBER,
                        PART_NAME = first.Part.PART_NAME,
                        ASSET_TYPE_NAME = string.Join(", ", assetTypeNames),
                        ASSET_CODES = string.Join(", ", assetCodes),
                        CRITICALITY_GRADE = first.Part.CRITICALITY_GRADE,
                        AFFECTED_ASSET_COUNT = assetCodes.Count,
                        MIN_RUL_YEARS = Math.Round(group.Min(x => x.RUL_YEARS), 2),
                        EXPECTED_DEMAND = Math.Round(expectedDemand, 4),
                        ORDER_QTY = orderQty,
                        UNIT_PRICE = first.Part.UNIT_PRICE,
                        ORDER_COST = orderQty * first.Part.UNIT_PRICE
                    };
                })
                .OrderBy(x => x.FISCAL_YEAR)
                .ThenByDescending(x => x.ORDER_COST)
                .ThenBy(x => x.PART_NUMBER)
                .ToList();

            var yearlyBudgetRows = Enumerable.Range(0, 3)
                .Select(offset =>
                {
                    int year = baseYear + offset;
                    var yearRows = rows.Where(x => x.FISCAL_YEAR == year).ToList();
                    return new SpareProcurementPlanYearSummary
                    {
                        FISCAL_YEAR = year,
                        CRITICAL = SumCost(yearRows, "CRITICAL"),
                        HIGH = SumCost(yearRows, "HIGH"),
                        MEDIUM = SumCost(yearRows, "MEDIUM"),
                        LOW = SumCost(yearRows, "LOW"),
                        TOTAL_COST = yearRows.Sum(x => x.ORDER_COST),
                        TOTAL_ORDER_QTY = yearRows.Sum(x => x.ORDER_QTY),
                        PART_COUNT = yearRows.Count
                    };
                })
                .ToList();

            int excludedAssetCount = validDecisions
                .Where(x => x.RULYears.GetValueOrDefault() >= 3d)
                .Select(x => x.Code)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count();

            return new SpareProcurementPlanResult
            {
                BASE_YEAR = baseYear,
                TOTAL_PART_COUNT = rows.Select(x => x.SPARE_ID).Distinct().Count(),
                TOTAL_ORDER_QTY = rows.Sum(x => x.ORDER_QTY),
                TOTAL_ORDER_COST = rows.Sum(x => x.ORDER_COST),
                EXCLUDED_ASSET_COUNT = excludedAssetCount,
                Rows = rows,
                YearlyBudgetRows = yearlyBudgetRows
            };
        }

        private static decimal SumCost(
            IEnumerable<SpareProcurementPlanRow> rows,
            string criticality)
        {
            return rows
                .Where(x => string.Equals(
                    x.CRITICALITY_GRADE,
                    criticality,
                    StringComparison.OrdinalIgnoreCase))
                .Sum(x => x.ORDER_COST);
        }

        private static int GetProcurementYear(double rulYears)
        {
            if (rulYears < 1d) return 1;
            if (rulYears < 2d) return 2;
            if (rulYears < 3d) return 3;
            return 4;
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
