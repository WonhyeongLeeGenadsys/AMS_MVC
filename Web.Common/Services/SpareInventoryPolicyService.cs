using System;
using System.Collections.Generic;
using System.Linq;

namespace Web.Common
{
    /// <summary>
    /// 원본 DM 모듈의 EOQ, 안전재고, 재주문점 계산식을 적용한다.
    /// </summary>
    public sealed class SpareInventoryPolicyService
    {
        private const double OrderingCost = 500000d;
        private const double HoldingCostRate = 0.2d;
        private const double ZScore = 1.65d;
        private const double DemandVariability = 0.3d;

        public List<SpareInventoryPolicyCalculation> Calculate(
            IEnumerable<SpareDemandForecastRow> forecastRows)
        {
            return (forecastRows ?? Enumerable.Empty<SpareDemandForecastRow>())
                .Select(CalculateRow)
                .ToList();
        }

        private static SpareInventoryPolicyCalculation CalculateRow(
            SpareDemandForecastRow row)
        {
            double annualDemand = Math.Max(0d, row.EXPECTED_DEMAND);
            double unitPrice = Math.Max(0d, Convert.ToDouble(row.UNIT_PRICE));
            double holdingCost = unitPrice * HoldingCostRate;
            double eoq = annualDemand > 0d && holdingCost > 0d
                ? Math.Sqrt((2d * annualDemand * OrderingCost) / holdingCost)
                : 0d;

            double dailyDemand = annualDemand / 365d;
            double demandStandardDeviation = dailyDemand * DemandVariability;
            int leadTimeDays = Math.Max(0, row.LEAD_TIME_DAYS);
            double safetyStock = demandStandardDeviation > 0d && leadTimeDays > 0
                ? ZScore
                    * demandStandardDeviation
                    * Math.Sqrt(leadTimeDays)
                    * GetLeadTimeWeight(row.CRITICALITY_GRADE)
                : 0d;
            double reorderPoint = (dailyDemand * leadTimeDays) + safetyStock;
            double minStock;
            double maxStock;
            string policyType;

            switch ((row.CRITICALITY_GRADE ?? "").Trim().ToUpperInvariant())
            {
                case "CRITICAL":
                    minStock = Math.Max(eoq, 2d);
                    maxStock = minStock + eoq;
                    policyType = "FIXED";
                    break;
                case "HIGH":
                    minStock = safetyStock;
                    maxStock = reorderPoint + eoq;
                    policyType = "DYNAMIC";
                    break;
                case "MEDIUM":
                    minStock = safetyStock * 0.5d;
                    maxStock = reorderPoint + (eoq * 0.5d);
                    policyType = "DYNAMIC";
                    break;
                default:
                    minStock = 0d;
                    maxStock = eoq;
                    policyType = "JIT";
                    break;
            }

            return new SpareInventoryPolicyCalculation
            {
                SPARE_ID = row.SPARE_ID,
                ANNUAL_DEMAND = Math.Round(annualDemand, 4),
                EOQ = CeilingToInt(eoq),
                SAFETY_STOCK = CeilingToInt(safetyStock),
                REORDER_POINT = CeilingToInt(reorderPoint),
                MIN_STOCK = CeilingToInt(minStock),
                MAX_STOCK = CeilingToInt(maxStock),
                POLICY_TYPE = policyType
            };
        }

        private static double GetLeadTimeWeight(string criticality)
        {
            switch ((criticality ?? "").Trim().ToUpperInvariant())
            {
                case "CRITICAL": return 1.5d;
                case "HIGH": return 1.2d;
                case "MEDIUM": return 1.0d;
                case "LOW": return 0.8d;
                default: return 1.0d;
            }
        }

        private static int CeilingToInt(double value)
        {
            if (value <= 0d || double.IsNaN(value))
            {
                return 0;
            }

            return value >= int.MaxValue
                ? int.MaxValue
                : (int)Math.Ceiling(value);
        }
    }
}
