SET XACT_ABORT ON;
BEGIN TRANSACTION;

/*
    MVDC AMS v3.1.0 spare-parts BOM
    Source: config/spare_parts_bom.yaml (2026-06-18)

    Existing AMS asset type IDs are preserved:
      1 VCB, 2 DC_Breaker, 3 MMC_Submodule, 4 DC_Cable, 5 Transformer
    v3.1 asset types added by this seed:
      6 Converter, 7 Circuit_Breaker, 8 Cable, 9 Switchgear,
      10 Protection_Relay, 11 Cooling_System, 12 Energy_Storage, 13 SCADA

    The migration is idempotent. User-created parts outside this BOM are not deleted.
*/

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
    (N'TR-WIND-HV', N'고압권선 세트', 5, 1, 'CRITICAL', 200000000, 180, N'효성중공업', N'v3.1 Transformer BOM'),
    (N'TR-WIND-LV', N'저압권선 세트', 5, 1, 'CRITICAL', 150000000, 180, N'효성중공업', N'v3.1 Transformer BOM'),
    (N'TR-INS-OIL', N'절연유', 5, 5000, 'HIGH', 50000, 30, N'SK엔무브', N'v3.1 Transformer BOM'),
    (N'TR-BUSHING', N'부싱 세트', 5, 6, 'HIGH', 30000000, 90, N'국내 제조사', N'v3.1 Transformer BOM'),
    (N'TR-TAP-CHANGER', N'탭 절환기', 5, 1, 'MEDIUM', 80000000, 120, N'MR', N'v3.1 Transformer BOM'),

    (N'CV-IGBT-MODULE', N'IGBT 모듈', 6, 48, 'CRITICAL', 15000000, 90, N'Infineon/Mitsubishi', N'v3.1 Converter BOM'),
    (N'CV-DC-CAP', N'DC 커패시터', 6, 24, 'CRITICAL', 8000000, 60, N'TDK/EPCOS', N'v3.1 Converter BOM'),
    (N'CV-COOLING-UNIT', N'냉각 유닛', 6, 4, 'HIGH', 50000000, 90, N'국내 제조사', N'v3.1 Converter BOM'),
    (N'CV-CONTROL-BOARD', N'제어 보드', 6, 12, 'HIGH', 5000000, 45, N'LS일렉트릭', N'v3.1 Converter BOM'),
    (N'CV-GATE-DRIVER', N'게이트 드라이버', 6, 48, 'MEDIUM', 2000000, 45, N'국내 제조사', N'v3.1 Converter BOM'),

    (N'SM-IGBT-PAIR', N'IGBT 페어', 3, 2, 'CRITICAL', 15000000, 90, N'Infineon/Mitsubishi', N'v3.1 MMC_Submodule BOM'),
    (N'SM-CAPACITOR', N'서브모듈 커패시터', 3, 1, 'CRITICAL', 8000000, 60, N'TDK/EPCOS', N'v3.1 MMC_Submodule BOM'),
    (N'SM-GATE-DRIVER', N'게이트 드라이버', 3, 2, 'HIGH', 2000000, 45, N'국내 제조사', N'v3.1 MMC_Submodule BOM'),
    (N'SM-SENSOR-BOARD', N'센서 보드', 3, 1, 'MEDIUM', 3000000, 30, N'국내 제조사', N'v3.1 MMC_Submodule BOM'),

    (N'VCB-CONTACT-SET', N'접점 세트', 1, 3, 'CRITICAL', 20000000, 60, N'LS일렉트릭', N'v3.1 VCB BOM'),
    (N'VCB-VACUUM-BOTTLE', N'진공 밸브', 1, 3, 'CRITICAL', 15000000, 90, N'ABB/Schneider', N'v3.1 VCB BOM'),
    (N'VCB-SPRING-MECH', N'스프링 조작기구', 1, 1, 'HIGH', 10000000, 60, N'국내 제조사', N'v3.1 VCB BOM'),
    (N'VCB-MOTOR-DRIVE', N'구동 모터', 1, 1, 'MEDIUM', 5000000, 30, N'국내 제조사', N'v3.1 VCB BOM'),

    (N'DCCB-IGBT-STACK', N'IGBT 스택', 2, 4, 'CRITICAL', 100000000, 120, N'ABB/Siemens', N'v3.1 DC_Breaker BOM'),
    (N'DCCB-MECH-SWITCH', N'기계식 스위치', 2, 2, 'CRITICAL', 80000000, 90, N'ABB/Siemens', N'v3.1 DC_Breaker BOM'),
    (N'DCCB-SURGE-ARREST', N'서지 흡수기', 2, 4, 'HIGH', 30000000, 60, N'국내 제조사', N'v3.1 DC_Breaker BOM'),
    (N'DCCB-CONTROL-UNIT', N'제어 유닛', 2, 1, 'HIGH', 40000000, 60, N'KEPCO/국내', N'v3.1 DC_Breaker BOM'),

    (N'DCCABLE-CONDUCTOR', N'동도체', 4, 1000, 'HIGH', 150000, 45, N'LS전선/대한전선', N'v3.1 DC_Cable BOM'),
    (N'DCCABLE-INSULATION', N'XLPE 절연재', 4, 1000, 'MEDIUM', 50000, 30, N'LS전선', N'v3.1 DC_Cable BOM'),
    (N'DCCABLE-TERMINATION', N'케이블 단말', 4, 2, 'HIGH', 15000000, 60, N'3M/국내', N'v3.1 DC_Cable BOM'),
    (N'DCCABLE-JOINT', N'케이블 접속함', 4, 5, 'MEDIUM', 10000000, 45, N'국내 제조사', N'v3.1 DC_Cable BOM'),

    (N'CB-CONTACT-SET', N'접점 세트', 7, 3, 'CRITICAL', 10000000, 60, N'LS일렉트릭', N'v3.1 Circuit_Breaker BOM'),
    (N'CB-ARC-CHAMBER', N'소호실', 7, 3, 'HIGH', 8000000, 45, N'LS일렉트릭', N'v3.1 Circuit_Breaker BOM'),
    (N'CB-SPRING-MECH', N'스프링 조작기구', 7, 1, 'MEDIUM', 5000000, 45, N'국내 제조사', N'v3.1 Circuit_Breaker BOM'),
    (N'CB-SF6-GAS', N'SF6 가스', 7, 100, 'LOW', 30000, 14, N'국내 공급사', N'v3.1 Circuit_Breaker BOM'),

    (N'CABLE-CONDUCTOR', N'동도체', 8, 1000, 'HIGH', 120000, 45, N'LS전선/대한전선', N'v3.1 Cable BOM'),
    (N'CABLE-INSULATION', N'XLPE 절연재', 8, 1000, 'MEDIUM', 40000, 30, N'LS전선', N'v3.1 Cable BOM'),
    (N'CABLE-TERMINATION', N'케이블 단말', 8, 2, 'MEDIUM', 8000000, 45, N'3M/국내', N'v3.1 Cable BOM'),

    (N'SWG-BUSBAR', N'모선(부스바)', 9, 3, 'HIGH', 12000000, 60, N'LS일렉트릭/현대일렉트릭', N'v3.1 Switchgear BOM'),
    (N'SWG-DISCONNECTOR', N'단로기', 9, 3, 'CRITICAL', 9000000, 60, N'국내 제조사', N'v3.1 Switchgear BOM'),
    (N'SWG-EARTH-SWITCH', N'접지 스위치', 9, 3, 'MEDIUM', 4000000, 45, N'국내 제조사', N'v3.1 Switchgear BOM'),
    (N'SWG-PROT-RELAY', N'보호 계전 유닛', 9, 1, 'HIGH', 6000000, 45, N'국내 제조사', N'v3.1 Switchgear BOM'),

    (N'PR-RELAY-UNIT', N'계전기 유닛(IED)', 10, 1, 'CRITICAL', 8000000, 60, N'SEL/LS일렉트릭', N'v3.1 Protection_Relay BOM'),
    (N'PR-CTPT-MODULE', N'CT/PT 입력 모듈', 10, 2, 'HIGH', 2000000, 45, N'국내 제조사', N'v3.1 Protection_Relay BOM'),
    (N'PR-COMM-MODULE', N'통신 모듈(IEC61850)', 10, 1, 'MEDIUM', 1500000, 30, N'국내 제조사', N'v3.1 Protection_Relay BOM'),

    (N'CS-PUMP', N'냉각 펌프', 11, 2, 'CRITICAL', 7000000, 60, N'국내 제조사', N'v3.1 Cooling_System BOM'),
    (N'CS-HEAT-EXCH', N'열교환기', 11, 1, 'HIGH', 12000000, 75, N'국내 제조사', N'v3.1 Cooling_System BOM'),
    (N'CS-FAN', N'냉각 팬', 11, 4, 'MEDIUM', 2000000, 30, N'국내 제조사', N'v3.1 Cooling_System BOM'),
    (N'CS-CTRL-VALVE', N'제어 밸브', 11, 3, 'MEDIUM', 1500000, 30, N'국내 제조사', N'v3.1 Cooling_System BOM'),

    (N'ES-BATTERY-MODULE', N'배터리 모듈', 12, 20, 'CRITICAL', 8000000, 90, N'LG에너지솔루션/삼성SDI', N'v3.1 Energy_Storage BOM'),
    (N'ES-PCS', N'PCS(전력변환장치)', 12, 1, 'CRITICAL', 40000000, 90, N'국내 제조사', N'v3.1 Energy_Storage BOM'),
    (N'ES-BMS', N'BMS(배터리관리)', 12, 1, 'HIGH', 6000000, 60, N'국내 제조사', N'v3.1 Energy_Storage BOM'),
    (N'ES-COOLING', N'ESS 냉각 유닛', 12, 2, 'MEDIUM', 5000000, 45, N'국내 제조사', N'v3.1 Energy_Storage BOM'),

    (N'SC-RTU', N'RTU(원격단말장치)', 13, 2, 'HIGH', 5000000, 60, N'국내 제조사', N'v3.1 SCADA BOM'),
    (N'SC-SERVER-HMI', N'서버/HMI', 13, 1, 'CRITICAL', 15000000, 75, N'국내 제조사', N'v3.1 SCADA BOM'),
    (N'SC-GATEWAY', N'통신 게이트웨이', 13, 2, 'MEDIUM', 3000000, 45, N'국내 제조사', N'v3.1 SCADA BOM'),
    (N'SC-UPS', N'무정전전원(UPS)', 13, 1, 'HIGH', 4000000, 30, N'국내 제조사', N'v3.1 SCADA BOM');

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
INNER JOIN @Bom B ON B.PART_NUMBER = P.PART_NUMBER;

