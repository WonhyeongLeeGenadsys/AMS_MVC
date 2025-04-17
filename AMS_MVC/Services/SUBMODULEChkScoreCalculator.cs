using System;
using System.Linq;
using AMS_MVC.Models;

namespace AMS_MVC.Services
{
    public class SUBMODULEChkScoreCalculator
    {
        public int CalculateFoldingFunction(SUBMODULEChk chk)
        {

            int[] scores = new int[]
            {
                (int)chk.CHK_CE_Voltage,
                (int)chk.CHK_G_Voltage,
                (int)chk.CHK_On_Resistance,
                (int)chk.CHK_Thermal_Resistance,
                (int)chk.CHK_C_Current,
                (int)chk.CHK_OnOff_Time,
                (int)chk.CHK_Insulation_Resistance,
                (int)chk.CHK_ESR,
                (int)chk.CHK_Capacitance,
                (int)chk.CHK_Temperature,
            };

            return scores.Max();  
        }
    }
}
