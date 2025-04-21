using System;
using System.Linq;
using AMS_MVC.Models;

namespace AMS_MVC.Services
{
    public class VCBChkScoreCalculator
    {
        public int CalculateFoldingFunction(VCBChk chk)
        {

            int[] scores = new int[]
            {
                (int)chk.CHK_ContactWearPercent,
                (int)chk.CHK_VacuumLeakCurrent,
                (int)chk.CHK_ContactResistance,
                (int)chk.CHK_InsulationResistance,
                (int)chk.CHK_HotSpot,
                (int)chk.CHK_PdPatternValue,
                (int)chk.CHK_MotorCurrent,
                (int)chk.CHK_AccumShortCircuitCurrent,
                (int)chk.CHK_ShortCircuitCount,
                (int)chk.CHK_OperationCount,
                (int)chk.CHK_OpenCloseTime,
                (int)chk.CHK_VisualCheck
            };

            return scores.Max();  
        }
    }
}
