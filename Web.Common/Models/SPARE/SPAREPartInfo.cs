using System;

namespace Web.Common
{
    public class SPAREPartInfo
    {
        public int TBL_IDX { get; set; }
        public int SPARE_ID { get; set; }
        public string PART_NUMBER { get; set; }
        public string PART_NAME { get; set; }
        public string CRITICALITY_GRADE { get; set; }
        public int? UNIT_PRICE { get; set; }
        public int? LEAD_TIME_DAYS { get; set; }
        public bool? IS_ACTIVE { get; set; }
        public DateTime? CREATED_AT { get; set; }
        public DateTime? UPDATED_AT { get; set; }
        public DateTime? TBL_GETDATE { get; set; }
    }
}