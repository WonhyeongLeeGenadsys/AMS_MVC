using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Web.Common
{
    public class SPAREBasicInfoRepository
    {
        // 예비품 전체 조회
        public Result GetAllSPAREBasicInfoRepo(out List<SPAREPartInfo> spareParts)
        {
            Result res = new Result(true);
            spareParts = new List<SPAREPartInfo>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = @"
                        SELECT
                            TBL_IDX,
                            SPARE_ID,
                            PART_NUMBER,
                            PART_NAME,
                            CRITICALITY_GRADE,
                            UNIT_PRICE,
                            LEAD_TIME_DAYS,
                            IS_ACTIVE,
                            CREATED_AT,
                            UPDATED_AT,
                            TBL_GETDATE
                        FROM TB_SPARE_PART
                        ORDER BY SPARE_ID DESC";

                    spareParts = dbHelper.Conn.Query<SPAREPartInfo>(query).AsList();
                    res.Message = "GetAllSPAREBasicInfoRepo 성공";
                    LogHelper.WriteLog("DB(TB_SPARE_PART)", res.Message);
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "GetAllSPAREBasicInfoRepo 실패: " + ex.Message;
                LogHelper.WriteLog("DB(TB_SPARE_PART)", res.Message + " / " + ex.StackTrace);
            }

            return res;
        }

        // 예비품 상세 조회
        public SPAREPartInfo GetSPAREPartBySPAREIdRepo(int spareId)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var query = @"
                    SELECT
                        TBL_IDX,
                        SPARE_ID,
                        PART_NUMBER,
                        PART_NAME,
                        CRITICALITY_GRADE,
                        UNIT_PRICE,
                        LEAD_TIME_DAYS,
                        IS_ACTIVE,
                        CREATED_AT,
                        UPDATED_AT,
                        TBL_GETDATE
                    FROM TB_SPARE_PART
                    WHERE SPARE_ID = @SPARE_ID";

                return dbHelper.Conn.QueryFirstOrDefault<SPAREPartInfo>(query, new { SPARE_ID = spareId });
            }
        }

        // 재고 조회
        public Result GetInventoryBySPAREIdRepo(int spareId, out InventoryInfo inventory)
        {
            Result res = new Result(true);
            inventory = null;

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = @"
                        SELECT
                            TBL_IDX,
                            INV_ID,
                            SPARE_ID,
                            CURRENT_QTY,
                            SAFETY_STOCK,
                            EOQ,
                            REORDER_POINT,
                            LAST_UPDATED,
                            TBL_GETDATE
                        FROM TB_INVENTORY
                        WHERE SPARE_ID = @SPARE_ID";

                    inventory = dbHelper.Conn.QueryFirstOrDefault<InventoryInfo>(query, new { SPARE_ID = spareId });
                    res.Message = "GetInventoryBySPAREIdRepo 성공";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "GetInventoryBySPAREIdRepo 실패: " + ex.Message;
                LogHelper.WriteLog("DB(TB_INVENTORY)", res.Message + " / " + ex.StackTrace);
            }

            return res;
        }

        // 설비타입ID 목록 조회
        public Result GetAssetTypeIdsBySPAREIdRepo(int spareId, out List<int> assetTypeIds)
        {
            Result res = new Result(true);
            assetTypeIds = new List<int>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = @"
                        SELECT ASSET_TYPE_ID
                        FROM TB_SPARE_ASSET_MAP
                        WHERE SPARE_ID = @SPARE_ID
                        ORDER BY ASSET_TYPE_ID";

                    assetTypeIds = dbHelper.Conn.Query<int>(query, new { SPARE_ID = spareId }).AsList();
                    res.Message = "GetAssetTypeIdsBySPAREIdRepo 성공";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "GetAssetTypeIdsBySPAREIdRepo 실패: " + ex.Message;
                LogHelper.WriteLog("DB(TB_SPARE_ASSET_MAP)", res.Message + " / " + ex.StackTrace);
            }

            return res;
        }

        // 발주 조회
        public Result GetProcurementBySPAREIdRepo(int spareId, out List<ProcurementInfo> procurements)
        {
            Result res = new Result(true);
            procurements = new List<ProcurementInfo>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = @"
                        SELECT
                            TBL_IDX,
                            PROC_ID,
                            SPARE_ID,
                            ORDER_QTY,
                            UNIT_COST,
                            TOTAL_PROC_COST,
                            ORDER_DATE,
                            STATUS,
                            SUPPLIER,
                            TBL_GETDATE
                        FROM TB_PROCUREMENT
                        WHERE SPARE_ID = @SPARE_ID
                        ORDER BY ORDER_DATE DESC, PROC_ID DESC";

                    procurements = dbHelper.Conn.Query<ProcurementInfo>(query, new { SPARE_ID = spareId }).AsList();
                    res.Message = "GetProcurementBySPAREIdRepo 성공";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "GetProcurementBySPAREIdRepo 실패: " + ex.Message;
                LogHelper.WriteLog("DB(TB_PROCUREMENT)", res.Message + " / " + ex.StackTrace);
            }

            return res;
        }

        // 비용관리 조회
        public Result GetCostManagementBySPAREIdRepo(int spareId, out List<CostManagementInfo> costs)
        {
            Result res = new Result(true);
            costs = new List<CostManagementInfo>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = @"
                        SELECT
                            TBL_IDX,
                            COST_ID,
                            SPARE_ID,
                            FISCAL_YEAR,
                            BUDGET_AMOUNT,
                            ACTUAL_AMOUNT,
                            UPDATED_AT,
                            TBL_GETDATE
                        FROM TB_COST_MANAGEMENT
                        WHERE SPARE_ID = @SPARE_ID
                        ORDER BY FISCAL_YEAR DESC, COST_ID DESC";

                    costs = dbHelper.Conn.Query<CostManagementInfo>(query, new { SPARE_ID = spareId }).AsList();
                    res.Message = "GetCostManagementBySPAREIdRepo 성공";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "GetCostManagementBySPAREIdRepo 실패: " + ex.Message;
                LogHelper.WriteLog("DB(TB_COST_MANAGEMENT)", res.Message + " / " + ex.StackTrace);
            }

            return res;
        }

        // 예비품 등록
        public Result CreateSPAREBasicInfoRepo(
            SPAREPartInfo sparePart,
            InventoryInfo inventory,
            List<int> assetTypeIds,
            ProcurementInfo procurement,
            CostManagementInfo cost)
        {
            Result res = new Result(true);

            using (DBHelper dbHelper = new DBHelper())
            using (var conn = dbHelper.Conn)
            using (var transaction = conn.BeginTransaction())
            {
                try
                {
                    // 1. TB_SPARE_PART
                    var spareIdQuery = "SELECT ISNULL(MAX(SPARE_ID), 0) + 1 FROM TB_SPARE_PART";
                    int newSpareId = conn.ExecuteScalar<int>(spareIdQuery, transaction: transaction);

                    var insertSpareQuery = @"
                        INSERT INTO TB_SPARE_PART
                        (
                            SPARE_ID,
                            PART_NUMBER,
                            PART_NAME,
                            CRITICALITY_GRADE,
                            UNIT_PRICE,
                            LEAD_TIME_DAYS,
                            IS_ACTIVE,
                            CREATED_AT,
                            UPDATED_AT
                        )
                        VALUES
                        (
                            @SPARE_ID,
                            @PART_NUMBER,
                            @PART_NAME,
                            @CRITICALITY_GRADE,
                            @UNIT_PRICE,
                            @LEAD_TIME_DAYS,
                            @IS_ACTIVE,
                            GETDATE(),
                            NULL
                        )";

                    conn.Execute(insertSpareQuery, new
                    {
                        SPARE_ID = newSpareId,
                        PART_NUMBER = sparePart.PART_NUMBER,
                        PART_NAME = sparePart.PART_NAME,
                        CRITICALITY_GRADE = sparePart.CRITICALITY_GRADE,
                        UNIT_PRICE = sparePart.UNIT_PRICE,
                        LEAD_TIME_DAYS = sparePart.LEAD_TIME_DAYS,
                        IS_ACTIVE = sparePart.IS_ACTIVE
                    }, transaction);

                    // 2. TB_INVENTORY
                    var invIdQuery = "SELECT ISNULL(MAX(INV_ID), 0) + 1 FROM TB_INVENTORY";
                    int newInvId = conn.ExecuteScalar<int>(invIdQuery, transaction: transaction);

                    var insertInventoryQuery = @"
                        INSERT INTO TB_INVENTORY
                        (
                            INV_ID,
                            SPARE_ID,
                            CURRENT_QTY,
                            SAFETY_STOCK,
                            EOQ,
                            REORDER_POINT,
                            LAST_UPDATED
                        )
                        VALUES
                        (
                            @INV_ID,
                            @SPARE_ID,
                            @CURRENT_QTY,
                            @SAFETY_STOCK,
                            @EOQ,
                            @REORDER_POINT,
                            GETDATE()
                        )";

                    conn.Execute(insertInventoryQuery, new
                    {
                        INV_ID = newInvId,
                        SPARE_ID = newSpareId,
                        CURRENT_QTY = inventory.CURRENT_QTY,
                        SAFETY_STOCK = inventory.SAFETY_STOCK,
                        EOQ = inventory.EOQ,
                        REORDER_POINT = inventory.REORDER_POINT
                    }, transaction);

                    // 3. TB_SPARE_ASSET_MAP
                    if (assetTypeIds != null && assetTypeIds.Count > 0)
                    {
                        var mapIdQuery = "SELECT ISNULL(MAX(SPARE_ASSET_MAP_ID), 0) + 1 FROM TB_SPARE_ASSET_MAP";
                        int nextMapId = conn.ExecuteScalar<int>(mapIdQuery, transaction: transaction);

                        var insertMapQuery = @"
                            INSERT INTO TB_SPARE_ASSET_MAP
                            (
                                SPARE_ASSET_MAP_ID,
                                SPARE_ID,
                                ASSET_TYPE_ID,
                                CREATED_AT
                            )
                            VALUES
                            (
                                @SPARE_ASSET_MAP_ID,
                                @SPARE_ID,
                                @ASSET_TYPE_ID,
                                GETDATE()
                            )";

                        foreach (var assetTypeId in assetTypeIds.Distinct())
                        {
                            conn.Execute(insertMapQuery, new
                            {
                                SPARE_ASSET_MAP_ID = nextMapId,
                                SPARE_ID = newSpareId,
                                ASSET_TYPE_ID = assetTypeId
                            }, transaction);

                            nextMapId++;
                        }
                    }

                    // 4. TB_PROCUREMENT
                    if (procurement != null &&
                        (procurement.ORDER_QTY.HasValue ||
                         procurement.UNIT_COST.HasValue ||
                         procurement.ORDER_DATE.HasValue ||
                         !string.IsNullOrWhiteSpace(procurement.STATUS) ||
                         !string.IsNullOrWhiteSpace(procurement.SUPPLIER)))
                    {
                        var procIdQuery = "SELECT ISNULL(MAX(PROC_ID), 0) + 1 FROM TB_PROCUREMENT";
                        int newProcId = conn.ExecuteScalar<int>(procIdQuery, transaction: transaction);

                        decimal? totalProcCost = null;
                        if (procurement.ORDER_QTY.HasValue && procurement.UNIT_COST.HasValue)
                        {
                            totalProcCost = procurement.ORDER_QTY.Value * procurement.UNIT_COST.Value;
                        }

                        var insertProcQuery = @"
                            INSERT INTO TB_PROCUREMENT
                            (
                                PROC_ID,
                                SPARE_ID,
                                ORDER_QTY,
                                UNIT_COST,
                                TOTAL_PROC_COST,
                                ORDER_DATE,
                                STATUS,
                                SUPPLIER
                            )
                            VALUES
                            (
                                @PROC_ID,
                                @SPARE_ID,
                                @ORDER_QTY,
                                @UNIT_COST,
                                @TOTAL_PROC_COST,
                                @ORDER_DATE,
                                @STATUS,
                                @SUPPLIER
                            )";

                        conn.Execute(insertProcQuery, new
                        {
                            PROC_ID = newProcId,
                            SPARE_ID = newSpareId,
                            ORDER_QTY = procurement.ORDER_QTY,
                            UNIT_COST = procurement.UNIT_COST,
                            TOTAL_PROC_COST = totalProcCost,
                            ORDER_DATE = procurement.ORDER_DATE,
                            STATUS = procurement.STATUS,
                            SUPPLIER = procurement.SUPPLIER
                        }, transaction);
                    }

                    // 5. TB_COST_MANAGEMENT
                    if (cost != null &&
                        (cost.FISCAL_YEAR.HasValue ||
                         cost.BUDGET_AMOUNT.HasValue ||
                         cost.ACTUAL_AMOUNT.HasValue))
                    {
                        var costIdQuery = "SELECT ISNULL(MAX(COST_ID), 0) + 1 FROM TB_COST_MANAGEMENT";
                        int newCostId = conn.ExecuteScalar<int>(costIdQuery, transaction: transaction);

                        var insertCostQuery = @"
                            INSERT INTO TB_COST_MANAGEMENT
                            (
                                COST_ID,
                                SPARE_ID,
                                FISCAL_YEAR,
                                BUDGET_AMOUNT,
                                ACTUAL_AMOUNT,
                                UPDATED_AT
                            )
                            VALUES
                            (
                                @COST_ID,
                                @SPARE_ID,
                                @FISCAL_YEAR,
                                @BUDGET_AMOUNT,
                                @ACTUAL_AMOUNT,
                                GETDATE()
                            )";

                        conn.Execute(insertCostQuery, new
                        {
                            COST_ID = newCostId,
                            SPARE_ID = newSpareId,
                            FISCAL_YEAR = cost.FISCAL_YEAR,
                            BUDGET_AMOUNT = cost.BUDGET_AMOUNT,
                            ACTUAL_AMOUNT = cost.ACTUAL_AMOUNT
                        }, transaction);
                    }

                    transaction.Commit();
                    res.Message = "CreateSPAREBasicInfoRepo 성공: PART_NUMBER=" + sparePart.PART_NUMBER;
                    LogHelper.WriteLog("DB(TB_SPARE_PART)", res.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    res.IsSuccess = false;
                    res.Message = "CreateSPAREBasicInfoRepo 실패: " + ex.Message;
                    LogHelper.WriteLog("DB(TB_SPARE_PART)", res.Message + " / " + ex.StackTrace);
                }
            }

            return res;
        }

        // 예비품 수정
        public Result UpdateSPAREBasicInfoRepo(
            SPAREPartInfo sparePart,
            InventoryInfo inventory,
            List<int> assetTypeIds)
        {
            Result res = new Result(true);

            using (DBHelper dbHelper = new DBHelper())
            using (var conn = dbHelper.Conn)
            using (var transaction = conn.BeginTransaction())
            {
                try
                {
                    var updateSpareQuery = @"
                        UPDATE TB_SPARE_PART
                        SET
                            PART_NUMBER = @PART_NUMBER,
                            PART_NAME = @PART_NAME,
                            CRITICALITY_GRADE = @CRITICALITY_GRADE,
                            UNIT_PRICE = @UNIT_PRICE,
                            LEAD_TIME_DAYS = @LEAD_TIME_DAYS,
                            IS_ACTIVE = @IS_ACTIVE,
                            UPDATED_AT = GETDATE()
                        WHERE SPARE_ID = @SPARE_ID";

                    int affectedRowsSpare = conn.Execute(updateSpareQuery, sparePart, transaction);
                    if (affectedRowsSpare <= 0)
                        throw new Exception("TB_SPARE_PART 수정 실패");

                    var updateInventoryQuery = @"
                        UPDATE TB_INVENTORY
                        SET
                            CURRENT_QTY = @CURRENT_QTY,
                            SAFETY_STOCK = @SAFETY_STOCK,
                            EOQ = @EOQ,
                            REORDER_POINT = @REORDER_POINT,
                            LAST_UPDATED = GETDATE()
                        WHERE SPARE_ID = @SPARE_ID";

                    int affectedRowsInventory = conn.Execute(updateInventoryQuery, new
                    {
                        SPARE_ID = sparePart.SPARE_ID,
                        CURRENT_QTY = inventory.CURRENT_QTY,
                        SAFETY_STOCK = inventory.SAFETY_STOCK,
                        EOQ = inventory.EOQ,
                        REORDER_POINT = inventory.REORDER_POINT
                    }, transaction);

                    if (affectedRowsInventory <= 0)
                        throw new Exception("TB_INVENTORY 수정 실패");

                    conn.Execute("DELETE FROM TB_SPARE_ASSET_MAP WHERE SPARE_ID = @SPARE_ID",
                        new { SPARE_ID = sparePart.SPARE_ID }, transaction);

                    if (assetTypeIds != null && assetTypeIds.Count > 0)
                    {
                        var mapIdQuery = "SELECT ISNULL(MAX(SPARE_ASSET_MAP_ID), 0) + 1 FROM TB_SPARE_ASSET_MAP";
                        int nextMapId = conn.ExecuteScalar<int>(mapIdQuery, transaction: transaction);

                        var insertMapQuery = @"
                            INSERT INTO TB_SPARE_ASSET_MAP
                            (
                                SPARE_ASSET_MAP_ID,
                                SPARE_ID,
                                ASSET_TYPE_ID,
                                CREATED_AT
                            )
                            VALUES
                            (
                                @SPARE_ASSET_MAP_ID,
                                @SPARE_ID,
                                @ASSET_TYPE_ID,
                                GETDATE()
                            )";

                        foreach (var assetTypeId in assetTypeIds.Distinct())
                        {
                            conn.Execute(insertMapQuery, new
                            {
                                SPARE_ASSET_MAP_ID = nextMapId,
                                SPARE_ID = sparePart.SPARE_ID,
                                ASSET_TYPE_ID = assetTypeId
                            }, transaction);

                            nextMapId++;
                        }
                    }

                    transaction.Commit();
                    res.Message = "UpdateSPAREBasicInfoRepo 성공: SPARE_ID=" + sparePart.SPARE_ID;
                    LogHelper.WriteLog("DB(TB_SPARE_PART)", res.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    res.IsSuccess = false;
                    res.Message = "UpdateSPAREBasicInfoRepo 실패: " + ex.Message;
                    LogHelper.WriteLog("DB(TB_SPARE_PART)", res.Message + " / " + ex.StackTrace);
                }
            }

            return res;
        }

        // 삭제
        public Result DeleteSPAREBasicInfoRepo(int spareId)
        {
            Result res = new Result(true);

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                using (var conn = dbHelper.Conn)
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        conn.Execute("DELETE FROM TB_COST_MANAGEMENT WHERE SPARE_ID = @SPARE_ID",
                            new { SPARE_ID = spareId }, transaction);

                        conn.Execute("DELETE FROM TB_PROCUREMENT WHERE SPARE_ID = @SPARE_ID",
                            new { SPARE_ID = spareId }, transaction);

                        conn.Execute("DELETE FROM TB_SPARE_ASSET_MAP WHERE SPARE_ID = @SPARE_ID",
                            new { SPARE_ID = spareId }, transaction);

                        conn.Execute("DELETE FROM TB_INVENTORY WHERE SPARE_ID = @SPARE_ID",
                            new { SPARE_ID = spareId }, transaction);

                        int affectedRows = conn.Execute("DELETE FROM TB_SPARE_PART WHERE SPARE_ID = @SPARE_ID",
                            new { SPARE_ID = spareId }, transaction);

                        if (affectedRows <= 0)
                            throw new Exception("삭제 대상 예비품을 찾을 수 없습니다.");

                        transaction.Commit();
                        res.Message = "DeleteSPAREBasicInfoRepo 성공: SPARE_ID=" + spareId;
                        LogHelper.WriteLog("DB(TB_SPARE_PART)", res.Message);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        res.IsSuccess = false;
                        res.Message = "DeleteSPAREBasicInfoRepo 실패: " + ex.Message;
                        LogHelper.WriteLog("DB(TB_SPARE_PART)", res.Message + " / " + ex.StackTrace);
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "DeleteSPAREBasicInfoRepo 오류: " + ex.Message;
                LogHelper.WriteLog("DB(TB_SPARE_PART)", res.Message + " / " + ex.StackTrace);
            }

            return res;
        }
    }
}