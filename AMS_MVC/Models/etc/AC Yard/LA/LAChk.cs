using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS_MVC.Models
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

        //
        public string CHK_1_1 { get; set; } // 애관의균열,오손여부점검및청소 
        public string CHK_2_1 { get; set; } // 접속부볼트조임
        public string CHK_3_1 { get; set; } // 가대및기초볼트조임
        public string CHK_4_1 { get; set; } // 부속장치동작상태점검및청소
        public string CHK_5_1 { get; set; } // 절연저항측정
        public string CHK_5_2 { get; set; } // 절연열화(Doble)측정 
        public string CHK_5_3 { get; set; } // 동작카운터점검
        public string CHK_5_4 { get; set; } // 누설전류측정 

        //
        public string CHK_Writer { get; set; } // 작성자     
        public DateTime CHK_Tbl_GetDate { get; set; }

    }
}