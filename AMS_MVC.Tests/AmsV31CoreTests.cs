using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Web.Common;

namespace AMS_MVC.Tests
{
    [TestClass]
    public class AmsV31CoreTests
    {
        [TestMethod]
        public void FoldingFunction_UsesActualWorstGradeFrequency()
        {
            var result = HiPofTable.GetHiPof(3, 2, "VCB");

            Assert.AreEqual(3.04m, result.HI);
            Assert.IsTrue(result.PoF > 0m && result.PoF < 100m);
        }

        [TestMethod]
        public void DiagnosticPof_IncreasesWithHealthIndex()
        {
            double pof1 = AmsV31Config.CalculateDiagnosticPof(1d, "VCB");
            double pof3 = AmsV31Config.CalculateDiagnosticPof(3d, "VCB");
            double pof5 = AmsV31Config.CalculateDiagnosticPof(5d, "VCB");

            Assert.IsTrue(pof1 < pof3);
            Assert.IsTrue(pof3 < pof5);
            Assert.IsTrue(pof5 <= 1d);
        }

        [TestMethod]
        public void Rul_UsesTargetPofPointFive()
        {
            double expected = 30d * Math.Pow(Math.Log(2d), 1d / 3d);
            double actual = AmsV31Config.CalculateRulYears("VCB", 0d);

            Assert.AreEqual(expected, actual, 0.000001d);
        }

        [TestMethod]
        public void AhpWeights_AreNormalizedAndConsistent()
        {
            double[] weights = AmsV31DecisionMath.CalculateAhpWeights();
            double consistencyRatio = AmsV31DecisionMath.CalculateConsistencyRatio(weights);

            Assert.AreEqual(1d, weights.Sum(), 0.000001d);
            Assert.IsTrue(consistencyRatio < 0.1d);
        }

        [TestMethod]
        public void Topsis_RanksDominantAssetFirst()
        {
            var matrix = new[,]
            {
                { 100d, 3d, 5d, 1d, 1d },
                {  10d, 1d, 2d, 9d, 0.3d }
            };

            IList<AmsV31TopsisResult> result = AmsV31DecisionMath.Rank(matrix);

            Assert.AreEqual(1, result[0].Rank);
            Assert.AreEqual(2, result[1].Rank);
        }

        [TestMethod]
        public void EconomicAssessment_BcrAndNpvUseSamePresentValues()
        {
            var costs = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                { "VCB", 200000000d },
                { "DCCB", 1200000000d }
            };
            var calculator = new OriginalDmCalculator(costs);
            var candidates = new List<DmDecisionInfo>
            {
                BuildCandidate(calculator, "VCB", 0.4d, 10),
                BuildCandidate(calculator, "DCCB", 0.7d, 15)
            };

            calculator.ApplyFleetCalculation(candidates);

            foreach (DmDecisionInfo candidate in candidates)
            {
                Assert.AreEqual(candidate.NpvBenefits - candidate.NpvCosts,
                    candidate.NpvValue, 0.01d);
                Assert.AreEqual(candidate.NpvBenefits / candidate.NpvCosts,
                    candidate.Bcr, 0.000001d);
                Assert.AreEqual(candidate.PoFRatio * candidate.CoF,
                    candidate.Risk, 0.01d);
            }
        }

        [TestMethod]
        public void FleetCalculation_PreservesRegisteredMokpoCofForRisk()
        {
            var calculator = new OriginalDmCalculator();
            DmDecisionInfo candidate = BuildCandidate(calculator, "VCB", 0.69591d, 0);
            candidate.CoF = 8589.56d;

            calculator.ApplyFleetCalculation(new List<DmDecisionInfo> { candidate });

            Assert.AreEqual(8589.56d, candidate.CoF, 0.000001d);
            Assert.AreEqual(8589.56d * 0.69591d, candidate.Risk, 0.000001d);
            Assert.IsTrue(candidate.DmCofKrw > 0d);
            Assert.IsTrue(candidate.NpvCosts > 0d);
        }

        [TestMethod]
        public void AdvancedAnalysis_RespectsBudgetAndBuildsFiveYearScenarios()
        {
            var decisions = new List<DmDecisionInfo>
            {
                new DmDecisionInfo
                {
                    Code = "VCB001", ProductName = "VCB #1", EquipmentKey = "VCB",
                    UsageYears = 10, HI = 4.5d, PoFRatio = 0.7d, CoF = 500000000d,
                    Risk = 350000000d, ReplacementCost = 200000000d, Bcr = 5d,
                    RULYears = 2d, Criticality = 0.7d, TopsisScore = 0.8d,
                    Decision = "즉시 교체"
                },
                new DmDecisionInfo
                {
                    Code = "ITR001", ProductName = "ITR #1", EquipmentKey = "ITR",
                    UsageYears = 5, HI = 2d, PoFRatio = 0.1d, CoF = 1000000000d,
                    Risk = 100000000d, ReplacementCost = 800000000d, Bcr = 2d,
                    RULYears = 20d, Criticality = 0.85d, TopsisScore = 0.3d,
                    Decision = "정기점검"
                }
            };

            DmAdvancedAnalysisInfo result = new DmAdvancedAnalysisService()
                .Analyze(decisions, 250000000d);

            Assert.IsTrue(result.Budget.TotalCost <= 250000000d);
            Assert.AreEqual(2, result.Budget.Actions.Count);
            Assert.AreEqual(25, result.Predictions.Count);
            Assert.AreEqual(2, result.Explanations.Count);
            Assert.AreEqual(5, result.MultiYearPlan.Count);
            Assert.AreEqual(2, result.Uncertainty.Count);
            Assert.AreEqual(6, result.Sensitivity.Count);
            Assert.IsTrue(result.InputReadiness.Any(x => x.Feature == "DGA 진단" && !x.Ready));

            double replaceRisk = result.Predictions
                .Single(x => x.Scenario == "replace" && x.YearNo == 1).FleetRisk;
            double noActionRisk = result.Predictions
                .Single(x => x.Scenario == "do_nothing" && x.YearNo == 1).FleetRisk;
            Assert.IsTrue(replaceRisk < noActionRisk);
        }

        private static DmDecisionInfo BuildCandidate(
            OriginalDmCalculator calculator,
            string equipmentKey,
            double pof,
            int age)
        {
            OriginalDmResult raw = calculator.CalculateRaw(equipmentKey, pof, age, 35d, 25d);
            return new DmDecisionInfo
            {
                EquipmentKey = equipmentKey,
                PoFRatio = pof,
                UsageYears = age,
                ReplacementCost = raw.ReplacementCost,
                CofTotalKrw = raw.CofTotalKrw,
                RawCofFinancial = raw.RawCofFinancial,
                RawCofReliability = raw.RawCofReliability,
                RawCofSafety = raw.RawCofSafety,
                RawCofEnvironmental = raw.RawCofEnvironmental
            };
        }
    }
}