INSERT INTO dbo.TB_SPARE_PART
(
    SPARE_ID, PART_NUMBER, PART_NAME, CRITICALITY_GRADE,
    UNIT_PRICE, LEAD_TIME_DAYS, IS_ACTIVE, SUPPLIER, NOTES,
    CREATED_AT, UPDATED_AT
)
SELECT
    NEXT VALUE FOR dbo.SEQ_SPARE_ID,
    B.PART_NUMBER, B.PART_NAME, B.CRITICALITY_GRADE,
    B.UNIT_PRICE, B.LEAD_TIME_DAYS, 1, B.SUPPLIER, B.NOTES,
    GETDATE(), NULL
FROM @Bom B
WHERE NOT EXISTS
(
    SELECT 1 FROM dbo.TB_SPARE_PART P WHERE P.PART_NUMBER = B.PART_NUMBER
);

/* Each v3.1 BOM part belongs to exactly one asset type. */
DELETE M
FROM dbo.TB_SPARE_ASSET_MAP M
INNER JOIN dbo.TB_SPARE_PART P ON P.SPARE_ID = M.SPARE_ID
INNER JOIN @Bom B ON B.PART_NUMBER = P.PART_NUMBER
WHERE M.ASSET_TYPE_ID <> B.ASSET_TYPE_ID;

