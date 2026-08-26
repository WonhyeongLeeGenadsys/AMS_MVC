using System;

namespace Web.Common
{
    public sealed class DmEquipmentCostInfo
    {
        public int TBL_IDX { get; set; }
        public string EQUIPMENT_KEY { get; set; }
        public decimal REPLACEMENT_COST { get; set; }
        public bool? IS_ACTIVE { get; set; }
        public DateTime? UPDATED_AT { get; set; }
        public DateTime TBL_GETDATE { get; set; }
    }
}
