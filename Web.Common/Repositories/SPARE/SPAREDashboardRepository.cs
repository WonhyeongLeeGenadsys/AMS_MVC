using Dapper;
using System.Collections.Generic;
using System.Linq;

namespace Web.Common
{
    public class SPAREDashboardRepository
    {
        private class SpareOverviewKpiDto
        {
            public int TOTAL_COUNT { get; set; }
            public decimal TOTAL_BUDGET { get; set; }
            public decimal INVENTORY_VALUE { get; set; }
            public double AVG_LEAD_TIME { get; set; }
            public int CRITICAL_COUNT { get; set; }
            public int CRITICAL_SHORTAGE_COUNT { get; set; }
        }

        private class SpareCriticalityDto
        {
            public string CRITICALITY_GRADE { get; set; }
            public int CNT { get; set; }
        }

        private class SpareAssetCostDto
        {
            public string ASSET_TYPE_NAME { get; set; }
            public decimal TOTAL_COST { get; set; }
        }

        private class SpareShortageDto
        {
            public string PART_NAME { get; set; }
            public int SHORTAGE_QTY { get; set; }
        }

        private class SpareScatterDto
        {
            public string PART_NAME { get; set; }
            public int LEAD_TIME_DAYS { get; set; }
            public int CURRENT_QTY { get; set; }
        }

        private class SpareOverviewRowDto
        {
            public int SPARE_ID { get; set; }
            public string PART_NUMBER { get; set; }
            public string PART_NAME { get; set; }
            public string CRITICALITY_GRADE { get; set; }
            public int UNIT_PRICE { get; set; }
            public int LEAD_TIME_DAYS { get; set; }
            public int CURRENT_QTY { get; set; }
            public int SAFETY_STOCK { get; set; }
        }

        public object GetSpareOverviewDataRepo(int? assetTypeId, string criticality)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var param = new
                {
                    AssetTypeId = assetTypeId,
                    Criticality = string.IsNullOrWhiteSpace(criticality) ? null : criticality
                };

                var kpiSql = @"
                    SELECT
                        COUNT(*) AS TOTAL_COUNT,
                        ISNULL(SUM(ISNULL(CS.TOTAL_BUDGET, 0)), 0) AS TOTAL_BUDGET,
                        ISNULL(SUM(ISNULL(S.UNIT_PRICE, 0) * ISNULL(I.CURRENT_QTY, 0)), 0) AS INVENTORY_VALUE,
                        ISNULL(AVG(CAST(NULLIF(S.LEAD_TIME_DAYS, 0) AS FLOAT)), 0) AS AVG_LEAD_TIME,
                        SUM(CASE WHEN S.CRITICALITY_GRADE = 'A' THEN 1 ELSE 0 END) AS CRITICAL_COUNT,
                        SUM(
                            CASE
                                WHEN S.CRITICALITY_GRADE = 'A'
                                 AND ISNULL(I.SAFETY_STOCK, 0) > ISNULL(I.CURRENT_QTY, 0)
                                THEN 1 ELSE 0
                            END
                        ) AS CRITICAL_SHORTAGE_COUNT
                    FROM TB_SPARE_PART S
                    LEFT JOIN TB_INVENTORY I
                        ON S.SPARE_ID = I.SPARE_ID
                    LEFT JOIN
                    (
                        SELECT SPARE_ID, SUM(ISNULL(BUDGET_AMOUNT, 0)) AS TOTAL_BUDGET
                        FROM TB_COST_MANAGEMENT
                        GROUP BY SPARE_ID
                    ) CS
                        ON S.SPARE_ID = CS.SPARE_ID
                    WHERE (@Criticality IS NULL OR S.CRITICALITY_GRADE = @Criticality)
                      AND (
                            @AssetTypeId IS NULL
                            OR EXISTS (
                                SELECT 1
                                FROM TB_SPARE_ASSET_MAP M
                                WHERE M.SPARE_ID = S.SPARE_ID
                                  AND M.ASSET_TYPE_ID = @AssetTypeId
                            )
                      );";

                var kpi = dbHelper.Conn.QueryFirstOrDefault<SpareOverviewKpiDto>(kpiSql, param)
                          ?? new SpareOverviewKpiDto();

