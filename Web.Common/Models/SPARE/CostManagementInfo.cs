using System;

namespace Web.Common
{
    public class CostManagementInfo
    {
        public int TBL_IDX { get; set; }
        public int COST_ID { get; set; }
        public int SPARE_ID { get; set; }
        public int? FISCAL_YEAR { get; set; }
        public decimal? BUDGET_AMOUNT { get; set; }
        public decimal? ACTUAL_AMOUNT { get; set; }
        public DateTime? UPDATED_AT { get; set; }
        public DateTime? TBL_GETDATE { get; set; }
    }
}