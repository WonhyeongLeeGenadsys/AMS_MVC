using System.Collections.Generic;

namespace Web.Common
{
    public sealed class SpareAssetCostRow
    {
        public string ASSET_TYPE_NAME { get; set; }
        public decimal TOTAL_COST { get; set; }
    }

    public sealed class SpareProcurementPlanRow
    {
        public int FISCAL_YEAR { get; set; }
        public int YEAR_NO { get; set; }
        public int SPARE_ID { get; set; }
        public string PART_NUMBER { get; set; }
        public string PART_NAME { get; set; }
        public string ASSET_TYPE_NAME { get; set; }
        public string ASSET_CODES { get; set; }
        public string CRITICALITY_GRADE { get; set; }
        public int AFFECTED_ASSET_COUNT { get; set; }
        public double MIN_RUL_YEARS { get; set; }
        public double EXPECTED_DEMAND { get; set; }
        public int ORDER_QTY { get; set; }
        public decimal UNIT_PRICE { get; set; }
        public decimal ORDER_COST { get; set; }
    }

    public sealed class SpareProcurementPlanYearSummary
    {
        public int FISCAL_YEAR { get; set; }
        public decimal CRITICAL { get; set; }
        public decimal HIGH { get; set; }
        public decimal MEDIUM { get; set; }
        public decimal LOW { get; set; }
        public decimal TOTAL_COST { get; set; }
        public int TOTAL_ORDER_QTY { get; set; }
        public int PART_COUNT { get; set; }
    }

    public sealed class SpareProcurementPlanResult
    {
        public SpareProcurementPlanResult()
        {
            Rows = new List<SpareProcurementPlanRow>();
            YearlyBudgetRows = new List<SpareProcurementPlanYearSummary>();
        }

        public int BASE_YEAR { get; set; }
        public int TOTAL_PART_COUNT { get; set; }
        public int TOTAL_ORDER_QTY { get; set; }
        public decimal TOTAL_ORDER_COST { get; set; }
        public int EXCLUDED_ASSET_COUNT { get; set; }
        public List<SpareProcurementPlanRow> Rows { get; set; }
        public List<SpareProcurementPlanYearSummary> YearlyBudgetRows { get; set; }
    }
}
