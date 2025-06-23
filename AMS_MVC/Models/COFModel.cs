using System;

namespace AMS_MVC.Models
{
    public class COFModel
    {
        public int Tbl_Idx { get; set; }  
        public string Code { get; set; }
        public decimal Power_Failure_Time { get; set; }             // 정전 시간 (h)
        public decimal Power_Failure_Cost { get; set; }             // 정전 비용 (원/kWh 등)
        public decimal Probability_Of_Power_Failure { get; set; }   // 정전 발생 확률 (%)
        public decimal Capacity { get; set; }                       // 용량 (kW)
        public decimal Power_Factor { get; set; }                   // 역률 (%)
        public decimal Coefficient { get; set; }                    // 계수 (추가 계산용, 예: 손실계수 등)

        public decimal Average_Utilization_Rate { get; set; }       // 평균 이용률 (%)
        public decimal Facility_Recovery_Time { get; set; }         // 설비 복구 시간 (h)
        public decimal Rated_Voltage { get; set; }                  // 정격 전압 (kV)
        public decimal Average_Electricity_Sales_Cost { get; set; } // 평균 전력 판매 비용 (원/kWh)
        public decimal Track_Length { get; set; }                   // 선로 길이 (km)

        public decimal Replacement_Probability { get; set; }        // 교체 확률 (%)
        public decimal Equipment_Unit_Price { get; set; }           // 설비 단가 (원)
        public decimal Facility_Contracting_Cost { get; set; }      // 설비 도급 비용 (원)
        public decimal Emergency_Construction_Surcharge_Rate { get; set; } // 긴급 공사 할증률 (%)

        public decimal General_Accident { get; set; }               // 안전사고 발생 확률 [일반] (%)
        public decimal Dead_Accident { get; set; }                  // 안전사고 발생 확률 [사망] (%)
        public decimal General_Cost { get; set; }                   // 안전사고 보상비용 [일반] (원)
        public decimal Dead_Cost { get; set; }                      // 안전사고 보상비용 [사망] (원)
        public decimal Safety_Sensitivity_Cost { get; set; }        // 안전 민감 계수 (무차원)

        public decimal Insulation_Oil_Area { get; set; }            // 절연유 유출면적 (㎥)
        public decimal Cost { get; set; }                           // 환경사고 보상 비용 (원/㎥)
        public decimal Environmental_Pollution { get; set; }        // 환경 오염 확률 (%)
        public decimal Position_Weight { get; set; }                // 위치 가중치 (무차원)

        // 계산 결과
        public decimal Customer_Power_Outage_Cost { get; set; }     // 고객 정전 비용 (원)
        public decimal System_Loss_Cost { get; set; }               // 계통 손실 비용 (원)
        public decimal Facility_Recovery_Cost { get; set; }         // 설비 복구 비용 (원)
        public decimal Loss_Of_Profit { get; set; }                 // 전력 판매 수익 손실 (원)
        public decimal Safety_Accident_Compensation_1 { get; set; } // 안전사고 보상 (원)
        public decimal Safety_Accident_Compensation_2 { get; set; } // 환경사고 보상 (원)

        public decimal Total_Cof { get; set; }                      // CoF 총 결과값
        public DateTime Tbl_GetDate { get; set; }                   // 입력/수정 일시
    }
}

