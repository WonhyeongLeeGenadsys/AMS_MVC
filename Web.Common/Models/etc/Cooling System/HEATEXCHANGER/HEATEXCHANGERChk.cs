using System;

namespace Web.Common
{
    public class HEATEXCHANGERChk
    {
        public int Tbl_Idx { get; set; }
        public string HEATEXCHANGER_Code { get; set; }
        public string CHK_Gongsa_Name { get; set; }
        public string CHK_Weather { get; set; }
        public string CHK_Temp { get; set; }
        public string CHK_Hum { get; set; }
        public string CHK_Company { get; set; }
        public string CHK_Worker { get; set; }
        public string CHK_Manager { get; set; }
        public string CHK_Urgent_No { get; set; }
        public string CHK_Type { get; set; }
        public DateTime? CHK_Start_Date { get; set; }
        public DateTime? CHK_End_Date { get; set; }

        // ---------------------------
        // 점검 항목
        // ---------------------------
        public string CHK_1_1 { get; set; } // 열교환기 표면의 부식, 손상, 누수 여부 확인
        public string CHK_1_2 { get; set; } // 배관 연결부, 밸브, 계측기 등의 이상 유무 확인
        public string CHK_1_3 { get; set; } // 주변 설비의 이상 유무 확인
        public string CHK_2_1 { get; set; } // 공급/환수 온도 측정 및 변화 확인
        public string CHK_2_2 { get; set; } // 공급/환수 압력 측정 및 변화 확인
        public string CHK_3_1 { get; set; } // 공급/환수 유량 변화 확인
        public string CHK_4_1 { get; set; } // 이상 소음 및 진동 여부 확인

        public string CHK_Writer { get; set; } // 작성자        
        public DateTime CHK_Tbl_GetDate { get; set; }


        private string ToStatusText(string value)
        {
            switch (value)
            {
                case "1": return "정상";
                case "5": return "이상";
                default: return "-";
            }
        }

        public string CHK_1_1_Text => ToStatusText(CHK_1_1);
        public string CHK_1_2_Text => ToStatusText(CHK_1_2);
        public string CHK_1_3_Text => ToStatusText(CHK_1_3);
        public string CHK_2_1_Text => ToStatusText(CHK_2_1);
        public string CHK_2_2_Text => ToStatusText(CHK_2_2);
        public string CHK_3_1_Text => ToStatusText(CHK_3_1);
        public string CHK_4_1_Text => ToStatusText(CHK_4_1);
    }
}
