using System;
using System.Linq;
using AMS_MVC.Models;
using AMS_MVC.Utlity;

namespace AMS_MVC.Services
{
    public class DCCABLEChkScoreCalculator
    {
        public(decimal HI, decimal PoF) CalculateHiPof(DCCABLEChk chk, decimal alpha = 0.99m)        
        {

            int[] scores = new int[]
            {
                (int)chk.CHK_Partial_Discharge,
                (int)chk.CHK_Rated_Voltage,
                (int)chk.CHK_Tan_Delta,
                (int)chk.CHK_Resistance,
                (int)chk.CHK_TDR,
               
            };

            int maxGrade = scores.Max();
            int frequency = scores.Count(s => s == maxGrade);
            return HiPofTable.GetHiPof(maxGrade, frequency, alpha);
        }
    }
}
