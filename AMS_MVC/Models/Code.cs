using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS_MVC.Models
{
    public class Code
    {
        public int Tbl_Idx { get; set; }
        public string Code_Type { get; set; }
        public string Code_Code { get; set; }
        public string Code_Value { get; set; }
        public DateTime Tbl_GetDate { get; set; }
    }
}