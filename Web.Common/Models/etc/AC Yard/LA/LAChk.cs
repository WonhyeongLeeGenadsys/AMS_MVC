using System;

namespace Web.Common
{
    public class LAChk
    {
        public int Tbl_Idx { get; set; }
        public string LA_Code { get; set; }
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
        public string CHK_1_1 { get; set; } // 애관의 균열, 오손 여부 점검 및 청소 
        public string CHK_2_1 { get; set; } // 접속부 볼트 조임
        public string CHK_3_1 { get; set; } // 가대 및 기초 볼트 조임
        public string CHK_4_1 { get; set; } // 부속장치 동작 상태 점검 및 청소
        public string CHK_5_1 { get; set; } // 절연저항 측정
        public string CHK_5_2 { get; set; } // 절연열화(Doble) 측정 
        public string CHK_5_3 { get; set; } // 동작 카운터 점검
        public string CHK_5_4 { get; set; } // 누설전류 측정 

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
        public string CHK_2_1_Text => ToStatusText(CHK_2_1);
        public string CHK_3_1_Text => ToStatusText(CHK_3_1);
        public string CHK_4_1_Text => ToStatusText(CHK_4_1);
        public string CHK_5_1_Text => ToStatusText(CHK_5_1);
        public string CHK_5_2_Text => ToStatusText(CHK_5_2);
        public string CHK_5_3_Text => ToStatusText(CHK_5_3);
        public string CHK_5_4_Text => ToStatusText(CHK_5_4);
    }
}
