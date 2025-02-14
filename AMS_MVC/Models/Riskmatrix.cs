using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS_MVC.Models
{
    public class Riskmatrix
    {
        public int Tbl_Idx { get; set; }
        public string Code { get; set; }
        public string Cof { get; set; }
        public string Pof { get; set; }
    }
}