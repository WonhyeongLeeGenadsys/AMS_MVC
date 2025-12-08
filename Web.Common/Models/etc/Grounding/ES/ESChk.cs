using System;

namespace Web.Common
{
    public class ESChk
    {
        public int Tbl_Idx { get; set; }
        public string ES_Code { get; set; }
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

        // --------------------
        // 점검 항목
        // --------------------
        public string CHK_1_1 { get; set; } // 케이스 손상, 부식, 변색 여부 확인
        public string CHK_1_2 { get; set; } // 냉각 팬 및 통풍구 막힘 여부 확인
        public string CHK_1_3 { get; set; } // 단자대 및 배선 손상 여부 점검
        public string CHK_1_4 { get; set; } // 접지 상태 확인
        public string CHK_2_1 { get; set; } // 전압, 전류, 저항 측정 및 정상 범위 확인
        public string CHK_2_2 { get; set; } // 절연 저항 측정 및 정상 범위 확인
        public string CHK_2_3 { get; set; } // 배터리 전압, 내부 저항 측정
        public string CHK_3_1 { get; set; } // 작동 온도 및 습도 확인
        public string CHK_3_2 { get; set; } // 온도 센서 및 습도 센서 작동 확인
        public string CHK_4_1 { get; set; } // 통신 프로토콜 작동 확인
        public string CHK_4_2 { get; set; } // 원격 제어 기능 점검
        public string CHK_4_3 { get; set; } // 안전 장치 (과전압, 과전류, 과열 등) 점검
        public string CHK_5_1 { get; set; } // 부품 마모 및 노후화 점검
        public string CHK_5_2 { get; set; } // 먼지 및 이물질 축적 여부 확인
        public string CHK_6_1 { get; set; } // 계통 연계형: 전력 품질 및 계통 연계 상태 점검
        public string CHK_6_2 { get; set; } // 독립형: 배터리 충/방전 상태 및 수명 점검
        public string CHK_7_1 { get; set; } // 보호 장치 작동 여부 확인

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
        public string CHK_1_4_Text => ToStatusText(CHK_1_4);
        public string CHK_2_1_Text => ToStatusText(CHK_2_1);
        public string CHK_2_2_Text => ToStatusText(CHK_2_2);
        public string CHK_2_3_Text => ToStatusText(CHK_2_3);
        public string CHK_3_1_Text => ToStatusText(CHK_3_1);
        public string CHK_3_2_Text => ToStatusText(CHK_3_2);
        public string CHK_4_1_Text => ToStatusText(CHK_4_1);
        public string CHK_4_2_Text => ToStatusText(CHK_4_2);
        public string CHK_4_3_Text => ToStatusText(CHK_4_3);
        public string CHK_5_1_Text => ToStatusText(CHK_5_1);
        public string CHK_5_2_Text => ToStatusText(CHK_5_2);
        public string CHK_6_1_Text => ToStatusText(CHK_6_1);
        public string CHK_6_2_Text => ToStatusText(CHK_6_2);
        public string CHK_7_1_Text => ToStatusText(CHK_7_1);
    }
}
