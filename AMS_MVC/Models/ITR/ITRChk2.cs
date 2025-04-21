using System;

namespace AMS_MVC.Models
{
    public class ITRChk2
    {
        public int Tbl_Idx { get; set; }
        public string ITR_Code { get; set; }

        // 공통 정보
        public string CHK2_Gongsa_Name { get; set; }    // 공사명
        public string CHK2_Weather { get; set; }    // 날씨
        public string CHK2_Temp { get; set; }    // 기온 
        public string CHK2_Hum { get; set; }    // 습도 
        public string CHK2_Company { get; set; }    // 업체명
        public string CHK2_Worker { get; set; }    // 작업자
        public string CHK2_Manager { get; set; }    // 감독자
        public string CHK2_Urgent_No { get; set; }    // 급전번호
        public string CHK2_Type { get; set; }    // 점검 형식
        public DateTime? CHK2_Start_Date { get; set; }
        public DateTime? CHK2_End_Date { get; set; }

        public string CHK2_Acid_Value { get; set; } // 산가

        // --- 기존 정밀점검 변수 ---
        public string CHK2_Computerized_Price { get; set; } // 전산가
        public string CHK2_Water_Content { get; set; } // 수분함량
        public string CHK2_Furfural { get; set; } // Furfural (플ural)
        public string CHK2_Excitation_Current { get; set; } // 여자 전류
        public string CHK2_Short_Current { get; set; } // 단락 전류
        public string CHK2_Voltage_Ratio { get; set; } // 전압비
        public string CHK2_PD { get; set; } // 부분 방전

        // 작성자·날짜
        public string CHK2_Writer { get; set; }
        public DateTime CHK2_Tbl_GetDate { get; set; }
    }
}
