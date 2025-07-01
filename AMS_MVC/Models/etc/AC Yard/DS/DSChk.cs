using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS_MVC.Models
{
    public class DSChk
    {
        public int Tbl_Idx { get; set; }
        public string DS_Code { get; set; }
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
        public string CHK_1_1 { get; set; } // 개폐 동작 상태 확인
        public string CHK_2_1 { get; set; } // 조작기구함 내 청소 및 볼트 조임
        public string CHK_3_1 { get; set; } // 접속부 마모 상태 점검 및 청소
        public string CHK_4_1 { get; set; } // 애자 균열, 오손 여부 점검 및 청소
        public string CHK_5_1 { get; set; } // 각 연결부 볼트 조임
        public string CHK_6_1 { get; set; } // 각종 시험 (절연시험, 동작시험 등)
        public string CHK_7_1 { get; set; } // 기어부의 그리스 제거 및 재도포

        //
        public string CHK_Writer { get; set; } // 작성자       
        public DateTime CHK_Tbl_GetDate { get; set; }

    }
}