using AMS_MVC.Database;
using AMS_MVC.Models;
using Dapper;
using System;

public class CoFRepository
{
    /// <summary>
    /// 특정 장비(Code) 의 최신 CoFModel 한 건을 가져옴
    /// </summary>
    public COFModel GetLatest(string code)
    {
        const string sql = @"
SELECT TOP 1
    Tbl_Idx,
    Code,
    POWER_FAILURE_TIME                  AS Power_Failure_Time,
    POWER_FAILURE_COST                  AS Power_Failure_Cost,
    PROBABILITY_OF_POWER_FAILURE        AS Probability_Of_Power_Failure,
    CAPACITY                            AS Capacity,
    POWER_FACTOR                        AS Power_Factor,
    COEFFICIENT                         AS Coefficient,
    AVERAGE_UTILIZATION_RATE            AS Average_Utilization_Rate,
    FACILITY_RECOVERY_TIME              AS Facility_Recovery_Time,
    RATED_VOLTAGE                       AS Rated_Voltage,
    AVERAGE_ELECTRICITY_SALES_COST      AS Average_Electricity_Sales_Cost,
    TRACK_LENGTH                        AS Track_Length,
    REPLACEMENT_PROBABILITY             AS Replacement_Probability,
    EQUIPMENT_UNIT_PRICE                AS Equipment_Unit_Price,
    FACILITY_CONTRACTING_COST           AS Facility_Contracting_Cost,
    EMERGENCY_CONSTRUCTION_SURCHARGE_RATE AS Emergency_Construction_Surcharge_Rate,
    GENERAL_ACCIDENT                    AS General_Accident,
    DEAD_ACCIDENT                       AS Dead_Accident,
    GENERAL_COST                        AS General_Cost,
    DEAD_COST                           AS Dead_Cost,
    SAFETY_SENSITIVITY_COST             AS Safety_Sensitivity_Cost,
    INSULATION_OIL_AREA                 AS Insulation_Oil_Area,
    COST                                AS Cost,
    ENVIRONMENTAL_POLLUTION             AS Environmental_Pollution,
    POSITION_WEIGHT                     AS Position_Weight,
    CUSTOMER_POWER_OUTAGE_COST          AS Customer_Power_Outage_Cost,
    SYSTEM_LOSS_COST                    AS System_Loss_Cost,
    FACILITY_RECOVERY_COST              AS Facility_Recovery_Cost,
    LOSS_OF_PROFIT                      AS Loss_Of_Profit,
    SAFETY_ACCIDENT_COMPENSATION_1      AS Safety_Accident_Compensation_1,
    SAFETY_ACCIDENT_COMPENSATION_2      AS Safety_Accident_Compensation_2,
    TOTAL_COF                           AS Total_Cof
FROM COF
WHERE Code = @Code
ORDER BY Tbl_GetDate DESC;
";

        using (var db = new DBHelper())
        {
            return db.Conn
                     .QueryFirstOrDefault<COFModel>(sql, new { Code = code })
                   ?? new COFModel { Code = code };
        }
    }

