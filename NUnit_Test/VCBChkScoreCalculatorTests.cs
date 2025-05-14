using NUnit.Framework;
using AMS_MVC.Models;
using AMS_MVC.Services;

namespace NUnit_Test
{
    [TestFixture]
    public class VCBChkScoreCalculatorTests
    {
        [Test]
        public void CalculateFoldingFunction_SampleCases()
        {
            var calc = new VCBChkScoreCalculator();


            var testInputs = new[]
            {
                new[] {1,1,1,1,1,1,1,1,1,1,1,1}, 
                new[] {1,1,1,1,1,1,1,1,1,1,1,1},
                new[] {1,1,1,1,1,1,1,1,1,1,1,1}, 
                new[] {1,1,1,1,1,1,4,1,1,1,1,1}, 
                new[] {1,1,1,1,1,1,1,1,1,1,1,1},
                new[] {1,1,1,1,1,1,1,3,1,1,4,1}, 
                new[] {1,1,1,1,1,1,1,1,3,3,4,3}, 
                new[] {1,1,1,1,1,1,1,1,1,4,1,1}, 
                new[] {1,1,1,1,1,5,1,1,1,4,1,1}, 
                new[] {1,1,1,1,1,1,1,1,1,5,1,1},
            };

            var expected = new[] { 1, 1, 1, 4, 3, 4, 4, 4, 5, 5 };

            for (int i = 0; i < testInputs.Length; i++)
            {
                var vals = testInputs[i];
                var chk = new VCBChk
                {
                    CHK_ContactWearPercent = vals[0],
                    CHK_VacuumLeakCurrent = vals[1],
                    CHK_ContactResistance = vals[2],
                    CHK_InsulationResistance = vals[3],
                    CHK_HotSpot = vals[4],
                    CHK_PdPatternValue = vals[5],
                    CHK_MotorCurrent = vals[6],
                    CHK_AccumShortCircuitCurrent = vals[7],
                    CHK_ShortCircuitCount = vals[8],
                    CHK_OperationCount = vals[9],
                    CHK_OpenCloseTime = vals[10],
                    CHK_VisualCheck = vals[11],
                };

                int actual = calc.CalculateFoldingFunction(chk);
                Assert.That(actual,
                            Is.EqualTo(expected[i]),
                            $"Case #{i + 1}: 기대 {expected[i]}인데 실제 {actual} 입니다.");
            }
        }
    }
}
