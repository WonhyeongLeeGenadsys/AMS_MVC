// AMS_MVC/Services/VCBChkScoreCalculator.cs

using System.Linq;

namespace Web.Common
{
    public class VCBChkScoreCalculator
    {
        public (decimal HI, decimal PoF) CalculateHiPof(VCBChk chk, decimal alpha = 0.99m)
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
                (int)chk.CHK_VisualCheck,
                (int)chk.CHK_ThermalTemperature
            };

            LogHelper.WriteLog("VCBChkScore",
                 $"[InputData]:  " +
                 $"접점 소모량={(int)chk.CHK_ContactWearPercent}, " +
                 $"진공(누설전류)={(int)chk.CHK_VacuumLeakCurrent}, " +
                 $"접촉저항={(int)chk.CHK_ContactResistance}, " +
                 $"절연저항={(int)chk.CHK_InsulationResistance}, " +
                 $"핫스팟={(int)chk.CHK_HotSpot}, " +
                 $"PD 패턴 및 성장={(int)chk.CHK_PdPatternValue}, " +
                 $"구동모터 전류={(int)chk.CHK_MotorCurrent}, " +
                 $"누적 단락전류량={(int)chk.CHK_AccumShortCircuitCurrent}, " +
                 $"단락 전류={(int)chk.CHK_ShortCircuitCount}, " +
                 $"동작 횟수={(int)chk.CHK_OperationCount}, " +
                 $"개폐 시간={(int)chk.CHK_OpenCloseTime}, " +
                 $"외관 점검={(int)chk.CHK_VisualCheck}, " +
                 $"열화상 및 온도={(int)chk.CHK_ThermalTemperature}");

            int maxGrade = scores.Max(); // Max 값이 HI 값 
            int frequency = scores.Count(s => s == maxGrade); // Max값으로 빈도수 찾아서 해당하는 Pof값 반환

            return HiPofTable.GetHiPof(maxGrade, frequency, "VCB", alpha); 
        }
    }
}
