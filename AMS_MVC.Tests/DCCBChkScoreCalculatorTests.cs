using Microsoft.VisualStudio.TestTools.UnitTesting;
using AMS_MVC.Models;
using AMS_MVC.Services;

namespace AMS_MVC.Tests
{
    [TestClass]
    public class DCCBChkScoreCalculatorTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void CalculateFoldingFunction_SampleCases()
        {
            var calc = new DCCBChkScoreCalculator();

            int[][] testInputs = new[]
            {
                new[] {1,1,2,1,2,2,1,2,2,1},
                new[] {1,1,1,1,2,2,2,2,2,2},
                new[] {1,1,1,1,1,2,1,1,2,2},

                new[] {1,1,1,1,3,2,3,2,2,2},
                new[] {1,1,1,2,2,3,1,2,2,2},
                new[] {4,1,2,2,2,2,1,1,2,2},

                new[] {1,4,1,1,2,2,2,1,1,2},
                new[] {1,1,2,1,5,2,5,5,5,5},
                new[] {5,5,5,1,2,2,1,2,2,1},

                new[] {1,1,2,5,2,5,1,2,1,5},
            };

            int[] expected = { 2,2,2,3,3,4,4,5,5,5 };

            for (int i = 0; i < testInputs.Length; i++)
            {
                var vals = testInputs[i];

                var chk = new DCCBChk
                {
                    CHK_MainCircuit_InsulationStrength = vals[0],
                    CHK_LeakTest = vals[1],
                    CHK_MechanicalOperation = vals[2],
                    CHK_AuxControlCircuit = vals[3],
                    CHK_CE_Voltage = vals[4],
                    CHK_G_Voltage = vals[5],
                    CHK_On_Resistance = vals[6],
                    CHK_Thermal_Resistance = vals[7],
                    CHK_C_Current = vals[8],
                    CHK_OnOff_Time = vals[9],
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
