//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using AMS_MVC.Models;
//using AMS_MVC.Services;
//using System.Linq;

//namespace AMS_MVC.Tests
//{
//    [TestClass]
//    public class ITRChkScoreCalculatorTests
//    {
//        public TestContext TestContext { get; set; }
//        private readonly ITRChkScoreCalculator _calc = new ITRChkScoreCalculator();

//        [TestMethod]
//        public void CalculateFoldingFunction_ITRChk1And2_AllCases()
//        {
//            // ITRChk1
//            int[][] chk1Inputs = new[]
//            {
//                new[] { 3,1,4,4,4,1,1,1,4,1,1,1,1,1,1,1,1,1,1},
//                new[] { 4,1,1,3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
//                new[] { 4,1,1,3,4,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
//                new[] { 1,1,1,4,3,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
//                new[] { 1,1,1,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
//                new[] { 1,1,4,4,4,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
//                new[] { 1,1,1,2,4,1,1,1,1,1,1,1,1,1,1,5,1,1,5},
//                new[] { 1,1,1,2,3,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
//                new[] { 3,5,4,4,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
//                new[] { 1,4,1,1,1,1,1,1,1,1,1,1,1,5,5,5,5,5,5},
//            };
//            int[] chk1Expected = { 4, 4, 4, 4, 2, 4, 5, 3, 5, 5 };

//            for (int i = 0; i < chk1Inputs.Length; i++)
//            {
//                var v = chk1Inputs[i];
//                var chk1 = new ITRChk1
//                {
//                    CHK1_H2 = v[0],
//                    CHK1_C2H2 = v[1],
//                    CHK1_C2H4 = v[2],
//                    CHK1_CH4 = v[3],
//                    CHK1_C2H6 = v[4],
//                    CHK1_CO = v[5],
//                    CHK1_CO2 = v[6],
//                    CHK1_Dielectric_Strength = v[7],
//                    CHK1_Remain_Life = v[8],
//                    CHK1_Age = v[9],
//                    CHK1_Gojang_History = v[10],
//                    CHK1_Doble = v[11],
//                    CHK1_SFRA = v[12],
//                    CHK1_HV_E = v[13],
//                    CHK1_LV_E = v[14],
//                    CHK1_TV_E = v[15],
//                    CHK1_HV_LV = v[16],
//                    CHK1_HV_TV = v[17],
//                    CHK1_LV_TV = v[18],
//                };

//                int actual1 = _calc.CalculateFoldingFunction(chk1);

//                // log
//                TestContext.WriteLine(
//                    $"ITRChk1 Case #{i + 1}: 입력값=[{string.Join(",", v)}], " +
//                    $"기대값={chk1Expected[i]}, 실제값={actual1}"
//                );

//                Assert.AreEqual(
//                    chk1Expected[i],
//                    actual1,
//                    $"ITRChk1 Case #{i + 1} 실패"
//                );
//            }

//            // ITRChk2
//            int[][] chk2Inputs = new[]
//            {
//                new[] { 1,1,1,1,1,1,1 },
//                new[] { 1,1,1,1,1,1,1 },
//                new[] { 1,1,1,1,1,1,1 },
//                new[] { 1,1,1,1,1,1,1 },
//                new[] { 1,1,1,1,1,1,1 },
//                new[] { 1,1,1,1,1,1,1 },
//                new[] { 1,1,1,1,1,1,1 },
//                new[] { 1,1,1,1,1,1,1 },
//                new[] { 1,1,1,1,1,1,1 },
//                new[] { 1,1,1,1,1,1,1 }
//            };
//            int[] chk2Expected = { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

//            for (int i = 0; i < chk2Inputs.Length; i++)
//            {
//                var w = chk2Inputs[i];
//                var chk2 = new ITRChk2
//                {
//                    CHK2_Computerized_Price = w[0],
//                    CHK2_Water_Content = w[1],
//                    CHK2_Furfural = w[2],
//                    CHK2_Excitation_Current = w[3],
//                    CHK2_Short_Current = w[4],
//                    CHK2_Voltage_Ratio = w[5],
//                    CHK2_PD = w[6],
//                };

//                int actual2 = _calc.CalculateFoldingFunction(chk2);

//                // log
//                TestContext.WriteLine(
//                    $"ITRChk2 Case #{i + 1}: 입력값=[{string.Join(",", w)}], " +
//                    $"기대값={chk2Expected[i]}, 실제값={actual2}"
//                );

//                Assert.AreEqual(
//                    chk2Expected[i],
//                    actual2,
//                    $"ITRChk2 Case #{i + 1} 실패"
//                );
//            }
//        }
//    }
//}
