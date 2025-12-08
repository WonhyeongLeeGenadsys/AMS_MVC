using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Common
{
    public class Riskmatrix
    {
        public int Tbl_Idx { get; set; }
        public string Code { get; set; }
        public decimal Cof { get; set; }
        public decimal Pof { get; set; }
        public string HI { get; set; }
        public DateTime LastTime { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}