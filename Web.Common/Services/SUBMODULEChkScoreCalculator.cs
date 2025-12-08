using System;
using System.Linq;

namespace Web.Common
{
    public class SUBMODULEChkScoreCalculator
    {
        public (decimal HI, decimal PoF) CalculateHiPof(SUBMODULEChk chk, decimal alpha = 1.00m)
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

            LogHelper.WriteLog("SUBMODULEChkScore",
                 $"[InputData]:  " +
                 $"V_ce={(int)chk.CHK_CE_Voltage}, " +
                 $"V_g={(int)chk.CHK_G_Voltage}, " +
                 $"R_on={(int)chk.CHK_On_Resistance}, " +
                 $"R_th={(int)chk.CHK_Thermal_Resistance}, " +
                 $"I_c={(int)chk.CHK_C_Current}, " +
                 $"T_on,off={(int)chk.CHK_OnOff_Time}, " +
                 $"절연저항={(int)chk.CHK_Insulation_Resistance}, " +
                 $"ESR={(int)chk.CHK_ESR}, " +
                 $"커패시턴스={(int)chk.CHK_Capacitance}, " +
                 $"온도={(int)chk.CHK_Temperature}");

            int maxGrade = scores.Max();
            int frequency = scores.Count(s => s == maxGrade);

            return HiPofTable.GetHiPof(maxGrade, frequency, alpha);
        }
    }
}
