using System;

namespace Web.Common
{
    public class ProcurementInfo
    {
        public int TBL_IDX { get; set; }
        public int PROC_ID { get; set; }
        public int SPARE_ID { get; set; }
        public int? ORDER_QTY { get; set; }
        public decimal? UNIT_COST { get; set; }
        public decimal? TOTAL_PROC_COST { get; set; }
        public DateTime? ORDER_DATE { get; set; }
        public string STATUS { get; set; }
        public string SUPPLIER { get; set; }
        public DateTime? TBL_GETDATE { get; set; }
    }
}