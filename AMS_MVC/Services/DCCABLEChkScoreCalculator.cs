using System;
using System.Linq;
using AMS_MVC.Models;

namespace AMS_MVC.Services
{
    public class DCCABLEChkScoreCalculator
    {
        public int CalculateFoldingFunction(DCCABLEChk chk)
        {

            int[] scores = new int[]
            {
                (int)chk.CHK_Partial_Discharge,
                (int)chk.CHK_Rated_Voltage,
                (int)chk.CHK_Tan_Delta,
                (int)chk.CHK_Resistance,
                (int)chk.CHK_TDR,
               
            };

            return scores.Max();  
        }
    }
}
