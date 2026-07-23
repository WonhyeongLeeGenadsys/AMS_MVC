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
        public Result GetInventoryListRepo(out List<dynamic> rows)
        {
            Result res = new Result(true);
            rows = new List<dynamic>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = @"
                SELECT
                    S.SPARE_ID,
                    S.PART_NUMBER,
                    S.PART_NAME,
                    S.CRITICALITY_GRADE,
                    ISNULL(I.CURRENT_QTY, 0) AS CURRENT_QTY,
                    ISNULL(I.SAFETY_STOCK, 0) AS SAFETY_STOCK,
                    ISNULL(I.EOQ, 0) AS EOQ,
                    ISNULL(I.REORDER_POINT, 0) AS REORDER_POINT,
                    I.LAST_UPDATED
                FROM TB_SPARE_PART S
                LEFT JOIN TB_INVENTORY I
                    ON S.SPARE_ID = I.SPARE_ID
                ORDER BY S.SPARE_ID DESC";

                    rows = dbHelper.Conn.Query(query).ToList<dynamic>();
                    res.Message = "재고 목록 조회 성공";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "재고 목록 조회 실패: " + ex.Message;
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

        public Result GetProcurementListRepo(out List<dynamic> rows)
        {
            Result res = new Result(true);
            rows = new List<dynamic>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = @"
                SELECT
                    P.PROC_ID,
                    P.SPARE_ID,
                    S.PART_NUMBER,
                    S.PART_NAME,
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
                ORDER BY P.ORDER_DATE DESC, P.PROC_ID DESC";

                    rows = dbHelper.Conn.Query(query).ToList<dynamic>();
                    res.Message = "발주 목록 조회 성공";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "발주 목록 조회 실패: " + ex.Message;
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

        public Result SaveProcurementRepo(ProcurementInfo model)
        {
            Result res = new Result(true);

            using (DBHelper dbHelper = new DBHelper())
            using (var conn = dbHelper.Conn)
            using (var tx = conn.BeginTransaction())
            {
                try
                {
                    decimal? totalProcCost = null;
                    if (model.ORDER_QTY.HasValue && model.UNIT_COST.HasValue)
                        totalProcCost = model.ORDER_QTY.Value * model.UNIT_COST.Value;

                    conn.Execute(@"
                INSERT INTO TB_PROCUREMENT
                (
                    PROC_ID, SPARE_ID, ORDER_QTY, UNIT_COST, TOTAL_PROC_COST,
                    ORDER_DATE, STATUS, SUPPLIER
                )
                VALUES
                (
                    NEXT VALUE FOR dbo.SEQ_PROC_ID, @SPARE_ID, @ORDER_QTY, @UNIT_COST, @TOTAL_PROC_COST,
                    @ORDER_DATE, @STATUS, @SUPPLIER
                )",
                        new
                        {
                            model.SPARE_ID,
                            model.ORDER_QTY,
                            model.UNIT_COST,
                            TOTAL_PROC_COST = totalProcCost,
                            model.ORDER_DATE,
                            model.STATUS,
                            model.SUPPLIER
                        }, tx);

                    tx.Commit();
                    res.Message = "발주 등록 성공";
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    res.IsSuccess = false;
                    res.Message = "발주 등록 실패: " + ex.Message;
                }
            }

            return res;
        }

        public Result GetCostListRepo(out List<dynamic> rows)
        {
            Result res = new Result(true);
            rows = new List<dynamic>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = @"
                SELECT
                    C.COST_ID,
                    C.SPARE_ID,
                    S.PART_NUMBER,
                    S.PART_NAME,
                    S.CRITICALITY_GRADE,
                    C.FISCAL_YEAR,
                    C.BUDGET_AMOUNT,
                    C.ACTUAL_AMOUNT,
                    C.UPDATED_AT
                FROM TB_COST_MANAGEMENT C
                INNER JOIN TB_SPARE_PART S
                    ON C.SPARE_ID = S.SPARE_ID
                ORDER BY C.FISCAL_YEAR DESC, C.COST_ID DESC";

                    rows = dbHelper.Conn.Query(query).ToList<dynamic>();
                    res.Message = "비용계획 목록 조회 성공";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "비용계획 목록 조회 실패: " + ex.Message;
            }

            return res;
        }

        public Result SaveCostRepo(CostManagementInfo model)
        {
            Result res = new Result(true);

            using (DBHelper dbHelper = new DBHelper())
            using (var conn = dbHelper.Conn)
            using (var tx = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute(@"
                INSERT INTO TB_COST_MANAGEMENT
                (
                    COST_ID, SPARE_ID, FISCAL_YEAR,
                    BUDGET_AMOUNT, ACTUAL_AMOUNT, UPDATED_AT
                )
                VALUES
                (
                    NEXT VALUE FOR dbo.SEQ_COST_ID, @SPARE_ID, @FISCAL_YEAR,
                    @BUDGET_AMOUNT, @ACTUAL_AMOUNT, GETDATE()
                )",
                        model, tx);

                    tx.Commit();
                    res.Message = "비용계획 등록 성공";
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    res.IsSuccess = false;
                    res.Message = "비용계획 등록 실패: " + ex.Message;
                }
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
            List<int> assetTypeIds,
            InventoryInfo initialInventory)
        {
            Result res = new Result(true);

            using (DBHelper dbHelper = new DBHelper())
            using (var conn = dbHelper.Conn)
            using (var tx = conn.BeginTransaction())
            {
                try
                {
                    int dupCount = conn.ExecuteScalar<int>(@"
                SELECT COUNT(1)
                FROM TB_SPARE_PART
                WHERE PART_NUMBER = @PART_NUMBER",
                        new { PART_NUMBER = sparePart.PART_NUMBER }, tx);

                    if (dupCount > 0)
                        throw new Exception("이미 등록된 부품번호입니다.");

                    int newSpareId = conn.ExecuteScalar<int>(
                        "SELECT NEXT VALUE FOR dbo.SEQ_SPARE_ID",
                        transaction: tx);

                    conn.Execute(@"
                INSERT INTO TB_SPARE_PART
                (
                    SPARE_ID, PART_NUMBER, PART_NAME, CRITICALITY_GRADE,
                    UNIT_PRICE, LEAD_TIME_DAYS, IS_ACTIVE, CREATED_AT, UPDATED_AT
                )
                VALUES
                (
                    @SPARE_ID, @PART_NUMBER, @PART_NAME, @CRITICALITY_GRADE,
                    @UNIT_PRICE, @LEAD_TIME_DAYS, @IS_ACTIVE, GETDATE(), NULL
                )",
                        new
                        {
                            SPARE_ID = newSpareId,
                            PART_NUMBER = sparePart.PART_NUMBER,
                            PART_NAME = sparePart.PART_NAME,
                            CRITICALITY_GRADE = sparePart.CRITICALITY_GRADE,
                            UNIT_PRICE = sparePart.UNIT_PRICE,
                            LEAD_TIME_DAYS = sparePart.LEAD_TIME_DAYS,
                            IS_ACTIVE = sparePart.IS_ACTIVE
                        }, tx);

                    foreach (var assetTypeId in (assetTypeIds ?? new List<int>()).Distinct())
                    {
                        conn.Execute(@"
                    INSERT INTO TB_SPARE_ASSET_MAP
                    (SPARE_ASSET_MAP_ID, SPARE_ID, ASSET_TYPE_ID, CREATED_AT)
                    VALUES
                    (NEXT VALUE FOR dbo.SEQ_SPARE_ASSET_MAP_ID, @SPARE_ID, @ASSET_TYPE_ID, GETDATE())",
                            new { SPARE_ID = newSpareId, ASSET_TYPE_ID = assetTypeId }, tx);
                    }

                    conn.Execute(@"
                INSERT INTO TB_INVENTORY
                (
                    INV_ID, SPARE_ID, CURRENT_QTY, SAFETY_STOCK,
                    EOQ, REORDER_POINT, LAST_UPDATED
                )
                VALUES
                (
                    NEXT VALUE FOR dbo.SEQ_INV_ID, @SPARE_ID, @CURRENT_QTY, @SAFETY_STOCK,
                    0, 0, GETDATE()
                )",
                        new
                        {
                            SPARE_ID = newSpareId,
                            CURRENT_QTY = initialInventory?.CURRENT_QTY ?? 0,
                            SAFETY_STOCK = initialInventory?.SAFETY_STOCK ?? 0
                        }, tx);

                    tx.Commit();
                    res.Message = "기본정보 등록 성공";
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    res.IsSuccess = false;
                    res.Message = ex.Message;
                }
            }

            return res;
        }

        // 예비품 수정
        public Result UpdateSPAREBasicInfoRepo(
            SPAREPartInfo sparePart,
            List<int> assetTypeIds)
        {
            Result res = new Result(true);

            using (DBHelper dbHelper = new DBHelper())
            using (var conn = dbHelper.Conn)
            using (var transaction = conn.BeginTransaction())
            {
                try
                {
                    int dupCount = conn.ExecuteScalar<int>(@"
                SELECT COUNT(1)
                FROM TB_SPARE_PART
                WHERE PART_NUMBER = @PART_NUMBER
                  AND SPARE_ID <> @SPARE_ID",
                        new { sparePart.PART_NUMBER, sparePart.SPARE_ID }, transaction);

                    if (dupCount > 0)
                        throw new Exception("이미 등록된 부품번호입니다.");

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

                    conn.Execute(
                        "DELETE FROM TB_SPARE_ASSET_MAP WHERE SPARE_ID = @SPARE_ID",
                        new { SPARE_ID = sparePart.SPARE_ID },
                        transaction);

                    foreach (var assetTypeId in (assetTypeIds ?? new List<int>()).Distinct())
                    {
                        conn.Execute(@"
                    INSERT INTO TB_SPARE_ASSET_MAP
                    (SPARE_ASSET_MAP_ID, SPARE_ID, ASSET_TYPE_ID, CREATED_AT)
                    VALUES
                    (NEXT VALUE FOR dbo.SEQ_SPARE_ASSET_MAP_ID, @SPARE_ID, @ASSET_TYPE_ID, GETDATE())",
                            new
                            {
                                SPARE_ID = sparePart.SPARE_ID,
                                ASSET_TYPE_ID = assetTypeId
                            },
                            transaction);
                    }

                    transaction.Commit();
                    res.Message = "기본정보 수정 성공";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    res.IsSuccess = false;
                    res.Message = "기본정보 수정 실패: " + ex.Message;
                }
            }

            return res;
        }

        // 이력을 보존하기 위해 물리 삭제 대신 미사용 처리한다.
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
                        int affectedRows = conn.Execute(@"
                            UPDATE TB_SPARE_PART
                            SET IS_ACTIVE = 0,
                                UPDATED_AT = GETDATE()
                            WHERE SPARE_ID = @SPARE_ID",
                            new { SPARE_ID = spareId }, transaction);
                        
                        if (affectedRows <= 0)
                            throw new Exception("미사용 처리 대상 예비품을 찾을 수 없습니다.");

                        transaction.Commit();
                        res.Message = "예비품 미사용 처리 성공: SPARE_ID=" + spareId;
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

        //재고관리 저장
        public Result SaveInventoryRepo(InventoryInfo model)
        {
            Result res = new Result(true);

            using (DBHelper dbHelper = new DBHelper())
            using (var conn = dbHelper.Conn)
            using (var tx = conn.BeginTransaction())
            {
                try
                {
                    bool exists = conn.ExecuteScalar<int>(
                        "SELECT COUNT(1) FROM TB_INVENTORY WHERE SPARE_ID = @SPARE_ID",
                        new { model.SPARE_ID }, tx) > 0;

                    if (exists)
                    {
                        conn.Execute(@"
                    UPDATE TB_INVENTORY
                    SET CURRENT_QTY = @CURRENT_QTY,
                        SAFETY_STOCK = @SAFETY_STOCK,
                        EOQ = @EOQ,
                        REORDER_POINT = @REORDER_POINT,
                        LAST_UPDATED = GETDATE()
                    WHERE SPARE_ID = @SPARE_ID",
                            model, tx);
                    }
                    else
                    {
                        conn.Execute(@"
                    INSERT INTO TB_INVENTORY
                    (INV_ID, SPARE_ID, CURRENT_QTY, SAFETY_STOCK, EOQ, REORDER_POINT, LAST_UPDATED)
                    VALUES
                    (NEXT VALUE FOR dbo.SEQ_INV_ID, @SPARE_ID, @CURRENT_QTY, @SAFETY_STOCK, @EOQ, @REORDER_POINT, GETDATE())",
                            model, tx);
                    }

                    tx.Commit();
                    res.Message = "재고 저장 성공";
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    res.IsSuccess = false;
                    res.Message = ex.Message;
                }
            }

            return res;
        }

        public Result GetAllSPAREBasicListRepo(out List<dynamic> rows)
        {
            Result res = new Result(true);
            rows = new List<dynamic>();

            try
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    var query = @"
                SELECT
                    S.SPARE_ID,
                    S.PART_NUMBER,
                    S.PART_NAME,
                    S.CRITICALITY_GRADE,
                    S.UNIT_PRICE,
                    S.LEAD_TIME_DAYS,
                    ISNULL(I.CURRENT_QTY, 0) AS CURRENT_QTY,
                    S.IS_ACTIVE,
                    S.CREATED_AT
                FROM TB_SPARE_PART S
                LEFT JOIN TB_INVENTORY I
                    ON S.SPARE_ID = I.SPARE_ID
                ORDER BY S.SPARE_ID DESC";

                    rows = dbHelper.Conn.Query(query).ToList<dynamic>();
                    res.Message = "기본정보 목록 조회 성공";
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "기본정보 목록 조회 실패: " + ex.Message;
            }

            return res;
        }

        public ProcurementInfo GetProcurementByIdRepo(int procId)
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
            WHERE PROC_ID = @PROC_ID";

                return dbHelper.Conn.QueryFirstOrDefault<ProcurementInfo>(query, new { PROC_ID = procId });
            }
        }

        public Result UpdateProcurementRepo(ProcurementInfo model)
        {
            Result res = new Result(true);

            using (DBHelper dbHelper = new DBHelper())
            using (var conn = dbHelper.Conn)
            using (var tx = conn.BeginTransaction())
            {
                try
                {
                    decimal? totalProcCost = null;
                    if (model.ORDER_QTY.HasValue && model.UNIT_COST.HasValue)
                        totalProcCost = model.ORDER_QTY.Value * model.UNIT_COST.Value;

                    int affected = conn.Execute(@"
                UPDATE TB_PROCUREMENT
                SET
                    ORDER_QTY = @ORDER_QTY,
                    UNIT_COST = @UNIT_COST,
                    TOTAL_PROC_COST = @TOTAL_PROC_COST,
                    ORDER_DATE = @ORDER_DATE,
                    STATUS = @STATUS,
                    SUPPLIER = @SUPPLIER
                WHERE PROC_ID = @PROC_ID",
                        new
                        {
                            model.PROC_ID,
                            model.ORDER_QTY,
                            model.UNIT_COST,
                            TOTAL_PROC_COST = totalProcCost,
                            model.ORDER_DATE,
                            model.STATUS,
                            model.SUPPLIER
                        }, tx);

                    if (affected <= 0)
                        throw new Exception("수정 대상 발주가 없습니다.");

                    tx.Commit();
                    res.Message = "발주 수정 성공";
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    res.IsSuccess = false;
                    res.Message = "발주 수정 실패: " + ex.Message;
                }
            }

            return res;
        }

        public CostManagementInfo GetCostByIdRepo(int costId)
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
            WHERE COST_ID = @COST_ID";

                return dbHelper.Conn.QueryFirstOrDefault<CostManagementInfo>(query, new { COST_ID = costId });
            }
        }

        public Result UpdateCostRepo(CostManagementInfo model)
        {
            Result res = new Result(true);

            using (DBHelper dbHelper = new DBHelper())
            using (var conn = dbHelper.Conn)
            using (var tx = conn.BeginTransaction())
            {
                try
                {
                    int affected = conn.Execute(@"
                UPDATE TB_COST_MANAGEMENT
                SET
                    FISCAL_YEAR = @FISCAL_YEAR,
                    BUDGET_AMOUNT = @BUDGET_AMOUNT,
                    ACTUAL_AMOUNT = @ACTUAL_AMOUNT,
                    UPDATED_AT = GETDATE()
                WHERE COST_ID = @COST_ID",
                        model, tx);

                    if (affected <= 0)
                        throw new Exception("수정 대상 비용계획이 없습니다.");

                    tx.Commit();
                    res.Message = "비용계획 수정 성공";
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    res.IsSuccess = false;
                    res.Message = "비용계획 수정 실패: " + ex.Message;
                }
            }

            return res;
        }
    }
}
