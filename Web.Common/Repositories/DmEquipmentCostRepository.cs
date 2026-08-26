using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Web.Common
{
    public sealed class DmEquipmentCostRepository
    {
        public IDictionary<string, double> GetActiveReplacementCosts()
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var rows = dbHelper.Conn.Query<DmEquipmentCostInfo>(@"
                    SELECT
                        TBL_IDX,
                        EQUIPMENT_KEY,
                        REPLACEMENT_COST,
                        IS_ACTIVE,
                        UPDATED_AT,
                        TBL_GETDATE
                    FROM TB_DM_EQUIPMENT_COST
                    WHERE ISNULL(IS_ACTIVE, 1) = 1
                    ORDER BY ISNULL(UPDATED_AT, TBL_GETDATE) DESC, TBL_IDX DESC;")
                    .ToList();

                return rows
                    .Where(x => !string.IsNullOrWhiteSpace(x.EQUIPMENT_KEY))
                    .GroupBy(
                        x => x.EQUIPMENT_KEY.Trim().ToUpperInvariant(),
                        StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(
                        group => group.Key,
                        group => Convert.ToDouble(group.First().REPLACEMENT_COST),
                        StringComparer.OrdinalIgnoreCase);
            }
        }
    }
}
