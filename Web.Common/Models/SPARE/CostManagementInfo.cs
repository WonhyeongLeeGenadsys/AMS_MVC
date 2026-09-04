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

    // ─────────────────────────────────────────────────────────────────────
    // 예비품 목록 화면 전용 DTO.
    //
    // 기존에는 이 조회들이 Dapper의 dynamic(DapperRow)을 그대로 반환했는데,
    // ASP.NET MVC의 기본 JsonResult(JavaScriptSerializer)는 DapperRow를
    // 객체가 아니라 KeyValuePair 열거형으로 직렬화한다. 그 결과 화면에서는
    // 행 개수만 맞고 모든 셀이 비어 보이며, cellTemplate의 options.data.XXX가
    // undefined가 되어 '수정' 링크가 ?costId=undefined 로 깨진다.
    // 반환 타입을 명시하면 Dapper가 프로퍼티로 매핑하므로 정상 직렬화된다.
    // ─────────────────────────────────────────────────────────────────────

    public class SpareCostListItemInfo
    {
        public int COST_ID { get; set; }
        public int SPARE_ID { get; set; }
        public string PART_NUMBER { get; set; }
        public string PART_NAME { get; set; }
        public string CRITICALITY_GRADE { get; set; }
        public int? FISCAL_YEAR { get; set; }
        public decimal? BUDGET_AMOUNT { get; set; }
        public decimal? ACTUAL_AMOUNT { get; set; }
        public DateTime? UPDATED_AT { get; set; }
    }

    public class SpareProcurementListItemInfo
    {
        public int PROC_ID { get; set; }
        public int SPARE_ID { get; set; }
        public string PART_NUMBER { get; set; }
        public string PART_NAME { get; set; }
        public string CRITICALITY_GRADE { get; set; }
        public int? ORDER_QTY { get; set; }
        public decimal? UNIT_COST { get; set; }
        public decimal? TOTAL_PROC_COST { get; set; }
        public DateTime? ORDER_DATE { get; set; }
        public string STATUS { get; set; }
        public string SUPPLIER { get; set; }
    }

    public class SpareBasicListItemInfo
    {
        public int SPARE_ID { get; set; }
        public string PART_NUMBER { get; set; }
        public string PART_NAME { get; set; }
        public string CRITICALITY_GRADE { get; set; }
        public decimal? UNIT_PRICE { get; set; }
        public int? LEAD_TIME_DAYS { get; set; }
        public int CURRENT_QTY { get; set; }
        public bool? IS_ACTIVE { get; set; }
        public DateTime? CREATED_AT { get; set; }
    }
}