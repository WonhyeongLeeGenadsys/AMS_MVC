using System;

namespace Web.Common
{
    public class BYPASSVALVEChk
    {
        public int Tbl_Idx { get; set; }
        public string BYPASSVALVE_Code { get; set; }
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
        public string CHK_1_1 { get; set; } // 밸브 주변 누유, 손상, 부식 여부 확인
        public string CHK_2_1 { get; set; } // 밸브 개폐 작동 상태 확인
        public string CHK_3_1 { get; set; } // 밸브 개폐 압력 측정 및 설계 사양 부합 여부 확인
        public string CHK_4_1 { get; set; } // 바이패스 경로 필터 막힘 여부 점검
        public string CHK_5_1 { get; set; } // 정비 이력 및 교체 주기 확인

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
    }
}
