using AMS_MVC.Database;
using AMS_MVC.Models;
using Dapper;
using System;

public class CoFRepository
{
    public COFModel GetLatest(string code)
    {
        const string sql = @"
SELECT TOP 1
  TBL_IDX as Tbl_Idx, CODE as Code,
  POWER_FAILURE_TIME as Power_Failure_Time,
  POWER_FAILURE_COST as Power_Failure_Cost,
  PROBABILITY_OF_POWER_FAILURE as Probability_Of_Power_Failure,
  CAPACITY as Capacity, POWER_FACTOR as Power_Factor,
  COEFFICIENT as Coefficient, AVERAGE_UTILIZATION_RATE as Average_Utilization_Rate,
  FACILITY_RECOVERY_TIME as Facility_Recovery_Time, RATED_VOLTAGE as Rated_Voltage,
  AVERAGE_ELECTRICITY_SALES_COST as Average_Electricity_Sales_Cost,
  TRACK_LENGTH as Track_Length, REPLACEMENT_PROBABILITY as Replacement_Probability,
  EQUIPMENT_UNIT_PRICE as Equipment_Unit_Price, FACILITY_CONTRACTING_COST as Facility_Contracting_Cost,
  EMERGENCY_CONSTRUCTION_SURCHARGE_RATE as Emergency_Construction_Surcharge_Rate,
  GENERAL_ACCIDENT as General_Accident, DEAD_ACCIDENT as Dead_Accident,
  GENERAL_COST as General_Cost, DEAD_COST as Dead_Cost,
  SAFETY_SENSITIVITY_COST as Safety_Sensitivity_Cost,
  INSULATION_OIL_AREA as Insulation_Oil_Area, COST as Cost,
  ENVIRONMENTAL_POLLUTION as Environmental_Pollution, POSITION_WEIGHT as Position_Weight,
  CUSTOMER_POWER_OUTAGE_COST as Customer_Power_Outage_Cost, SYSTEM_LOSS_COST as System_Loss_Cost,
  FACILITY_RECOVERY_COST as Facility_Recovery_Cost, LOSS_OF_PROFIT as Loss_Of_Profit,
  SAFETY_ACCIDENT_COMPENSATION_1 as Safety_Accident_Compensation_1,
  SAFETY_ACCIDENT_COMPENSATION_2 as Safety_Accident_Compensation_2,
  TOTAL_COF as Total_Cof, TBL_GETDATE as Tbl_GetDate
FROM COF
WHERE CODE = @Code
ORDER BY TBL_GETDATE DESC, TBL_IDX DESC;";
        using (var db = new DBHelper())
            return db.Conn.QueryFirstOrDefault<COFModel>(sql, new { Code = code });
    }

    public int Insert(COFModel m)
    {
        const string sql = @"
INSERT INTO COF (
  CODE, POWER_FAILURE_TIME, POWER_FAILURE_COST, PROBABILITY_OF_POWER_FAILURE,
  CAPACITY, POWER_FACTOR, COEFFICIENT, AVERAGE_UTILIZATION_RATE,
  FACILITY_RECOVERY_TIME, RATED_VOLTAGE, AVERAGE_ELECTRICITY_SALES_COST,
  TRACK_LENGTH, REPLACEMENT_PROBABILITY, EQUIPMENT_UNIT_PRICE, FACILITY_CONTRACTING_COST,
  EMERGENCY_CONSTRUCTION_SURCHARGE_RATE, GENERAL_ACCIDENT, DEAD_ACCIDENT, GENERAL_COST, DEAD_COST,
  SAFETY_SENSITIVITY_COST, INSULATION_OIL_AREA, COST, ENVIRONMENTAL_POLLUTION, POSITION_WEIGHT,
  CUSTOMER_POWER_OUTAGE_COST, SYSTEM_LOSS_COST, FACILITY_RECOVERY_COST, LOSS_OF_PROFIT,
  SAFETY_ACCIDENT_COMPENSATION_1, SAFETY_ACCIDENT_COMPENSATION_2, TOTAL_COF
) VALUES (
  @Code, @Power_Failure_Time, @Power_Failure_Cost, @Probability_Of_Power_Failure,
  @Capacity, @Power_Factor, @Coefficient, @Average_Utilization_Rate,
  @Facility_Recovery_Time, @Rated_Voltage, @Average_Electricity_Sales_Cost,
  @Track_Length, @Replacement_Probability, @Equipment_Unit_Price, @Facility_Contracting_Cost,
  @Emergency_Construction_Surcharge_Rate, @General_Accident, @Dead_Accident, @General_Cost, @Dead_Cost,
  @Safety_Sensitivity_Cost, @Insulation_Oil_Area, @Cost, @Environmental_Pollution, @Position_Weight,
  @Customer_Power_Outage_Cost, @System_Loss_Cost, @Facility_Recovery_Cost, @Loss_Of_Profit,
  @Safety_Accident_Compensation_1, @Safety_Accident_Compensation_2, @Total_Cof
);";
        using (var db = new DBHelper())
            return db.Conn.Execute(sql, m);
    }