    /// <summary>
    /// 날짜가 같으면 UPDATE, 아니면 INSERT
    /// </summary>
    public void SaveOrUpdate(COFModel m)
    {
        // 1) 입력전 날짜 확인
        var latest = GetLatest(m.Code);

        if (latest.Tbl_GetDate.Date == DateTime.Today && latest.Tbl_Idx != 0)
        {
            // 같은 날 이미 있으면 UPDATE
            const string updateSql = @"
UPDATE COF
   SET
     POWER_FAILURE_TIME                  = @Power_Failure_Time,
     POWER_FAILURE_COST                  = @Power_Failure_Cost,
     PROBABILITY_OF_POWER_FAILURE        = @Probability_Of_Power_Failure,
     CAPACITY                            = @Capacity,
     POWER_FACTOR                        = @Power_Factor,
     COEFFICIENT                         = @Coefficient,
     AVERAGE_UTILIZATION_RATE            = @Average_Utilization_Rate,
     FACILITY_RECOVERY_TIME              = @Facility_Recovery_Time,
     RATED_VOLTAGE                       = @Rated_Voltage,
     AVERAGE_ELECTRICITY_SALES_COST      = @Average_Electricity_Sales_Cost,
     TRACK_LENGTH                        = @Track_Length,
     REPLACEMENT_PROBABILITY             = @Replacement_Probability,
     EQUIPMENT_UNIT_PRICE                = @Equipment_Unit_Price,
     FACILITY_CONTRACTING_COST           = @Facility_Contracting_Cost,
     EMERGENCY_CONSTRUCTION_SURCHARGE_RATE = @Emergency_Construction_Surcharge_Rate,
     GENERAL_ACCIDENT                    = @General_Accident,
     DEAD_ACCIDENT                       = @Dead_Accident,
     GENERAL_COST                        = @General_Cost,
     DEAD_COST                           = @Dead_Cost,
     SAFETY_SENSITIVITY_COST             = @Safety_Sensitivity_Cost,
     INSULATION_OIL_AREA                 = @Insulation_Oil_Area,
     COST                                = @Cost,
     ENVIRONMENTAL_POLLUTION             = @Environmental_Pollution,
     POSITION_WEIGHT                     = @Position_Weight,
     CUSTOMER_POWER_OUTAGE_COST          = @Customer_Power_Outage_Cost,
     SYSTEM_LOSS_COST                    = @System_Loss_Cost,
     FACILITY_RECOVERY_COST              = @Facility_Recovery_Cost,
     LOSS_OF_PROFIT                      = @Loss_Of_Profit,
     SAFETY_ACCIDENT_COMPENSATION_1      = @Safety_Accident_Compensation_1,
     SAFETY_ACCIDENT_COMPENSATION_2      = @Safety_Accident_Compensation_2,
     TOTAL_COF                           = @Total_Cof
 WHERE Tbl_Idx = @Tbl_Idx;
";
            using (var db = new DBHelper())
                db.Conn.Execute(updateSql, m);
        }
        else
        {
            // 날짜가 다르면 새로 INSERT
            Insert(m);
        }
    }

    public void Insert(COFModel m)
    {
        const string sql = @"
INSERT INTO COF (
    Code,
    POWER_FAILURE_TIME,
    POWER_FAILURE_COST,
    PROBABILITY_OF_POWER_FAILURE,
    CAPACITY,
    POWER_FACTOR,
    COEFFICIENT,
    AVERAGE_UTILIZATION_RATE,
    FACILITY_RECOVERY_TIME,
    RATED_VOLTAGE,
    AVERAGE_ELECTRICITY_SALES_COST,
    TRACK_LENGTH,
    REPLACEMENT_PROBABILITY,
    EQUIPMENT_UNIT_PRICE,
    FACILITY_CONTRACTING_COST,
    EMERGENCY_CONSTRUCTION_SURCHARGE_RATE,
    GENERAL_ACCIDENT,
    DEAD_ACCIDENT,
    GENERAL_COST,
    DEAD_COST,
    SAFETY_SENSITIVITY_COST,
    INSULATION_OIL_AREA,
    COST,
    ENVIRONMENTAL_POLLUTION,
    POSITION_WEIGHT,
    CUSTOMER_POWER_OUTAGE_COST,
    SYSTEM_LOSS_COST,
    FACILITY_RECOVERY_COST,
    LOSS_OF_PROFIT,
    SAFETY_ACCIDENT_COMPENSATION_1,
    SAFETY_ACCIDENT_COMPENSATION_2,
    TOTAL_COF
) VALUES (
    @Code,
    @Power_Failure_Time,
    @Power_Failure_Cost,
    @Probability_Of_Power_Failure,
    @Capacity,
    @Power_Factor,
    @Coefficient,
    @Average_Utilization_Rate,
    @Facility_Recovery_Time,
    @Rated_Voltage,
    @Average_Electricity_Sales_Cost,
    @Track_Length,
    @Replacement_Probability,
    @Equipment_Unit_Price,
    @Facility_Contracting_Cost,
    @Emergency_Construction_Surcharge_Rate,
    @General_Accident,
    @Dead_Accident,
    @General_Cost,
    @Dead_Cost,
    @Safety_Sensitivity_Cost,
    @Insulation_Oil_Area,
    @Cost,
    @Environmental_Pollution,
    @Position_Weight,
    @Customer_Power_Outage_Cost,
    @System_Loss_Cost,
    @Facility_Recovery_Cost,
    @Loss_Of_Profit,
    @Safety_Accident_Compensation_1,
    @Safety_Accident_Compensation_2,
    @Total_Cof
);
";
        using (var db = new DBHelper())
        {
            db.Conn.Execute(sql, m);
        }
    }
}
