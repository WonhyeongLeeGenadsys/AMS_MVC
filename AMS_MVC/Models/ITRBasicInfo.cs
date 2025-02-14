using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS_MVC.Models
{
    public class ITRBasicInfo
    {
        public int Tbl_Idx { get; set; }
        public string ITR_Code { get; set; } // ITR 코드
        public string Serial_No { get; set; } // 시리얼 번호 
        public string Name { get; set; } // 제품 이름
        public DateTime? Install_Date { get; set; } // 설치일 
        public DateTime? Operating_Date { get; set; } // 가동일 
        public float Price { get; set; } // 구입 비용 
        public string Install_Place { get; set; } // 설치 장소
        public float Capacity { get; set; } // 용량 
        public float Rated_V { get; set; } // 정격 전압 
        public float Rated_A { get; set; } // 정격 전류 
        public string Constant { get; set; } // 상수
        public string Make_Company { get; set; } // 제작사 
        public string Make_No { get; set; } // 제작 번호 
        public string Photo { get; set; } // 제품 사진 
        public bool Is_Diagnostics { get; set; } // 진단 사용 여부 
        public bool Is_Health { get; set; } // 건전도 사용 여부 
        public string Writer { get; set; } // 작성자 
        public DateTime Tbl_GetDate { get; set; } // 작성일 
    }
}