using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS_MVC.Models
{
    public class SAChk
    {
        public int Tbl_Idx { get; set; }
        public string SA_Code { get; set; }
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
        public string CHK_1_1 { get; set; } // 도면 일치 여부, 접지선 접속 및 노후 상태, 설치 상태 확인
        public string CHK_2_1 { get; set; } // 접지 저항 측정 및 기준값 이내 여부 확인
        public string CHK_3_1 { get; set; } // 절연 저항 측정 (전선 및 기기 절연 성능 확인)
        public string CHK_4_1 { get; set; } // 절연 내력 시험 (고전압 인가)
        public string CHK_5_1 { get; set; } // 보호 계전기 동작 시험 (과전류, 과전압 등)
        public string CHK_6_1 { get; set; } // 절연유 내압 및 산가 시험 (유입 변압기)
        public string CHK_7_1 { get; set; } // 차단기 개폐 동작 및 차단 성능 확인
        public string CHK_8_1 { get; set; } // 발전기 출력 및 성능 운전 시험
        public string CHK_9_1 { get; set; } // 전기설비 외관, 작동, 기능 일상 점검
        public string CHK_10_1 { get; set; } // 누전 차단기 동작 확인
        public string CHK_11_1 { get; set; } // 전원 차단 및 안전 수칙 준수 여부 확인
        public string CHK_12_1 { get; set; } // 점검 결과 기록 및 보관


        //
        public string CHK_Writer { get; set; } // 작성자         
        public DateTime CHK_Tbl_GetDate { get; set; }

    }
}