                var criticalitySql = @"
                    SELECT
                        S.CRITICALITY_GRADE,
                        COUNT(*) AS CNT
                    FROM TB_SPARE_PART S
                    WHERE (@Criticality IS NULL OR S.CRITICALITY_GRADE = @Criticality)
                      AND (
                            @AssetTypeId IS NULL
                            OR EXISTS (
                                SELECT 1
                                FROM TB_SPARE_ASSET_MAP M
                                WHERE M.SPARE_ID = S.SPARE_ID
                                  AND M.ASSET_TYPE_ID = @AssetTypeId
                            )
                      )
                    GROUP BY S.CRITICALITY_GRADE
                    ORDER BY S.CRITICALITY_GRADE;";

                var criticalityData = dbHelper.Conn.Query<SpareCriticalityDto>(criticalitySql, param).ToList();

                var assetCostSql = @"
                    SELECT
                        CASE M.ASSET_TYPE_ID
                            WHEN 1 THEN 'VCB'
                            WHEN 2 THEN 'DCCB'
                            WHEN 3 THEN 'SUBMODULE'
                            WHEN 4 THEN 'DCCABLE'
                            WHEN 5 THEN 'INTERFACETR'
                            ELSE 'UNKNOWN'
                        END AS ASSET_TYPE_NAME,
                        SUM(ISNULL(S.UNIT_PRICE, 0) * ISNULL(I.CURRENT_QTY, 0)) AS TOTAL_COST
                    FROM TB_SPARE_PART S
                    LEFT JOIN TB_INVENTORY I
                        ON S.SPARE_ID = I.SPARE_ID
                    LEFT JOIN TB_SPARE_ASSET_MAP M
                        ON S.SPARE_ID = M.SPARE_ID
                    WHERE (@Criticality IS NULL OR S.CRITICALITY_GRADE = @Criticality)
                      AND (@AssetTypeId IS NULL OR M.ASSET_TYPE_ID = @AssetTypeId)
                        
                    GROUP BY M.ASSET_TYPE_ID
                    ORDER BY TOTAL_COST DESC;";

                var assetCostData = dbHelper.Conn.Query<SpareAssetCostDto>(assetCostSql, param).ToList();

                var shortageSql = @"
                    SELECT
                        S.PART_NAME,
                        (ISNULL(I.SAFETY_STOCK, 0) - ISNULL(I.CURRENT_QTY, 0)) AS SHORTAGE_QTY
                    FROM TB_SPARE_PART S
                    LEFT JOIN TB_INVENTORY I
                        ON S.SPARE_ID = I.SPARE_ID
                    WHERE ISNULL(I.SAFETY_STOCK, 0) > ISNULL(I.CURRENT_QTY, 0)
                      AND (@Criticality IS NULL OR S.CRITICALITY_GRADE = @Criticality)
                      AND (
                            @AssetTypeId IS NULL
                            OR EXISTS (
                                SELECT 1
                                FROM TB_SPARE_ASSET_MAP M
                                WHERE M.SPARE_ID = S.SPARE_ID
                                  AND M.ASSET_TYPE_ID = @AssetTypeId
                            )
                      )
                    ORDER BY SHORTAGE_QTY DESC, S.PART_NAME;";

                var shortageData = dbHelper.Conn.Query<SpareShortageDto>(shortageSql, param).ToList();

                var scatterSql = @"
                    SELECT
                        S.PART_NAME,
                        ISNULL(S.LEAD_TIME_DAYS, 0) AS LEAD_TIME_DAYS,
                        ISNULL(I.CURRENT_QTY, 0) AS CURRENT_QTY
                    FROM TB_SPARE_PART S
                    LEFT JOIN TB_INVENTORY I
                        ON S.SPARE_ID = I.SPARE_ID
                    WHERE ISNULL(S.LEAD_TIME_DAYS, 0) > 0
                      AND (@Criticality IS NULL OR S.CRITICALITY_GRADE = @Criticality)
                      AND (
                            @AssetTypeId IS NULL
                            OR EXISTS (
                                SELECT 1
                                FROM TB_SPARE_ASSET_MAP M
                                WHERE M.SPARE_ID = S.SPARE_ID
                                  AND M.ASSET_TYPE_ID = @AssetTypeId
                            )
                      )
                    ORDER BY S.PART_NAME;";

                var scatterData = dbHelper.Conn.Query<ScareScatterDtoFix>(scatterSql, param)
                    .Select(x => new SpareScatterDto
                    {
                        PART_NAME = x.PART_NAME,
                        LEAD_TIME_DAYS = x.LEAD_TIME_DAYS,
                        CURRENT_QTY = x.CURRENT_QTY
                    }).ToList();

