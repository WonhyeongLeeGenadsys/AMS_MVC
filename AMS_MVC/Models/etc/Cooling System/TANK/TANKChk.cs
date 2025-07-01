using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS_MVC.Models
{
    public class TANKChk
    {
        public int Tbl_Idx { get; set; }
        public string TANK_Code { get; set; }
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
        public string CHK_1_1 { get; set; } // 탱크 표면 및 연결 부위 누유, 부식, 손상 여부 확인
        public string CHK_1_2 { get; set; } // 밸브 및 계측기 상태 점검
        public string CHK_2_1 { get; set; } // 압력, 온도 정상 범위 작동 여부 확인
        public string CHK_2_2 { get; set; } // 유량 정상 작동 여부 확인
        public string CHK_3_1 { get; set; } // 안전밸브, 방폭 장치 작동 상태 확인
        public string CHK_4_1 { get; set; } // 위험 물질 및 화재 위험 요인 여부 확인

        //
        public string CHK_Writer { get; set; } // 작성자
        public DateTime CHK_Tbl_GetDate { get; set; }

    }
}