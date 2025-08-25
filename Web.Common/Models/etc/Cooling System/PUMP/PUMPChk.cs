using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Common
{
    public class PUMPChk
    {
        public int Tbl_Idx { get; set; }
        public string PUMP_Code { get; set; }
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
        public string CHK_1_1 { get; set; } // 펌프 본체, 연결부, 베이스 부식/손상/누수 여부 확인
        public string CHK_1_2 { get; set; } // 모터 표면의 부식 및 손상 확인
        public string CHK_2_1 { get; set; } // 정상 작동 여부 및 이상 소음, 진동 확인
        public string CHK_2_2 { get; set; } // 흡입/토출 유량 및 압력, 탱크 내 액위 확인
        public string CHK_3_1 { get; set; } // 제어반 수동 기동 후 전체 계통 정상 여부 확인
        public string CHK_4_1 { get; set; } // 밸브, 필터, 압력계 등 부속품 작동 상태 점검
        public string CHK_5_1 { get; set; } // 흡입/토출 유량 및 압력 측정
        public string CHK_6_1 { get; set; } // 점검 주기 및 결과 기록, 상태 변화 추적

        //
        public string CHK_Writer { get; set; } // 작성자         
        public DateTime CHK_Tbl_GetDate { get; set; }

    }
}