                var rowsSql = @"
                    SELECT
                        S.SPARE_ID,
                        S.PART_NUMBER,
                        S.PART_NAME,
                        S.CRITICALITY_GRADE,
                        ISNULL(S.UNIT_PRICE, 0) AS UNIT_PRICE,
                        ISNULL(S.LEAD_TIME_DAYS, 0) AS LEAD_TIME_DAYS,
                        ISNULL(I.CURRENT_QTY, 0) AS CURRENT_QTY,
                        ISNULL(I.SAFETY_STOCK, 0) AS SAFETY_STOCK
                    FROM TB_SPARE_PART S
                    LEFT JOIN TB_INVENTORY I
                        ON S.SPARE_ID = I.SPARE_ID
                    WHERE (@Criticality IS NULL OR S.CRITICALITY_GRADE = @Criticality)
                      AND (
                            @AssetTypeId IS NULL
                            OR EXISTS (
                                SELECT 1
                                FROM TB_SPARE_ASSET_MAP M
                                WHERE M.SPARE_ID = S.SPARE_ID
                                  AND M.ASSET_TYPE_ID = @AssetTypeId
                            )
                      )
                    ORDER BY S.TBL_IDX DESC;";

                var rows = dbHelper.Conn.Query<SpareOverviewRowDto>(rowsSql, param).ToList();

