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
                (int)chk.CHK_MainCircuit_PD,
                (int)chk.CHK_Machine_Part_Operation_Time,
                (int)chk.CHK_Mechanical_Vibration_acceleration,
                (int)chk.CHK_Relay_Auxiliary_Contact_Resistance,
                (int)chk.CHK_CE_Voltage,
                (int)chk.CHK_G_Voltage,
                (int)chk.CHK_C_Current,
                (int)chk.CHK_OnOff_Time,
            };

            LogHelper.WriteLog("DCCBChkScore",
                 $"[InputData]:  " +
                 $"주 회로 절연내력 시험={(int)chk.CHK_MainCircuit_InsulationStrength}, " +
                 $"주 회로 부분방전={(int)chk.CHK_MainCircuit_PD}, " +
                 $"기계부 동작시간={(int)chk.CHK_Machine_Part_Operation_Time}, " +
                 $"기계부 진동/가속도={(int)chk.CHK_Mechanical_Vibration_acceleration}, " +
                 $"릴레이 보조접점 저항={(int)chk.CHK_Relay_Auxiliary_Contact_Resistance}, " +
                 $"V_ce={(int)chk.CHK_CE_Voltage}, " +
                 $"V_g={(int)chk.CHK_G_Voltage}, " +
                 $"I_c={(int)chk.CHK_C_Current}, " +
                 $"T_on,off={(int)chk.CHK_OnOff_Time}");

            int maxGrade = scores.Max();
            int frequency = scores.Count(s => s == maxGrade);

            return HiPofTable.GetHiPof(maxGrade, frequency, alpha);
        }
    }
}
