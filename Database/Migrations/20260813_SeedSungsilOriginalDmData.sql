SET XACT_ABORT ON;
BEGIN TRANSACTION;

/*
    숭실대 DM모듈 배포본 v1.0 기준 초기 데이터
    - 5종 주요장비 교체비
    - 5종 주요장비 예비품 BOM 21종
    - 장비유형별 필요수량
    - 원본 Weibull 파라미터(일 단위 척도는 AMS의 연 단위로 환산)
*/

DECLARE @EquipmentCost TABLE
(
    EQUIPMENT_KEY VARCHAR(50) NOT NULL PRIMARY KEY,
    REPLACEMENT_COST DECIMAL(18, 2) NOT NULL
);

INSERT INTO @EquipmentCost (EQUIPMENT_KEY, REPLACEMENT_COST)
VALUES
    ('VCB',        200000000),
    ('ITR',        800000000),
    ('SUBMODULE', 1000000000),
    ('DCCB',      1200000000),
    ('DCCABLE',    300000000);

UPDATE C
SET C.REPLACEMENT_COST = S.REPLACEMENT_COST,
    C.IS_ACTIVE = 1,
    C.UPDATED_AT = GETDATE()
FROM dbo.TB_DM_EQUIPMENT_COST C
INNER JOIN @EquipmentCost S
    ON S.EQUIPMENT_KEY = C.EQUIPMENT_KEY;

INSERT INTO dbo.TB_DM_EQUIPMENT_COST
(
    EQUIPMENT_KEY,
    REPLACEMENT_COST,
    IS_ACTIVE,
    UPDATED_AT
)
SELECT
    S.EQUIPMENT_KEY,
    S.REPLACEMENT_COST,
    1,
    GETDATE()
FROM @EquipmentCost S
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.TB_DM_EQUIPMENT_COST C
    WHERE C.EQUIPMENT_KEY = S.EQUIPMENT_KEY
);

DECLARE @Bom TABLE
(
    PART_NUMBER NVARCHAR(50) NOT NULL PRIMARY KEY,
    PART_NAME NVARCHAR(200) NOT NULL,
    ASSET_TYPE_ID INT NOT NULL,
    REQUIRED_QTY INT NOT NULL,
    CRITICALITY_GRADE VARCHAR(10) NOT NULL,
    UNIT_PRICE INT NOT NULL,
    LEAD_TIME_DAYS INT NOT NULL,
    SUPPLIER NVARCHAR(100) NULL,
    NOTES NVARCHAR(500) NULL
);

