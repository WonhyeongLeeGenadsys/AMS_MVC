using System;

namespace Web.Common
{
    public class SpareAssetMapInfo
    {
        public int TBL_IDX { get; set; }
        public int SPARE_ASSET_MAP_ID { get; set; }
        public int SPARE_ID { get; set; }
        public int ASSET_TYPE_ID { get; set; }
        public DateTime? CREATED_AT { get; set; }
        public DateTime? TBL_GETDATE { get; set; }
    }
}