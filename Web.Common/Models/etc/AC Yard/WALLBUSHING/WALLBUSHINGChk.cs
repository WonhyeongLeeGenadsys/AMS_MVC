using System;

namespace Web.Common
{
    public class WALLBUSHINGChk
    {
        public int Tbl_Idx { get; set; }
        public string WALLBUSHING_Code { get; set; }
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

        // 점검 항목
        public string CHK_1_1 { get; set; } // 부싱의 균열 및 오손 여부 확인/청소
        public string CHK_1_2 { get; set; } // 절연유 누유 여부 확인
        public string CHK_1_3 { get; set; } // 절연 열화 진단
        public string CHK_2_1 { get; set; } // 절연유 성능 시험
        public string CHK_2_2 { get; set; } // 유중 가스 분석
        public string CHK_2_3 { get; set; } // 수분 측정

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
        public string CHK_2_3_Text => ToStatusText(CHK_2_3);
    }
}
