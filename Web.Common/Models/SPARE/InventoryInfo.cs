using System;

namespace Web.Common
{
    public class InventoryInfo
    {
        public int TBL_IDX { get; set; }
        public int INV_ID { get; set; }
        public int SPARE_ID { get; set; }
        public int? CURRENT_QTY { get; set; }
        public int? SAFETY_STOCK { get; set; }
        public int? EOQ { get; set; }
        public int? REORDER_POINT { get; set; }
        public DateTime? LAST_UPDATED { get; set; }
        public DateTime? TBL_GETDATE { get; set; }
    }
}