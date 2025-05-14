using Microsoft.VisualStudio.TestTools.UnitTesting;
using AMS_MVC.Models;
using AMS_MVC.Services;

namespace AMS_MVC.Tests
{
    [TestClass]
    public class DCCABLEChkScoreCalculatorTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void CalculateFoldingFunction_SampleCases()
        {
            var calc = new DCCABLEChkScoreCalculator();

            int[][] testInputs = new[]
            {
                new[] {2,1,2,2,2},
                new[] {2,1,2,2,2},
                new[] {2,2,2,1,2},

                new[] {3,1,1,2,1},
                new[] {2,4,2,2,1},
                new[] {1,1,1,3,1},

                new[] {2,1,2,1,3},
                new[] {5,5,5,2,1},
                new[] {1,1,2,5,5},

                new[] {5,5,5,5,5},

            };

            int[] expected = { 2, 2, 2, 3, 4, 3, 3, 5, 5, 5 };

            for (int i = 0; i < testInputs.Length; i++)
            {
                var vals = testInputs[i];

                var chk = new DCCABLEChk
                {
                    CHK_Partial_Discharge = vals[0],
                    CHK_Rated_Voltage = vals[1],
                    CHK_Tan_Delta = vals[2],
                    CHK_Resistance = vals[3],
                    CHK_TDR = vals[4],

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
