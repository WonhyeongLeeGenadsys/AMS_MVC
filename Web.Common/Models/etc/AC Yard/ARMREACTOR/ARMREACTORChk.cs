using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Common
{
    public class ARMREACTORChk
    {
        public int Tbl_Idx { get; set; }
        public string ARMREACTOR_Code { get; set; }
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
        public string CHK_1_1 { get; set; } // 보조기기류의 외부 점검
        public string CHK_1_2 { get; set; } // 붓싱류의 균열, 오손 여부, 누유 확인 및 청소

        public string CHK_2_1 { get; set; } // 주회로 단자 조임 확인
        public string CHK_3_1 { get; set; } // 제어 회로 결선 상태 및 접속부 이완 여부 확인
        public string CHK_4_1 { get; set; } // 절연저항 측정 (BCT 포함)
        public string CHK_5_1 { get; set; } // 각종 경보 확인 시험
        public string CHK_6_1 { get; set; } // 각종 부속장치 동작 상태 점검 및 청소
        public string CHK_7_1 { get; set; } // 가스 성분 분석 (가스변압기 경우)
        public string CHK_8_1 { get; set; } // GAS 압력 측정 
        public string CHK_9_1 { get; set; } // 각종 시험 (성능 및 보호 시험 등)
        public string CHK_10_1 { get; set; } // 가스 블로워 동작 및 윤활유 교체 (가스변압기, 진공형)
        public string CHK_11_1 { get; set; } // OLTC 구동장치 동작 상태 및 조작함 점검
        public string CHK_12_1 { get; set; } // OLTC 절연유의 절연내력 시험 (진공형) 기계적 보호장치 동작 특성 시험
        public string CHK_13_1 { get; set; } // 기계적 보호장치 동작 특성 시험

        //

        public string CHK_Writer { get; set; } // 작성자     

        public DateTime CHK_Tbl_GetDate { get; set; }
    }
}