UPDATE M
SET M.REQUIRED_QTY = B.REQUIRED_QTY
FROM dbo.TB_SPARE_ASSET_MAP M
INNER JOIN dbo.TB_SPARE_PART P ON P.SPARE_ID = M.SPARE_ID
INNER JOIN @Bom B
    ON B.PART_NUMBER = P.PART_NUMBER
   AND B.ASSET_TYPE_ID = M.ASSET_TYPE_ID;

INSERT INTO dbo.TB_SPARE_ASSET_MAP
(
    SPARE_ASSET_MAP_ID, SPARE_ID, ASSET_TYPE_ID, REQUIRED_QTY, CREATED_AT
)
SELECT
    NEXT VALUE FOR dbo.SEQ_SPARE_ASSET_MAP_ID,
    P.SPARE_ID, B.ASSET_TYPE_ID, B.REQUIRED_QTY, GETDATE()
FROM @Bom B
INNER JOIN dbo.TB_SPARE_PART P ON P.PART_NUMBER = B.PART_NUMBER
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.TB_SPARE_ASSET_MAP M
    WHERE M.SPARE_ID = P.SPARE_ID
      AND M.ASSET_TYPE_ID = B.ASSET_TYPE_ID
);

INSERT INTO dbo.TB_INVENTORY
(
    INV_ID, SPARE_ID, CURRENT_QTY, SAFETY_STOCK, EOQ,
    REORDER_POINT, MIN_STOCK, MAX_STOCK, POLICY_TYPE, LAST_UPDATED
)
SELECT
    NEXT VALUE FOR dbo.SEQ_INV_ID,
    P.SPARE_ID, 0, 0, 0, 0, 0, 0, NULL, GETDATE()
FROM dbo.TB_SPARE_PART P
INNER JOIN @Bom B ON B.PART_NUMBER = P.PART_NUMBER
WHERE NOT EXISTS
(
    SELECT 1 FROM dbo.TB_INVENTORY I WHERE I.SPARE_ID = P.SPARE_ID
);

COMMIT TRANSACTION;

