using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS_MVC.Models
{
    public class NGRChk
    {
        public int Tbl_Idx { get; set; }
        public string NGR_Code { get; set; }
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
        public string CHK_1_1 { get; set; } // 외함 온도 상승 및 이음 여부
        public string CHK_1_2 { get; set; } // 부싱 균열 및 오손 확인/청소
        public string CHK_1_3 { get; set; } // 절연유 누유 여부
        public string CHK_2_1 { get; set; } // 접지선 점검
        public string CHK_2_2 { get; set; } // 각 연결부 볼트 조임
        public string CHK_3_1 { get; set; } // 호흡기 상태/절연저항 측정 등 


        //
        public string CHK_Writer { get; set; } // 작성자         
        public DateTime CHK_Tbl_GetDate { get; set; }

    }
}