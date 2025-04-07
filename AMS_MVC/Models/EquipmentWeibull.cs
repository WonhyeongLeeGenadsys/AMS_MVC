using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AMS_MVC.Models
{
    [Table("EquipmentWeibull")]
    public class EquipmentWeibull
    {
        [Key]
        public int Id { get; set; }

        public string Category { get; set; }       // 예: "DC", "AC", ...
        public string EquipmentName { get; set; }  // 예: "DC Cable", "DCCB", "VCB" 등
        public double? ShapeParam { get; set; }    // 형상모수
        public double? ScaleParam { get; set; }    // 척도모수
        public double? FailureRate { get; set; }   // 고장률
    }
}