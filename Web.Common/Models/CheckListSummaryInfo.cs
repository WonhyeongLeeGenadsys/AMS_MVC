using System;

namespace Web.Common
{
    /// <summary>
    /// 점검 목록 화면에서 실제로 사용하는 공통 필드만 담는다.
    /// 상세 점검값은 기존 장비별 Repository가 계속 담당한다.
    /// </summary>
    public class CheckListSummaryInfo
    {
        public int Tbl_Idx { get; set; }
        public string EquipmentCode { get; set; }
        public string Name { get; set; }
        public string Serial_No { get; set; }
        public string GongsaName { get; set; }
        public string Weather { get; set; }
        public string Temp { get; set; }
        public string Hum { get; set; }
        public string Company { get; set; }
        public string Worker { get; set; }
        public string Manager { get; set; }
        public string UrgentNo { get; set; }
        public string CheckType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
