using System;
using AMS_MVC.Models;

namespace AMS_MVC.Services
{
    /// <summary>
    /// COFModel의 입력값을 사용해 계산 결과 속성들을 채워 주는 서비스 클래스
    /// </summary>
    public class CoFCalculator
    {
        /// <summary>
        /// COFModel에 있는 순수 decimal 속성들을 그대로 사용하여 
        /// 6가지 비용/손실 항목을 계산하고, 결과를 모델에 설정합니다.
        /// </summary>
        public void Calculate(COFModel m)
        {
            if (m == null) throw new ArgumentNullException(nameof(m));

            //
            // (1) 고객 정전 비용
            //     = (정전비용 × 용량 × 정전시간) × (정전발생확률 / 100)
            //
            m.Customer_Power_Outage_Cost =
                m.Power_Failure_Cost
                * m.Capacity
                * m.Power_Failure_Time
                * (m.Probability_Of_Power_Failure / 100m);

            //
            // (2) 계통 손실 비용
            //     = (용량 × 평균 이용률/100) × 선로길이 × 평균 전력판매비용 × 설비 복구 시간
            //
            var actualLoad = m.Capacity * (m.Average_Utilization_Rate / 100m);
            m.System_Loss_Cost =
                actualLoad
                * m.Track_Length
                * m.Average_Electricity_Sales_Cost
                * m.Facility_Recovery_Time;

            //
            // (3) 설비 복구 비용
            //     = (설비 단가 × 교체확률/100 + 설비 도급비용 × 교체확률/100)
            //       × (1 + 긴급공사 할증률/100)
            //
            var replacePartCost =
                m.Equipment_Unit_Price * (m.Replacement_Probability / 100m);
            var contractPartCost =
                m.Facility_Contracting_Cost * (m.Replacement_Probability / 100m);
            m.Facility_Recovery_Cost =
                (replacePartCost + contractPartCost)
                * (1m + (m.Emergency_Construction_Surcharge_Rate / 100m));

            //
            // (4) 전력 판매 수익 손실
            //     = (용량 × 역률/100) × 정전시간 × 평균 전력판매비용 × (정전발생확률 / 100)
            //
            var actualPowerOut = m.Capacity * (m.Power_Factor / 100m);
            m.Loss_Of_Profit =
                actualPowerOut
                * m.Power_Failure_Time
                * m.Average_Electricity_Sales_Cost
                * (m.Probability_Of_Power_Failure / 100m);

            //
            // (5) 안전사고 보상
            //     = ((일반사고 확률/100 × 일반사고비용)
            //        + (사망사고 확률/100 × 사망사고비용))
            //       × 안전 민감 계수
            //
            var safetyPart =
                (m.General_Accident / 100m) * m.General_Cost
                + (m.Dead_Accident / 100m) * m.Dead_Cost;
            m.Safety_Accident_Compensation_1 =
                safetyPart * m.Safety_Sensitivity_Cost;

            //
            // (6) 환경사고 보상
            //     = (절연유 유출면적 × 보상비용) 
            //       × (환경오염확률/100) × 위치가중치
            //
            var oilTotal = m.Insulation_Oil_Area * m.Cost;
            m.Safety_Accident_Compensation_2 =
                oilTotal
                * (m.Environmental_Pollution / 100m)
                * m.Position_Weight;
        }
    }
}
