using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;

namespace Web.Common
{
    /// <summary>
    /// 목록 화면 전용 경량 조회 Repository.
    /// HI/PoF 계산과 상세 화면에서 사용하는 기존 전체 조회는 변경하지 않는다.
    /// </summary>
    public class CheckListSummaryRepository
    {
        private sealed class CheckTable
        {
            public string CheckTableName { get; set; }
            public string BasicTableName { get; set; }
            public string CodeColumn { get; set; }
            public string FieldPrefix { get; set; }
        }

        private static readonly IDictionary<string, CheckTable> Tables =
            new Dictionary<string, CheckTable>(StringComparer.OrdinalIgnoreCase)
            {
                ["VCB"] = Create("VCB_CHK", "VCB_BASICINFO", "VCB_CODE", "CHK"),
                ["ITR1"] = Create("ITR_CHK1", "ITR_BASICINFO", "ITR_CODE", "CHK1"),
                ["ITR2"] = Create("ITR_CHK2", "ITR_BASICINFO", "ITR_CODE", "CHK2"),
                ["SUBMODULE"] = Create("SUBMODULE_CHK", "SUBMODULE_BASICINFO", "SUBMODULE_CODE", "CHK"),
                ["DCCB"] = Create("DCCB_CHK", "DCCB_BASICINFO", "DCCB_CODE", "CHK"),
                ["DCCABLE"] = Create("DCCABLE_CHK", "DCCABLE_BASICINFO", "DCCABLE_CODE", "CHK")
            };

        public Result GetList(
            string equipmentType,
            string equipmentCode,
            out List<CheckListSummaryInfo> items)
        {
            items = new List<CheckListSummaryInfo>();
            var result = new Result();

            if (!Tables.TryGetValue(equipmentType ?? string.Empty, out var table))
            {
                result.IsSuccess = false;
                result.Message = $"지원하지 않는 점검 장비 유형입니다: {equipmentType}";
                return result;
            }

            try
            {
                var prefix = table.FieldPrefix;
                var where = string.IsNullOrWhiteSpace(equipmentCode)
                    ? string.Empty
                    : $"WHERE c.[{table.CodeColumn}] = @EquipmentCode";

                var sql = $@"
SELECT
    c.[TBL_IDX] AS Tbl_Idx,
    c.[{table.CodeColumn}] AS EquipmentCode,
    ISNULL(b.[NAME], '') AS Name,
    ISNULL(b.[SERIAL_NO], '') AS Serial_No,
    c.[{prefix}_GONGSA_NAME] AS GongsaName,
    c.[{prefix}_WEATHER] AS Weather,
    c.[{prefix}_TEMP] AS Temp,
    c.[{prefix}_HUM] AS Hum,
    c.[{prefix}_COMPANY] AS Company,
    c.[{prefix}_WORKER] AS Worker,
    c.[{prefix}_MANAGER] AS Manager,
    c.[{prefix}_URGENT_NO] AS UrgentNo,
    c.[{prefix}_TYPE] AS CheckType,
    c.[{prefix}_START_DATE] AS StartDate,
    c.[{prefix}_END_DATE] AS EndDate
FROM [{table.CheckTableName}] c
LEFT JOIN [{table.BasicTableName}] b
    ON b.[{table.CodeColumn}] = c.[{table.CodeColumn}]
{where}
ORDER BY c.[TBL_IDX] DESC;";

                using (var db = new DBHelper())
                {
                    items = db.Conn.Query<CheckListSummaryInfo>(
                        sql,
                        new { EquipmentCode = equipmentCode }).AsList();
                }

                result.IsSuccess = true;
                result.Message = $"{equipmentType} 점검 목록 조회 성공: {items.Count}건";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"{equipmentType} 점검 목록 조회 실패: {ex.Message}";
            }

            return result;
        }

        private static CheckTable Create(
            string checkTableName,
            string basicTableName,
            string codeColumn,
            string fieldPrefix)
        {
            return new CheckTable
            {
                CheckTableName = checkTableName,
                BasicTableName = basicTableName,
                CodeColumn = codeColumn,
                FieldPrefix = fieldPrefix
            };
        }
    }
}
