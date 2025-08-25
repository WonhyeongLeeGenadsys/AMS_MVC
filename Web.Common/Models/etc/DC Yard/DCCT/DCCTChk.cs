using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Common
{
    public class DCCTChk
    {
        public int Tbl_Idx { get; set; }
        public string DCCT_Code { get; set; }
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

        //
        public string CHK_1_1 { get; set; } // 내부 점검 및 청소
        public string CHK_2_1 { get; set; } // 수동/부동/균등 충전 및 전압 측정
        public string CHK_3_1 { get; set; } // Drop Ry 동작 시험
        public string CHK_4_1 { get; set; } // 표시 시험 (LED, LCD 등)
        public string CHK_5_1 { get; set; } // 절연저항 측정
        public string CHK_6_1 { get; set; } // 계기 오차 시험
        public string CHK_7_1 { get; set; } // 계전기 시험
        public string CHK_8_1 { get; set; } // AC 입력 및 충전기 절체 기능 점검
        public string CHK_9_1 { get; set; } // DC 출력 전압의 리플 함유율 측정 (필요시)
        public string CHK_10_1 { get; set; } // 노-휴즈 브레이커(NFB) 동작 체크       

        //
        public string CHK_Writer { get; set; } // 작성자        
        public DateTime CHK_Tbl_GetDate { get; set; }

    }
}