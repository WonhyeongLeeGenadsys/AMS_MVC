using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS_MVC.Models
{
    public class ITRMaintenanceHistory
    {
        public int Tbl_Idx { get; set; }
        public string ITR_Code { get; set; }
        public string MR_Bosu_Name { get; set; }
        public string MR_Weather { get; set; }
        public string MR_Temp { get; set; }
        public string MR_Hum { get; set; }
        public string MR_Content { get; set; }
        public string MR_Status { get; set; }
        public string MR_Part { get; set; }
        public string MR_Company { get; set; }
        public string MR_Worker { get; set; }
        public string MR_Manager { get; set; }
        public DateTime? MR_Date { get; set; }
        public string MR_Writer { get; set; }
        public DateTime MR_Tbl_GetDate { get; set; }
    }
}