    public int UpdateById(COFModel m)
    {
        const string sql = @"
UPDATE COF SET
  POWER_FAILURE_TIME = @Power_Failure_Time,
  POWER_FAILURE_COST = @Power_Failure_Cost,
  PROBABILITY_OF_POWER_FAILURE = @Probability_Of_Power_Failure,
  CAPACITY = @Capacity, POWER_FACTOR = @Power_Factor, COEFFICIENT = @Coefficient,
  AVERAGE_UTILIZATION_RATE = @Average_Utilization_Rate, FACILITY_RECOVERY_TIME = @Facility_Recovery_Time,
  RATED_VOLTAGE = @Rated_Voltage, AVERAGE_ELECTRICITY_SALES_COST = @Average_Electricity_Sales_Cost,
  TRACK_LENGTH = @Track_Length, REPLACEMENT_PROBABILITY = @Replacement_Probability,
  EQUIPMENT_UNIT_PRICE = @Equipment_Unit_Price, FACILITY_CONTRACTING_COST = @Facility_Contracting_Cost,
  EMERGENCY_CONSTRUCTION_SURCHARGE_RATE = @Emergency_Construction_Surcharge_Rate,
  GENERAL_ACCIDENT = @General_Accident, DEAD_ACCIDENT = @Dead_Accident,
  GENERAL_COST = @General_Cost, DEAD_COST = @Dead_Cost,
  SAFETY_SENSITIVITY_COST = @Safety_Sensitivity_Cost, INSULATION_OIL_AREA = @Insulation_Oil_Area,
  COST = @Cost, ENVIRONMENTAL_POLLUTION = @Environmental_Pollution, POSITION_WEIGHT = @Position_Weight,
  CUSTOMER_POWER_OUTAGE_COST = @Customer_Power_Outage_Cost, SYSTEM_LOSS_COST = @System_Loss_Cost,
  FACILITY_RECOVERY_COST = @Facility_Recovery_Cost, LOSS_OF_PROFIT = @Loss_Of_Profit,
  SAFETY_ACCIDENT_COMPENSATION_1 = @Safety_Accident_Compensation_1,
  SAFETY_ACCIDENT_COMPENSATION_2 = @Safety_Accident_Compensation_2,
  TOTAL_COF = @Total_Cof,
  TBL_GETDATE = GETDATE()
WHERE TBL_IDX = @Tbl_Idx;";
        using (var db = new DBHelper())
            return db.Conn.Execute(sql, m);
    }
    public decimal GetTotalCofByPrefix(string prefix)
    {
        const string sql = @"
    SELECT SUM(TOTAL_COF) AS TotalCof
    FROM COF
    WHERE CODE LIKE @Prefix";

        using (var db = new DBHelper())
        {
            var result = db.Conn.QueryFirstOrDefault<decimal?>(sql, new { Prefix = prefix + "%" });
            return result ?? 0m;
        }
    }

}
