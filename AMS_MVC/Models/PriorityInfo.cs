using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS_MVC.Models
{
    public class PriorityInfo
    {
        public int Priority { get; set; } //우선순위
        public string Sort { get; set; }          // 구분 (AC/DC)
        public string Code { get; set; }          // VCB_CODE 또는 ITR_CODE
        public string Serial_No { get; set; }      // 시리얼 번호
        public string Name { get; set; }          // 이름
        public DateTime Install_Date { get; set; } // 설치일
        public DateTime Operating_Date { get; set; } // 운영일
        public int UsagePeriod { get; set; }      // 사용기간 (일수)
        public float Price { get; set; }        // 가격
        public float Rated_V { get; set; } // 정격 전압
        public float Rated_A { get; set; } // 정격 전류
        public string Make_Company { get; set; }  // 제작사
        public string Writer { get; set; }  // 작성자
        public string CoF { get; set; }
        public string PoF { get; set; }
    }
}