INSERT INTO @Bom
(
    PART_NUMBER, PART_NAME, ASSET_TYPE_ID, REQUIRED_QTY,
    CRITICALITY_GRADE, UNIT_PRICE, LEAD_TIME_DAYS, SUPPLIER, NOTES
)
VALUES
    (N'TR-WIND-HV', N'고압권선 세트 (High Voltage Winding)', 5, 1, 'CRITICAL', 200000000, 180, N'효성중공업', N'110kV 절연 권선, 고장 시 전체 교체 필요'),
    (N'TR-WIND-LV', N'저압권선 세트 (Low Voltage Winding)', 5, 1, 'CRITICAL', 150000000, 180, N'효성중공업', N'22.9kV 권선, 고장 시 전체 교체 필요'),
    (N'TR-INS-OIL', N'절연유 (Insulation Oil)', 5, 5000, 'HIGH', 50000, 30, N'SK엔무브', N'정기 보충용, 연간 5% 손실'),
    (N'TR-BUSHING', N'부싱 세트 (Bushing Set)', 5, 6, 'HIGH', 30000000, 90, N'국내 제조사', N'고압/저압 부싱 각 3개'),
    (N'TR-TAP-CHANGER', N'탭 절환기 (Tap Changer)', 5, 1, 'MEDIUM', 80000000, 120, N'MR', N'전압 조정용, 수명 15년'),

    (N'VCB-CONTACT-SET', N'접점 세트 (Contact Set)', 1, 3, 'CRITICAL', 20000000, 60, N'LS일렉트릭/현대일렉트릭', N'3상 접점, 수명 10,000회 개폐'),
    (N'VCB-VACUUM-BOTTLE', N'진공 밸브 (Vacuum Interrupter)', 1, 3, 'CRITICAL', 15000000, 90, N'ABB/Schneider', N'진공도 10^-6 Torr, 수명 30년'),
    (N'VCB-SPRING-MECH', N'스프링 조작기구 (Spring Mechanism)', 1, 1, 'HIGH', 10000000, 60, N'국내 제조사', N'전자 조작식'),
    (N'VCB-MOTOR-DRIVE', N'구동 모터 (Motor Drive)', 1, 1, 'MEDIUM', 5000000, 30, N'국내 제조사', N'스프링 충전용 모터'),

    (N'SM-IGBT-PAIR', N'IGBT 페어 (IGBT Pair)', 3, 2, 'CRITICAL', 15000000, 90, N'Infineon/Mitsubishi', N'Half-bridge 구성 IGBT'),
    (N'SM-CAPACITOR', N'서브모듈 커패시터', 3, 1, 'CRITICAL', 8000000, 60, N'TDK/EPCOS', N'DC 링크 커패시터'),
    (N'SM-GATE-DRIVER', N'게이트 드라이버', 3, 2, 'HIGH', 2000000, 45, N'국내 제조사', N'광섬유 절연 게이트 드라이버'),
    (N'SM-SENSOR-BOARD', N'센서 보드', 3, 1, 'MEDIUM', 3000000, 30, N'국내 제조사', N'전압/전류 센서'),

    (N'DCCB-IGBT-STACK', N'IGBT 스택 (IGBT Stack)', 2, 4, 'CRITICAL', 100000000, 120, N'ABB/Siemens', N'Hybrid DC 차단기용 고속 IGBT, 직렬 4개'),
    (N'DCCB-MECH-SWITCH', N'기계식 스위치 (Mechanical Switch)', 2, 2, 'CRITICAL', 80000000, 90, N'ABB/Siemens', N'Ultra-fast disconnector'),
    (N'DCCB-SURGE-ARREST', N'서지 흡수기 (Surge Arrester)', 2, 4, 'HIGH', 30000000, 60, N'국내 제조사', N'MOV 타입 서지 흡수기'),
    (N'DCCB-CONTROL-UNIT', N'제어 유닛', 2, 1, 'HIGH', 40000000, 60, N'KEPCO/국내', N'차단 제어 및 보호'),

    (N'DCCABLE-CONDUCTOR', N'동도체 (Copper Conductor)', 4, 1000, 'HIGH', 150000, 45, N'LS전선/대한전선', N'1000mm2 동도체'),
    (N'DCCABLE-INSULATION', N'XLPE 절연재 (XLPE Insulation)', 4, 1000, 'MEDIUM', 50000, 30, N'LS전선', N'가교 폴리에틸렌 절연'),
    (N'DCCABLE-TERMINATION', N'케이블 단말 (Cable Termination)', 4, 2, 'HIGH', 15000000, 60, N'3M/국내', N'±80kV 단말'),
    (N'DCCABLE-JOINT', N'케이블 접속함 (Cable Joint)', 4, 5, 'MEDIUM', 10000000, 45, N'국내 제조사', N'중간 접속함');

UPDATE P
SET P.PART_NAME = B.PART_NAME,
    P.CRITICALITY_GRADE = B.CRITICALITY_GRADE,
    P.UNIT_PRICE = B.UNIT_PRICE,
    P.LEAD_TIME_DAYS = B.LEAD_TIME_DAYS,
    P.SUPPLIER = B.SUPPLIER,
    P.NOTES = B.NOTES,
    P.IS_ACTIVE = 1,
    P.UPDATED_AT = GETDATE()
FROM dbo.TB_SPARE_PART P
INNER JOIN @Bom B
    ON B.PART_NUMBER = P.PART_NUMBER;

INSERT INTO dbo.TB_SPARE_PART
(
    SPARE_ID,
    PART_NUMBER,
    PART_NAME,
    CRITICALITY_GRADE,
    UNIT_PRICE,
    LEAD_TIME_DAYS,
    IS_ACTIVE,
    SUPPLIER,
    NOTES,
    CREATED_AT,
    UPDATED_AT
)
SELECT
    NEXT VALUE FOR dbo.SEQ_SPARE_ID,
    B.PART_NUMBER,
    B.PART_NAME,
    B.CRITICALITY_GRADE,
    B.UNIT_PRICE,
    B.LEAD_TIME_DAYS,
    1,
    B.SUPPLIER,
    B.NOTES,
    GETDATE(),
    NULL
FROM @Bom B
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.TB_SPARE_PART P
    WHERE P.PART_NUMBER = B.PART_NUMBER
);

