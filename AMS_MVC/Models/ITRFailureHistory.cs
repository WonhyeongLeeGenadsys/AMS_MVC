using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS_MVC.Models
{
    public class ITRFailureHistory
    {
        public int Tbl_Idx { get; set; }
        public string ITR_Code { get; set; }
        public string Fail_Gojang_Name { get; set; }
        public string Fail_Weather { get; set; }
        public string Fail_Temp { get; set; }
        public string Fail_Hum { get; set; }
        public string Fail_Cause { get; set; }
        public string Fail_Reason { get; set; }
        public string Fail_Status { get; set; }
        public string Fail_Part { get; set; }
        public string Fail_Period { get; set; }
        public string Fail_Finder { get; set; }
        public string Fail_Repairer { get; set; }
        public string Fail_Supervisor { get; set; }
        public string Fail_Registrar { get; set; }
        public DateTime? Fail_Date { get; set; }
        public string Fail_Writer { get; set; }
        public DateTime? Fail_Tbl_GetDate { get; set; }
    }
}