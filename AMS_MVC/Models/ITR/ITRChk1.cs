using System;

namespace AMS_MVC.Models
{
    public class ITRChk1
    {
        public int Tbl_Idx { get; set; }
        public string ITR_Code { get; set; }

        // 공통 정보
        public string CHK1_Gongsa_Name { get; set; }    // 공사명
        public string CHK1_Weather { get; set; }    // 날씨
        public string CHK1_Temp { get; set; }    // 기온 
        public string CHK1_Hum { get; set; }    // 습도 
        public string CHK1_Company { get; set; }    // 업체명
        public string CHK1_Worker { get; set; }    // 작업자
        public string CHK1_Manager { get; set; }    // 감독자
        public string CHK1_Urgent_No { get; set; }    // 급전번호
        public string CHK1_Type { get; set; }    // 점검 형식
        public DateTime? CHK1_Start_Date { get; set; }
        public DateTime? CHK1_End_Date { get; set; }

        // --- DGA 가스 분석 ---
        public string CHK1_H2 { get; set; }
        public string CHK1_C2H2 { get; set; }
        public string CHK1_C2H4 { get; set; }
        public string CHK1_CH4 { get; set; }
        public string CHK1_C2H6 { get; set; }
        public string CHK1_CO { get; set; }
        public string CHK1_CO2 { get; set; }

        public string CHK1_Dielectric_Strength { get; set; } // 유전체강도

        // --- 절연 손상／노후도 ---
        public string CHK1_Remain_Life { get; set; } // 잔여수명
        public string CHK1_Age { get; set; } // 나이
        public string CHK1_Gojang_History { get; set; } // 고장이력

        // --- 절연진동／기계적 시험 ---
        public string CHK1_Doble { get; set; }
        public string CHK1_SFRA { get; set; }
        
        // --- 절연저항 측정 (HV‑E, LV‑E, HV‑LV, HV‑TV, LV‑TV) ---
        public string CHK1_HV_E { get; set; }
        public string CHK1_LV_E { get; set; }
        public string CHK1_TV_E { get; set; }
        public string CHK1_HV_LV { get; set; }
        public string CHK1_HV_TV { get; set; }
        public string CHK1_LV_TV { get; set; }

        // 작성자·날짜
        public string CHK1_Writer { get; set; }
        public DateTime CHK1_Tbl_GetDate { get; set; }
    }
}
