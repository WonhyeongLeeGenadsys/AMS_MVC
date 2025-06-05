using System;
using System.Data;
using Dapper;
using AMS_MVC.Database;
using AMS_MVC.Models;

namespace AMS_MVC.Repositories
{
    public class CoFRepository
    {
        /// <summary>
        /// TB_COF 테이블에서 최신(입력/수정일자가 가장 큰) 한 건을 가져옵니다.
        /// </summary>
        public COFModel GetLatest()
        {
            const string sql = @"
                SELECT TOP 1
                    Tbl_Idx,
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
                    Tbl_GetDate
                FROM TB_COF
                ORDER BY Tbl_GetDate DESC;
            ";

            using (var db = new DBHelper())
            {
                // Dapper의 QueryFirstOrDefault<COFModel>은 컬럼 이름 ↔ 모델 속성 이름 매핑을 자동으로 수행합니다.
                var result = db.Conn.QueryFirstOrDefault<COFModel>(sql);
                return result ?? new COFModel();
            }
        }

        /// <summary>
        /// 새로운 COFModel을 TB_COF 테이블에 INSERT 합니다.
        /// Tbl_GetDate는 GETDATE()로 자동 설정됩니다.
        /// </summary>
        public void Insert(COFModel m)
        {
            const string sql = @"
                INSERT INTO TB_COF (
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
                    Tbl_GetDate
                ) VALUES (
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
                    GETDATE()
                );
            ";

            using (var db = new DBHelper())
            {
                // 익명의 객체에 COFModel 속성을 그대로 넘기면, Dapper가 매핑해 줍니다.
                db.Conn.Execute(sql, new
                {
                    m.Power_Failure_Time,
                    m.Power_Failure_Cost,
                    m.Probability_Of_Power_Failure,
                    m.Capacity,
                    m.Power_Factor,
                    m.Coefficient,
                    m.Average_Utilization_Rate,
                    m.Facility_Recovery_Time,
                    m.Rated_Voltage,
                    m.Average_Electricity_Sales_Cost,
                    m.Track_Length,
                    m.Replacement_Probability,
                    m.Equipment_Unit_Price,
                    m.Facility_Contracting_Cost,
                    m.Emergency_Construction_Surcharge_Rate,
                    m.General_Accident,
                    m.Dead_Accident,
                    m.General_Cost,
                    m.Dead_Cost,
                    m.Safety_Sensitivity_Cost,
                    m.Insulation_Oil_Area,
                    m.Cost,
                    m.Environmental_Pollution,
                    m.Position_Weight,
                    m.Customer_Power_Outage_Cost,
                    m.System_Loss_Cost,
                    m.Facility_Recovery_Cost,
                    m.Loss_Of_Profit,
                    m.Safety_Accident_Compensation_1,
                    m.Safety_Accident_Compensation_2
                });
            }
        }
    }
}
