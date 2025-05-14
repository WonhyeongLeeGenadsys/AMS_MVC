using Microsoft.VisualStudio.TestTools.UnitTesting;
using AMS_MVC.Models;
using AMS_MVC.Services;
using System.Linq;

namespace AMS_MVC.Tests
{
    [TestClass]
    public class VCBChkScoreCalculatorTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void CalculateFoldingFunction_SampleCases()
        {
            var calc = new VCBChkScoreCalculator();

            int[][] testInputs = new[]
            {
                new[] {1,1,1,1,1,1,1,1,1,1,1,1},
                new[] {1,1,1,1,1,1,1,1,1,1,1,1},
                new[] {1,1,1,1,1,1,1,1,1,1,1,1},
                new[] {1,1,1,1,1,1,4,1,1,2,1,1},
                new[] {1,1,1,3,1,1,1,3,3,2,1,1},
                new[] {3,1,1,1,1,1,1,1,1,3,4,3},
                new[] {1,1,1,1,1,1,1,1,1,3,1,4},
                new[] {1,1,1,1,1,1,1,1,1,4,1,1},
                new[] {1,1,1,5,5,5,1,1,1,4,1,1},
                new[] {1,1,1,1,1,1,1,1,1,5,1,1},
            };

            int[] expected = { 1, 1, 1, 4, 3, 4, 4, 4, 5, 5 };

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

                TestContext.WriteLine(
                    $"Case #{i + 1}: 입력값=[{string.Join(",", vals)}], " +
                    $"기대값={expected[i]}, 실제값={actual}"
                );

                Assert.AreEqual(expected[i], actual,
                    $"Case #{i + 1} 오류");
            }
        }
    }
}
