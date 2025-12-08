using System;
using System.Linq;


namespace Web.Common
{
    public class DCCBChkScoreCalculator
    {
        public (decimal HI, decimal PoF) CalculateHiPof(DCCBChk chk, decimal alpha= 1.00m)
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

            LogHelper.WriteLog("DCCBChkScore",
                 $"[InputData]:  " +
                 $"주 회로 절연내력 시험={(int)chk.CHK_MainCircuit_InsulationStrength}, " +
                 $"기밀 시험={(int)chk.CHK_MechanicalOperation}, " +
                 $"기계적 동작 시험 (속도)={(int)chk.CHK_MechanicalOperation}, " +
                 $"보조/제어회로 시험 (응답시간)={(int)chk.CHK_AuxControlCircuit}, " +
                 $"V_ce={(int)chk.CHK_CE_Voltage}, " +
                 $"V_g={(int)chk.CHK_G_Voltage}, " +
                 $"R_on={(int)chk.CHK_On_Resistance}, " +
                 $"I_c={(int)chk.CHK_C_Current}, " +
                 $"T_on,off={(int)chk.CHK_OnOff_Time}");

            int maxGrade = scores.Max();
            int frequency = scores.Count(s => s == maxGrade);

            return HiPofTable.GetHiPof(maxGrade, frequency, alpha);
        }
    }
}
