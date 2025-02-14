using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS_MVC.Models
{
    public class Company // 업체 등록 모델
    {
        public int Tbl_Idx { get; set; }
        public string Com_Code { get; set; } 
        public string Com_Name { get; set; }
        public string Com_Address { get; set; }
        public string Com_Phone { get; set; }
        public string Com_Buseo { get; set; }
        public string Com_Email { get; set; }
        public DateTime Tbl_GetDate { get; set; }
    }
}
