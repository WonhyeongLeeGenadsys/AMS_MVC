using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS_MVC.Models
{
    public class EquipmentGroupInfo
    {
        public string EquipmentType { get; set; }  
        public DateTime? OperatingDate { get; set; }

        public int UsageYears
        {
            get
            {
                if (OperatingDate.HasValue)
                {
                    return DateTime.Now.Year - OperatingDate.Value.Year;
                }
                return 0;
            }
        }
    }
}