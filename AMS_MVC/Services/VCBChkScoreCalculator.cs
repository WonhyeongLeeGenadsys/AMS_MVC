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
                (int)chk.ContactWearPercent,
                (int)chk.VacuumLeakCurrent,
                (int)chk.ContactResistance,
                (int)chk.InsulationResistance,
                (int)chk.HotSpot,
                (int)chk.PdPatternValue,
                (int)chk.MotorCurrent,
                (int)chk.AccumShortCircuitCurrent,
                (int)chk.ShortCircuitCount,
                (int)chk.OperationCount,
                (int)chk.OpenCloseTime,
                (int)chk.VisualCheck
            };

            return scores.Max();  
        }
    }
}