/* 기존 화면 확인용 테스트 데이터는 이력을 보존하면서 계산에서 제외한다. */
UPDATE dbo.TB_SPARE_PART
SET IS_ACTIVE = 0,
    UPDATED_AT = GETDATE()
WHERE PART_NUMBER IN (N'SP001', N'SP002', N'SP003');

/* 원본 BOM 부품에는 원본에서 정의한 장비유형 연결만 유지한다. */
DELETE M
FROM dbo.TB_SPARE_ASSET_MAP M
INNER JOIN dbo.TB_SPARE_PART P
    ON P.SPARE_ID = M.SPARE_ID
INNER JOIN @Bom B
    ON B.PART_NUMBER = P.PART_NUMBER
WHERE M.ASSET_TYPE_ID <> B.ASSET_TYPE_ID;

UPDATE M
SET M.REQUIRED_QTY = B.REQUIRED_QTY
FROM dbo.TB_SPARE_ASSET_MAP M
INNER JOIN dbo.TB_SPARE_PART P
    ON P.SPARE_ID = M.SPARE_ID
INNER JOIN @Bom B
    ON B.PART_NUMBER = P.PART_NUMBER
   AND B.ASSET_TYPE_ID = M.ASSET_TYPE_ID;

INSERT INTO dbo.TB_SPARE_ASSET_MAP
(
    SPARE_ASSET_MAP_ID,
    SPARE_ID,
    ASSET_TYPE_ID,
    REQUIRED_QTY,
    CREATED_AT
)
SELECT
    NEXT VALUE FOR dbo.SEQ_SPARE_ASSET_MAP_ID,
    P.SPARE_ID,
    B.ASSET_TYPE_ID,
    B.REQUIRED_QTY,
    GETDATE()
FROM @Bom B
INNER JOIN dbo.TB_SPARE_PART P
    ON P.PART_NUMBER = B.PART_NUMBER
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.TB_SPARE_ASSET_MAP M
    WHERE M.SPARE_ID = P.SPARE_ID
      AND M.ASSET_TYPE_ID = B.ASSET_TYPE_ID
);

INSERT INTO dbo.TB_INVENTORY
(
    INV_ID,
    SPARE_ID,
    CURRENT_QTY,
    SAFETY_STOCK,
    EOQ,
    REORDER_POINT,
    MIN_STOCK,
    MAX_STOCK,
    POLICY_TYPE,
    LAST_UPDATED
)
SELECT
    NEXT VALUE FOR dbo.SEQ_INV_ID,
    P.SPARE_ID,
    0,
    0,
    0,
    0,
    0,
    0,
    NULL,
    GETDATE()
FROM dbo.TB_SPARE_PART P
INNER JOIN @Bom B
    ON B.PART_NUMBER = P.PART_NUMBER
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.TB_INVENTORY I
    WHERE I.SPARE_ID = P.SPARE_ID
);

/* 원본 weibull_pof_calculator.py의 일 단위 scale을 AMS의 연 단위로 환산한다. */
DECLARE @Weibull TABLE
(
    Category VARCHAR(10) NOT NULL,
    EquipmentName VARCHAR(50) NOT NULL PRIMARY KEY,
    ShapeParam FLOAT NULL,
    ScaleParam FLOAT NULL,
    FailureRate FLOAT NULL
);

INSERT INTO @Weibull (Category, EquipmentName, ShapeParam, ScaleParam, FailureRate)
VALUES
    ('AC', 'ITR',       2.0, 3000.0 / 365.24, NULL),
    ('AC', 'VCB',       5.1, 40.4,            NULL),
    ('DC', 'SUBMODULE', 4.0, 25.0,            NULL),
    ('DC', 'DCCB',      5.6, 43.4,            NULL),
    ('DC', 'DCCABLE',   1.0, 15000.0 / 365.24, NULL);

UPDATE W
SET W.Category = S.Category,
    W.ShapeParam = S.ShapeParam,
    W.ScaleParam = S.ScaleParam,
    W.FailureRate = S.FailureRate
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
SELECT
    S.Category,
    S.EquipmentName,
    S.ShapeParam,
    S.ScaleParam,
    S.FailureRate
FROM @Weibull S
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.EquipmentWeibull W
    WHERE UPPER(W.EquipmentName) = UPPER(S.EquipmentName)
);

COMMIT TRANSACTION;
