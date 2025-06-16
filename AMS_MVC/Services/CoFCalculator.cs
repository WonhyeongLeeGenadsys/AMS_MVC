using System;
using AMS_MVC.Models;

namespace AMS_MVC.Services
{
    public class CoFCalculator
    {
        private const decimal SQRT3 = 1.732050807568877293m;

        public void Calculate(COFModel m)
        {
            if (m == null)
                throw new ArgumentNullException(nameof(m));

            switch (m.Code)
            {
                case "VCB":
                    CalculateForVCB(m);
                    break;
                case "ITR":
                    CalculateForITR(m);
                    break;
                case "DCCB":
                    CalculateForDCCB(m);
                    break;
                case "DCCABLE":
                    CalculateForDCCABLE(m);
                    break;
                case "SUBMODULE":
                    CalculateForSUBMODULE(m);
                    break;
                default:
                    CalculateDefault(m);
                    break;
            }
        }

        private void CalculateForVCB(COFModel m)
        {
            DoCommonCalculation(m);
        }

        private void CalculateForITR(COFModel m)
        {
            DoCommonCalculation(m);
        }

        private void CalculateForDCCB(COFModel m)
        {
            DoCommonCalculation(m);
        }

        private void CalculateForDCCABLE(COFModel m)
        {
            DoCommonCalculation(m);
        }

        private void CalculateForSUBMODULE(COFModel m)
        {
            DoCommonCalculation(m);
        }

        private void CalculateDefault(COFModel m)
        {
            DoCommonCalculation(m);
        }

        // 헬퍼 메서드: 분모가 0이면 0, 아니면 나눗셈
        private decimal SafeDivide(decimal numerator, decimal denominator)
            => denominator == 0m ? 0m : numerator / denominator;

        private void DoCommonCalculation(COFModel m)
        {
            // (1) 고객 정전 비용
            m.Customer_Power_Outage_Cost =
                m.Power_Failure_Cost
              * (m.Power_Factor / 100m)
              * (m.Coefficient / 100m)
              * m.Power_Failure_Time
              * m.Capacity
              * (m.Probability_Of_Power_Failure / 100m);

            // (2) 계통 손실 비용 
            decimal denom = SQRT3 * m.Rated_Voltage;
            m.System_Loss_Cost =
                SafeDivide(m.Capacity * 1000m, denom)
              * (m.Average_Utilization_Rate / 100m)
              * m.Track_Length
              * m.Facility_Recovery_Time
              * m.Average_Electricity_Sales_Cost;

            // (3) 설비 복구 비용
            m.Facility_Recovery_Cost =
                (m.Equipment_Unit_Price + (m.Facility_Contracting_Cost * (m.Emergency_Construction_Surcharge_Rate / 100m))) * ((m.Replacement_Probability / 100m)* m.Power_Failure_Time);

            // (4) 전력 판매 수익 손실
            m.Loss_Of_Profit =
                (m.Capacity * m.Average_Utilization_Rate / 100m * m.Power_Failure_Time)
              * m.Average_Electricity_Sales_Cost
              * (m.Probability_Of_Power_Failure / 100m)
              * 1000m;

            // (5) 안전사고 보상
            m.Safety_Accident_Compensation_1 =
                ((m.General_Accident / 100m) * m.General_Cost)
              + ((m.Dead_Accident / 100m) * m.Dead_Cost)
              * m.Safety_Sensitivity_Cost;

            // (6) 환경사고 보상
            m.Safety_Accident_Compensation_2 =
                m.Insulation_Oil_Area
              * m.Cost
              * (m.Environmental_Pollution / 100m)
              * m.Position_Weight;

            // (7) 최종 COF 합산
            m.Total_Cof =
                m.Customer_Power_Outage_Cost
              + m.System_Loss_Cost
              + m.Facility_Recovery_Cost
              + m.Loss_Of_Profit
              + m.Safety_Accident_Compensation_1
              + m.Safety_Accident_Compensation_2;
        }
    }
}
