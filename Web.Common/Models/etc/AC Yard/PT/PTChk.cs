using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Common
{
    public class PTChk
    {
        public int Tbl_Idx { get; set; }
        public string PT_Code { get; set; }
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
        public string CHK_1_1 { get; set; } // 애관부 균열 및 오손 여부 점검/청소
        public string CHK_2_1 { get; set; } // 1차 단자부 볼트 조임
        public string CHK_2_2 { get; set; } // 2차 단자함 내부 점검 및 볼트 조임 
        public string CHK_3_1 { get; set; } // 절연저항 / 절연열화 측정 / 포화시험
        public string CHK_4_1 { get; set; } //콘덴서형 전압변성기 보호캡 점검
        public string CHK_4_2 { get; set; } // 호흡기 상태 점검
        public string CHK_5_1 { get; set; } // 케이블 고정부 청소 및 이완 여부 점검
        public string CHK_6_1 { get; set; } // 접지선 접속부 조임 상태 점검
        public string CHK_7_1 { get; set; } // Oil Level 점검

        //
        public string CHK_Writer { get; set; } // 작성자     
        public DateTime CHK_Tbl_GetDate { get; set; }

    }
}