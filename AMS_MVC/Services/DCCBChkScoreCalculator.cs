using System;
using System.Linq;
using AMS_MVC.Models;

namespace AMS_MVC.Services
{
    public class DCCBChkScoreCalculator
    {
        public int CalculateFoldingFunction(DCCBChk chk)
        {

            int[] scores = new int[]
            {
                (int)chk.CHK_MainCircuit_InsulationStrength,
                (int)chk.CHK_LeakTest,
                (int)chk.CHK_MechanicalOperation,
                (int)chk.CHK_AuxControlCircuit,
                (int)chk.CHK_CE_Voltage,
                (int)chk.CHK_G_Voltage,
                (int)chk.CHK_On_Resistance,
                (int)chk.CHK_Thermal_Resistance,
                (int)chk.CHK_C_Current,
                (int)chk.CHK_OnOff_Time,
            };

            return scores.Max();  
        }
    }
}
