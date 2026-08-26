using System.Collections.Generic;

namespace Web.Common
{
    public sealed class SpareDemandInput
    {
        public int SPARE_ID { get; set; }
        public string PART_NUMBER { get; set; }
        public string PART_NAME { get; set; }
        public string CRITICALITY_GRADE { get; set; }
        public decimal UNIT_PRICE { get; set; }
        public int LEAD_TIME_DAYS { get; set; }
        public int CURRENT_QTY { get; set; }
        public int ASSET_TYPE_ID { get; set; }
        public int REQUIRED_QTY { get; set; }
    }

    public sealed class SpareDemandForecastRow
    {
        public int SPARE_ID { get; set; }
        public string PART_NUMBER { get; set; }
        public string PART_NAME { get; set; }
        public string ASSET_TYPE_NAME { get; set; }
        public string CRITICALITY_GRADE { get; set; }
        public decimal UNIT_PRICE { get; set; }
        public int LEAD_TIME_DAYS { get; set; }
        public int CURRENT_QTY { get; set; }
        public int AFFECTED_ASSET_COUNT { get; set; }
        public string TOP_DECISION { get; set; }
        public double EXPECTED_DEMAND { get; set; }
        public int RECOMMENDED_QTY { get; set; }
        public int SHORTAGE_QTY { get; set; }
        public decimal EXPECTED_COST { get; set; }
    }

    public sealed class SpareInventoryPolicyCalculation
    {
        public int SPARE_ID { get; set; }
        public double ANNUAL_DEMAND { get; set; }
        public int EOQ { get; set; }
        public int SAFETY_STOCK { get; set; }
        public int REORDER_POINT { get; set; }
        public int MIN_STOCK { get; set; }
        public int MAX_STOCK { get; set; }
        public string POLICY_TYPE { get; set; }
    }

    public sealed class SpareDemandForecastResult
    {
        public SpareDemandForecastResult()
        {
            Rows = new List<SpareDemandForecastRow>();
        }

        public int TOTAL_PART_COUNT { get; set; }
        public int FORECAST_PART_COUNT { get; set; }
        public int SHORTAGE_PART_COUNT { get; set; }
        public int TOTAL_RECOMMENDED_QTY { get; set; }
        public double TOTAL_EXPECTED_DEMAND { get; set; }
        public decimal TOTAL_EXPECTED_COST { get; set; }
        public List<SpareDemandForecastRow> Rows { get; set; }
    }
}
