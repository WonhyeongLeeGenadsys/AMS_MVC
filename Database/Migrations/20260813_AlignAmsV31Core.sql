SET XACT_ABORT ON;
BEGIN TRANSACTION;

/* MVDC AMS v3.1.0 핵심 기준값 정합화 */
DECLARE @EquipmentCost TABLE
(
    EQUIPMENT_KEY VARCHAR(50) NOT NULL PRIMARY KEY,
    REPLACEMENT_COST DECIMAL(18, 2) NOT NULL
);

INSERT INTO @EquipmentCost (EQUIPMENT_KEY, REPLACEMENT_COST)
VALUES
    ('ITR',        800000000),
    ('VCB',        200000000),
    ('SUBMODULE',   50000000),
    ('DCCB',      1200000000),
    ('DCCABLE',    300000000);

UPDATE C
SET C.REPLACEMENT_COST = S.REPLACEMENT_COST,
    C.IS_ACTIVE = 1,
    C.UPDATED_AT = GETDATE()
FROM dbo.TB_DM_EQUIPMENT_COST C
INNER JOIN @EquipmentCost S
    ON UPPER(S.EQUIPMENT_KEY) = UPPER(C.EQUIPMENT_KEY);

INSERT INTO dbo.TB_DM_EQUIPMENT_COST
(
    EQUIPMENT_KEY,
    REPLACEMENT_COST,
    IS_ACTIVE,
    UPDATED_AT
)
SELECT S.EQUIPMENT_KEY, S.REPLACEMENT_COST, 1, GETDATE()
FROM @EquipmentCost S
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.TB_DM_EQUIPMENT_COST C
    WHERE UPPER(C.EQUIPMENT_KEY) = UPPER(S.EQUIPMENT_KEY)
);

DECLARE @Weibull TABLE
(
    Category VARCHAR(10) NOT NULL,
    EquipmentName VARCHAR(50) NOT NULL PRIMARY KEY,
    ShapeParam FLOAT NOT NULL,
    ScaleParam FLOAT NOT NULL
);

INSERT INTO @Weibull (Category, EquipmentName, ShapeParam, ScaleParam)
VALUES
    ('AC', 'ITR',       2.5, 35.0),
    ('AC', 'VCB',       3.0, 30.0),
    ('DC', 'SUBMODULE', 2.2, 10.0),
    ('DC', 'DCCB',      3.0, 22.0),
    ('DC', 'DCCABLE',   2.5, 32.0);

UPDATE W
SET W.Category = S.Category,
    W.ShapeParam = S.ShapeParam,
    W.ScaleParam = S.ScaleParam,
    W.FailureRate = NULL
FROM dbo.EquipmentWeibull W
INNER JOIN @Weibull S
    ON UPPER(S.EquipmentName) = UPPER(W.EquipmentName);

INSERT INTO dbo.EquipmentWeibull
(
    Category,
    EquipmentName,
    ShapeParam,
    ScaleParam,
    FailureRate
)
SELECT S.Category, S.EquipmentName, S.ShapeParam, S.ScaleParam, NULL
FROM @Weibull S
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.EquipmentWeibull W
    WHERE UPPER(W.EquipmentName) = UPPER(S.EquipmentName)
);

COMMIT TRANSACTION;