                return new
                {
                    success = true,
                    kpi,
                    criticalityData,
                    assetCostData,
                    shortageData,
                    scatterData,
                    rows
                };
            }
        }

        private class ScareScatterDtoFix
        {
            public string PART_NAME { get; set; }
            public int LEAD_TIME_DAYS { get; set; }
            public int CURRENT_QTY { get; set; }
        }

        public object GetSpareInventoryStatusDataRepo(int? assetTypeId, string criticality)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var param = new
                {
                    AssetTypeId = assetTypeId,
                    Criticality = string.IsNullOrWhiteSpace(criticality) ? null : criticality
                };

                var sql = @"
            SELECT
                S.SPARE_ID,
                S.PART_NUMBER,
                S.PART_NAME,
                CASE M1.ASSET_TYPE_ID
                    WHEN 1 THEN 'VCB'
                    WHEN 2 THEN 'DCCB'
                    WHEN 3 THEN 'SUBMODULE'
                    WHEN 4 THEN 'DCCABLE'
                    WHEN 5 THEN 'Interface TR'
                    ELSE 'UNKNOWN'
                END AS ASSET_TYPE_NAME,
                S.CRITICALITY_GRADE,
                ISNULL(I.CURRENT_QTY, 0) AS CURRENT_QTY,
                ISNULL(I.SAFETY_STOCK, 0) AS SAFETY_STOCK,
                ISNULL(I.REORDER_POINT, 0) AS ROP,
                ISNULL(S.UNIT_PRICE, 0) AS UNIT_PRICE
            FROM TB_SPARE_PART S
            LEFT JOIN TB_INVENTORY I
                ON S.SPARE_ID = I.SPARE_ID
            LEFT JOIN
            (
                SELECT SPARE_ID, MIN(ASSET_TYPE_ID) AS ASSET_TYPE_ID
                FROM TB_SPARE_ASSET_MAP
                GROUP BY SPARE_ID
            ) M1
                ON S.SPARE_ID = M1.SPARE_ID
            WHERE (@Criticality IS NULL OR S.CRITICALITY_GRADE = @Criticality)
              AND (
                    @AssetTypeId IS NULL
                    OR EXISTS (
                        SELECT 1
                        FROM TB_SPARE_ASSET_MAP M
                        WHERE M.SPARE_ID = S.SPARE_ID
                          AND M.ASSET_TYPE_ID = @AssetTypeId
                    )
              )
            ORDER BY S.TBL_IDX DESC;";

                var rows = dbHelper.Conn.Query(sql, param)
                    .Select(x =>
                    {
                        int currentQty = (int)(x.CURRENT_QTY ?? 0);
                        int safetyStock = (int)(x.SAFETY_STOCK ?? 0);
                        int rop = (int)(x.ROP ?? 0);
                        int unitPrice = (int)(x.UNIT_PRICE ?? 0);

                        string stockStatus;
                        if (currentQty <= 0 || currentQty < safetyStock)
                            stockStatus = "부족";
                        else if (currentQty == rop)
                            stockStatus = "ROP 도달";
                        else if (currentQty < rop)
                            stockStatus = "주의";
                        else
                            stockStatus = "정상";

                        int stockAmount = currentQty * unitPrice;
                        int stockBarMax = System.Math.Max(System.Math.Max(safetyStock, rop), currentQty);
                        if (stockBarMax <= 0) stockBarMax = 1;

                        return new
                        {
                            SPARE_ID = (int)x.SPARE_ID,
                            PART_NUMBER = (string)x.PART_NUMBER,
                            PART_NAME = (string)x.PART_NAME,
                            ASSET_TYPE_NAME = (string)x.ASSET_TYPE_NAME,
                            CRITICALITY_GRADE = (string)x.CRITICALITY_GRADE,
                            CURRENT_QTY = currentQty,
                            SAFETY_STOCK = safetyStock,
                            ROP = rop,
                            STOCK_STATUS = stockStatus,
                            STOCK_AMOUNT = stockAmount,
                            BAR_CURRENT = currentQty,
                            BAR_MAX = stockBarMax
                        };
                    })
                    .ToList();

                var kpi = new
                {
                    SHORTAGE_COUNT = rows.Count(x => x.STOCK_STATUS == "부족"),
                    REORDER_RECOMMEND_COUNT = rows.Count(x => x.CURRENT_QTY <= x.ROP),
                    TOTAL_STOCK_COUNT = rows.Sum(x => x.CURRENT_QTY),
                    CRITICAL_ALERT_COUNT = rows.Count(x => x.CRITICALITY_GRADE == "A" && x.CURRENT_QTY <= 2)
                };

                return new
                {
                    success = true,
                    kpi,
                    rows
                };
            }
        }

        public object GetSpareProcurementStatusDataRepo(int? assetTypeId, string status)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var param = new
                {
                    AssetTypeId = assetTypeId,
                    Status = string.IsNullOrWhiteSpace(status) ? null : status
                };

                var topCostSql = @"
            SELECT TOP 10
                S.PART_NAME,
                SUM(ISNULL(P.TOTAL_PROC_COST, 0)) AS TOTAL_PROC_COST
            FROM TB_PROCUREMENT P
            INNER JOIN TB_SPARE_PART S
                ON P.SPARE_ID = S.SPARE_ID
            WHERE (@Status IS NULL OR P.STATUS = @Status)
              AND (
                    @AssetTypeId IS NULL
                    OR EXISTS (
                        SELECT 1
                        FROM TB_SPARE_ASSET_MAP M
                        WHERE M.SPARE_ID = S.SPARE_ID
                          AND M.ASSET_TYPE_ID = @AssetTypeId
                    )
              )
            GROUP BY S.PART_NAME
            ORDER BY SUM(ISNULL(P.TOTAL_PROC_COST, 0)) DESC;";

                var topCostRows = dbHelper.Conn.Query(topCostSql, param)
                    .Select(x => new
                    {
                        PART_NAME = (string)x.PART_NAME,
                        TOTAL_PROC_COST = (decimal)(x.TOTAL_PROC_COST ?? 0)
                    })
                    .ToList();

                var detailSql = @"
            SELECT
                P.PROC_ID,
                P.SPARE_ID,
                S.PART_NAME,
                S.PART_NUMBER,
                S.CRITICALITY_GRADE,
                P.ORDER_QTY,
                P.UNIT_COST,
                P.TOTAL_PROC_COST,
                P.ORDER_DATE,
                P.STATUS,
                P.SUPPLIER
            FROM TB_PROCUREMENT P
            INNER JOIN TB_SPARE_PART S
                ON P.SPARE_ID = S.SPARE_ID
            WHERE (@Status IS NULL OR P.STATUS = @Status)
              AND (
                    @AssetTypeId IS NULL
                    OR EXISTS (
                        SELECT 1
                        FROM TB_SPARE_ASSET_MAP M
                        WHERE M.SPARE_ID = S.SPARE_ID
                          AND M.ASSET_TYPE_ID = @AssetTypeId
                    )
              )
            ORDER BY P.ORDER_DATE DESC, P.PROC_ID DESC;";

                var rows = dbHelper.Conn.Query(detailSql, param)
                    .Select(x => new
                    {
                        PROC_ID = (int)x.PROC_ID,
                        SPARE_ID = (int)x.SPARE_ID,
                        PART_NAME = (string)x.PART_NAME,
                        PART_NUMBER = (string)x.PART_NUMBER,
                        CRITICALITY_GRADE = (string)x.CRITICALITY_GRADE,
                        ORDER_QTY = (int?)(x.ORDER_QTY) ?? 0,
                        UNIT_COST = (decimal?)(x.UNIT_COST) ?? 0,
                        TOTAL_PROC_COST = (decimal?)(x.TOTAL_PROC_COST) ?? 0,
                        ORDER_DATE = x.ORDER_DATE == null ? "" : ((System.DateTime)x.ORDER_DATE).ToString("yyyy-MM-dd"),
                        STATUS = (string)x.STATUS,
                        SUPPLIER = (string)x.SUPPLIER
                    })
                    .ToList();

                var kpi = new
                {
                    TOTAL_PROC_COST = rows.Sum(x => x.TOTAL_PROC_COST),
                    TOTAL_ORDER_QTY = rows.Sum(x => x.ORDER_QTY),
                    ORDER_COUNT = rows.Count,
                    NORMAL_COUNT = rows.Count(x => x.STATUS == "정상")
                };

                return new
                {
                    success = true,
                    kpi,
                    topCostRows,
                    rows
                };
            }
        }
        public object GetSpareCostPlanDataRepo(int? assetTypeId, int? fiscalYear)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var param = new
                {
                    AssetTypeId = assetTypeId,
                    FiscalYear = fiscalYear
                };

                // 1. 설비 유형별 예비품 비용
                var assetCostSql = @"
            SELECT
                CASE M.ASSET_TYPE_ID
                    WHEN 1 THEN 'VCB'
                    WHEN 2 THEN 'DCCB'
                    WHEN 3 THEN 'SubModule'
                    WHEN 4 THEN 'DCCABLE'
                    WHEN 5 THEN 'Interface TR'
                    ELSE 'UNKNOWN'
                END AS ASSET_TYPE_NAME,
                SUM(ISNULL(S.UNIT_PRICE, 0) * ISNULL(I.CURRENT_QTY, 0)) AS TOTAL_COST
            FROM TB_SPARE_PART S
            LEFT JOIN TB_INVENTORY I
                ON S.SPARE_ID = I.SPARE_ID
            LEFT JOIN TB_SPARE_ASSET_MAP M
                ON S.SPARE_ID = M.SPARE_ID
            WHERE (@AssetTypeId IS NULL OR M.ASSET_TYPE_ID = @AssetTypeId)
            GROUP BY M.ASSET_TYPE_ID
            ORDER BY TOTAL_COST DESC;";

                var assetCostRows = dbHelper.Conn.Query(assetCostSql, param)
                    .Select(x => new
                    {
                        ASSET_TYPE_NAME = (string)x.ASSET_TYPE_NAME,
                        TOTAL_COST = (decimal)(x.TOTAL_COST ?? 0)
                    })
                    .ToList();

                // 2. 3개년 조달 예산
                var yearlyBudgetSql = @"
            SELECT
                C.FISCAL_YEAR,
                S.CRITICALITY_GRADE,
                SUM(ISNULL(C.BUDGET_AMOUNT, 0)) AS BUDGET_AMOUNT
            FROM TB_COST_MANAGEMENT C
            INNER JOIN TB_SPARE_PART S
                ON C.SPARE_ID = S.SPARE_ID
            WHERE (@FiscalYear IS NULL OR C.FISCAL_YEAR BETWEEN @FiscalYear - 2 AND @FiscalYear)
              AND (
                    @AssetTypeId IS NULL
                    OR EXISTS (
                        SELECT 1
                        FROM TB_SPARE_ASSET_MAP M
                        WHERE M.SPARE_ID = S.SPARE_ID
                          AND M.ASSET_TYPE_ID = @AssetTypeId
                    )
              )
            GROUP BY C.FISCAL_YEAR, S.CRITICALITY_GRADE
            ORDER BY C.FISCAL_YEAR, S.CRITICALITY_GRADE;";

                var budgetRaw = dbHelper.Conn.Query(yearlyBudgetSql, param)
                    .Select(x => new
                    {
                        FISCAL_YEAR = (int)x.FISCAL_YEAR,
                        CRITICALITY_GRADE = (string)x.CRITICALITY_GRADE,
                        BUDGET_AMOUNT = (decimal)(x.BUDGET_AMOUNT ?? 0)
                    })
                    .ToList();

                var yearlyBudgetRows = budgetRaw
                    .GroupBy(x => x.FISCAL_YEAR)
                    .Select(g => new
                    {
                        FISCAL_YEAR = g.Key,
                        A = g.Where(x => x.CRITICALITY_GRADE == "A").Sum(x => x.BUDGET_AMOUNT),
                        B = g.Where(x => x.CRITICALITY_GRADE == "B").Sum(x => x.BUDGET_AMOUNT),
                        C = g.Where(x => x.CRITICALITY_GRADE == "C").Sum(x => x.BUDGET_AMOUNT),
                        D = g.Where(x => x.CRITICALITY_GRADE == "D").Sum(x => x.BUDGET_AMOUNT)
                    })
                    .OrderBy(x => x.FISCAL_YEAR)
                    .ToList();

                return new
                {
                    success = true,
                    assetCostRows,
                    yearlyBudgetRows
                };
            }
        }

        public object GetSparePolicyDataRepo(int? assetTypeId, string policyType)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var param = new
                {
                    AssetTypeId = assetTypeId
                };

                var sql = @"
            SELECT
                S.SPARE_ID,
                S.PART_NAME,
                S.PART_NUMBER,
                S.CRITICALITY_GRADE,
                ISNULL(S.UNIT_PRICE, 0) AS UNIT_PRICE,
                ISNULL(I.CURRENT_QTY, 0) AS CURRENT_QTY,
                ISNULL(I.SAFETY_STOCK, 0) AS SAFETY_STOCK,
                ISNULL(I.EOQ, 0) AS EOQ,
                ISNULL(I.REORDER_POINT, 0) AS ROP,
                CASE M1.ASSET_TYPE_ID
                    WHEN 1 THEN 'VCB'
                    WHEN 2 THEN 'DCCB'
                    WHEN 3 THEN 'SUBMODULE'
                    WHEN 4 THEN 'DCCABLE'
                    WHEN 5 THEN 'Interface TR'
                    ELSE 'UNKNOWN'
                END AS ASSET_TYPE_NAME
            FROM TB_SPARE_PART S
            LEFT JOIN TB_INVENTORY I
                ON S.SPARE_ID = I.SPARE_ID
            LEFT JOIN
            (
                SELECT SPARE_ID, MIN(ASSET_TYPE_ID) AS ASSET_TYPE_ID
                FROM TB_SPARE_ASSET_MAP
                GROUP BY SPARE_ID
            ) M1
                ON S.SPARE_ID = M1.SPARE_ID
            WHERE (
                    @AssetTypeId IS NULL
                    OR EXISTS (
                        SELECT 1
                        FROM TB_SPARE_ASSET_MAP M
                        WHERE M.SPARE_ID = S.SPARE_ID
                          AND M.ASSET_TYPE_ID = @AssetTypeId
                    )
                  )
            ORDER BY S.TBL_IDX DESC;";

                var rows = dbHelper.Conn.Query(sql, param)
                    .Select(x =>
                    {
                        int currentQty = (int)(x.CURRENT_QTY ?? 0);
                        int safetyStock = (int)(x.SAFETY_STOCK ?? 0);
                        int eoq = (int)(x.EOQ ?? 0);
                        int rop = (int)(x.ROP ?? 0);
                        int unitPrice = (int)(x.UNIT_PRICE ?? 0);

                        int min = safetyStock > 0 ? System.Math.Max(1, safetyStock - 1) : 1;
                        int max = eoq > 0 ? safetyStock + eoq : System.Math.Max(safetyStock + 5, 5);

                        string stockPolicy;
                        if (currentQty <= rop && rop > 0)
                            stockPolicy = "ROP";
                        else if (eoq >= safetyStock * 2 && eoq > 0)
                            stockPolicy = "Min-Max";
                        else
                            stockPolicy = "EOQ";

                        return new
                        {
                            SPARE_ID = (int)x.SPARE_ID,
                            PART_NAME = (string)x.PART_NAME,
                            PART_NUMBER = (string)x.PART_NUMBER,
                            ASSET_TYPE_NAME = (string)x.ASSET_TYPE_NAME,
                            CRITICALITY_GRADE = (string)x.CRITICALITY_GRADE,
                            EOQ = eoq,
                            SAFETY_STOCK = safetyStock,
                            ROP = rop,
                            MIN = min,
                            MAX = max,
                            STOCK_POLICY = stockPolicy,
                            STOCK_AMOUNT = currentQty * unitPrice
                        };
                    })
                    .ToList();

                if (!string.IsNullOrWhiteSpace(policyType) && policyType != "ALL")
                {
                    rows = rows.Where(x => x.STOCK_POLICY == policyType).ToList();
                }

                var topRows = rows
                    .OrderByDescending(x => x.STOCK_AMOUNT)
                    .Take(20)
                    .ToList();

                return new
                {
                    success = true,
                    rows = topRows
                };
            }
        }
    }
}