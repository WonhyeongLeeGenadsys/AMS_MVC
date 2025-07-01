using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS_MVC.Models
{
    public class ZIGZAGTRChk
    {
        public int Tbl_Idx { get; set; }
        public string ZIGZAGTR_Code { get; set; }
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
        public string CHK_1_1 { get; set; } // 변압기 본체, 냉각 장치, 단자함 파손/부식/변형/누유 확인
        public string CHK_2_1 { get; set; } // 절연 저항 및 부분 방전 측정
        public string CHK_3_1 { get; set; } // 접지 저항 측정 및 접지선 연결 상태 확인
        public string CHK_4_1 { get; set; } // 본체, 권선, 오일 온도 측정 및 과열 여부 확인
        public string CHK_5_1 { get; set; } // 이상 소음 유무 확인
        public string CHK_6_1 { get; set; } // 절연유 레벨, 색상, 절연 내력 확인 (유입식)
        public string CHK_7_1 { get; set; } // 1차 및 2차 권선 저항 측정
        public string CHK_8_1 { get; set; } // 임피던스 전압, 단락 전류 측정
        public string CHK_9_1 { get; set; } // 운전 중 권선 온도 상승률 측정
        public string CHK_10_1 { get; set; } // 보호장치, 냉각장치, 부싱 상태 점검

        //
        public string CHK_Writer { get; set; } // 작성자         
        public DateTime CHK_Tbl_GetDate { get; set; }

    }
}