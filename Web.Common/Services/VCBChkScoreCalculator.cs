// AMS_MVC/Services/VCBChkScoreCalculator.cs

using System.Linq;

namespace Web.Common
{
    public class VCBChkScoreCalculator
    {
        public (decimal HI, decimal PoF) CalculateHiPof(VCBChk chk, decimal alpha = 1.00m) //alpha는 보정계수 0.99 고정
        {
            int[] scores = new[]
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

            int maxGrade = scores.Max(); // Max 값이 HI 값 
            int frequency = scores.Count(s => s == maxGrade); // Max값으로 빈도수 찾아서 해당하는 Pof값 반환

            return HiPofTable.GetHiPof(maxGrade, frequency, alpha); 
        }
    }
}
