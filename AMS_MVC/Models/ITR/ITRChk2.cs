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

        // --- 기존 정밀점검 변수 ---
        public int CHK2_Computerized_Price { get; set; } // 전산가
        public int CHK2_Water_Content { get; set; } // 수분함량
        public int CHK2_Furfural { get; set; } // Furfural (플ural)
        public int CHK2_Excitation_Current { get; set; } // 여자 전류
        public int CHK2_Short_Current { get; set; } // 단락 전류
        public int CHK2_Voltage_Ratio { get; set; } // 전압비
        public int CHK2_PD { get; set; } // 부분 방전
        public int FoldingFunction { get; set; }

        // 작성자·날짜
        public string CHK2_Writer { get; set; }
        public DateTime CHK2_Tbl_GetDate { get; set; }

        public string CompPriceText
        {
            get
            {
                switch (CHK2_Computerized_Price)
                {
                    case 1: return "≤0.1";
                    case 3: return "≤0.3";
                    case 5: return ">0.3";
                    default: return "-";
                }
            }
        }
        public string WaterContentText
        {
            get
            {
                switch (CHK2_Water_Content)
                {
                    case 1: return "<40";
                    case 3: return "≤50";
                    case 5: return ">50";
                    default: return "-";
                }
            }
        }
        public string FurfuralText
        {
            get
            {
                switch (CHK2_Furfural)
                {
                    case 1: return "NOT";
                    case 5: return "OCCUR";
                    default: return "-";
                }
            }
        }
        public string ExcitationCurrentText
        {
            get
            {
                switch (CHK2_Excitation_Current)
                {
                    case 1: return "20%↑";
                    case 5: return "100%↑";
                    default: return "-";
                }
            }
        }
        public string ShortCurrentText
        {
            get
            {
                switch (CHK2_Short_Current)
                {
                    case 1: return "≤20%";
                    case 5: return ">20%";
                    default: return "-";
                }
            }
        }
        public string VoltageRatioText
        {
            get
            {
                switch (CHK2_Voltage_Ratio)
                {
                    case 1: return "≤±5%";
                    case 5: return ">±5%";
                    default: return "-";
                }
            }
        }
        public string PDText
        {
            get
            {
                switch (CHK2_PD)
                {
                    case 1: return "NOT";
                    case 5: return "OCCUR";
                    default: return "-";
                }
            }
        }
    }
}
