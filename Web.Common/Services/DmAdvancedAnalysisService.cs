using System;
using System.Collections.Generic;
using System.Linq;

namespace Web.Common
{
    /// <summary>
    /// MVDC AMS v3.1 Budget Optimizer, Predictive Twin and deterministic
    /// decision trace. The budget model uses the same three actions and
    /// objective coefficients as bo_budget_optimizer.py.
    /// </summary>
    public sealed class DmAdvancedAnalysisService
    {
        private const double DefaultBudget = 6000000000d;
        private const double MinAllocation = 10000000d;
        private const double MaxAllocation = 2500000000d;
        private const int HorizonYears = 5;

        public DmAdvancedAnalysisInfo Analyze(
            IList<DmDecisionInfo> source,
            double? requestedBudget = null)
        {
            var decisions = (source ?? new List<DmDecisionInfo>())
                .Where(x => x != null)
                .ToList();

            return new DmAdvancedAnalysisInfo
            {
                Budget = OptimizeBudget(decisions, requestedBudget.GetValueOrDefault(DefaultBudget)),
                Predictions = PredictFleet(decisions),
                Explanations = Explain(decisions),
                MultiYearPlan = BuildMultiYearPlan(decisions, requestedBudget.GetValueOrDefault(DefaultBudget)),
                Uncertainty = CalculateUncertainty(decisions),
                Sensitivity = CalculateSensitivity(decisions),
                InputReadiness = BuildInputReadiness(decisions),
                PredictionHorizonYears = HorizonYears,
                CalculationVersion = "MVDC AMS v3.1.0"
            };
        }

        public DmBudgetOptimizationInfo OptimizeBudget(
            IList<DmDecisionInfo> decisions,
            double budget)
        {
            budget = budget > 0d ? budget : DefaultBudget;
            var optionRows = decisions.Select(BuildOptions).ToList();
            var states = new List<BudgetState>
            {
                new BudgetState { Cost = 0L, Benefit = 0d, Actions = new List<DmBudgetActionInfo>() }
            };

            foreach (List<DmBudgetActionInfo> options in optionRows)
            {
                var nextByCost = new Dictionary<long, BudgetState>();
                foreach (BudgetState state in states)
                {
                    foreach (DmBudgetActionInfo option in options)
                    {
                        long cost = state.Cost + Convert.ToInt64(Math.Round(option.Cost));
                        if (cost > budget)
                        {
                            continue;
                        }

                        double benefit = state.Benefit + option.Benefit;
                        BudgetState current;
                        if (!nextByCost.TryGetValue(cost, out current) || benefit > current.Benefit)
                        {
                            var actions = new List<DmBudgetActionInfo>(state.Actions) { option };
                            nextByCost[cost] = new BudgetState
                            {
                                Cost = cost,
                                Benefit = benefit,
                                Actions = actions
                            };
                        }
                    }
                }

                // Remove states that cost more without yielding a higher benefit.
                double bestBenefit = double.MinValue;
                states = new List<BudgetState>();
                foreach (BudgetState state in nextByCost.Values.OrderBy(x => x.Cost))
                {
                    if (state.Benefit > bestBenefit + 0.0001d)
                    {
                        states.Add(state);
                        bestBenefit = state.Benefit;
                    }
                }
            }

            BudgetState best = states
                .OrderByDescending(x => x.Benefit)
                .ThenBy(x => x.Cost)
                .FirstOrDefault() ?? new BudgetState
                {
                    Actions = new List<DmBudgetActionInfo>()
                };

            return new DmBudgetOptimizationInfo
            {
                TotalBudget = budget,
                TotalCost = best.Cost,
                TotalBenefit = best.Benefit,
                RemainingBudget = Math.Max(0d, budget - best.Cost),
                UtilizationPct = budget > 0d ? best.Cost / budget * 100d : 0d,
                ReplaceCount = best.Actions.Count(x => x.Action == "REPLACE"),
                MaintainCount = best.Actions.Count(x => x.Action == "MAINTAIN"),
                DoNothingCount = best.Actions.Count(x => x.Action == "DO_NOTHING"),
                Actions = best.Actions
                    .OrderByDescending(x => x.Benefit)
                    .ThenBy(x => x.Code)
                    .ToList()
            };
        }

        private static List<DmBudgetActionInfo> BuildOptions(DmDecisionInfo item)
        {
            return new[]
            {
                BuildOption(item, "DO_NOTHING", 0d, 0d),
                BuildOption(item, "MAINTAIN", 0.15d, 0.30d),
                BuildOption(item, "REPLACE", 1.00d, 0.95d)
            }.ToList();
        }

        private static DmBudgetActionInfo BuildOption(
            DmDecisionInfo item,
            string action,
            double costMultiplier,
            double pofReduction)
        {
            double cost = item.ReplacementCost * costMultiplier;
            if (action != "DO_NOTHING" && cost > 0d)
            {
                cost = Math.Max(MinAllocation, Math.Min(MaxAllocation, cost));
            }

            double bcrNormalized = Math.Min(Math.Max(item.Bcr, 0d) / 30d, 1d);
            double benefit = (0.7d * item.Risk * pofReduction)
                + (0.3d * bcrNormalized * cost * pofReduction);

            return new DmBudgetActionInfo
            {
                Code = item.Code,
                ProductName = item.ProductName,
                Action = action,
                Cost = cost,
                Benefit = benefit,
                BenefitCostRatio = cost > 0d ? benefit / cost : 0d
            };
        }

        private static List<DmPredictionInfo> PredictFleet(IList<DmDecisionInfo> decisions)
        {
            var scenarios = new[]
            {
                new Scenario("do_nothing", "현 상태 유지"),
                new Scenario("replace", "즉시 교체"),
                new Scenario("maintain", "정비 실시"),
                new Scenario("upgrade", "신기술 교체"),
                new Scenario("phased_replace", "단계적 교체")
            };
            var results = new List<DmPredictionInfo>();
            int baseYear = DateTime.Now.Year;

            foreach (Scenario scenario in scenarios)
            {
                for (int year = 1; year <= HorizonYears; year++)
                {
                    var predictedPofs = decisions
                        .Select(x => PredictPof(x, scenario.Code, year))
                        .ToList();
                    double risk = decisions
                        .Select((x, index) => x.CoF * predictedPofs[index])
                        .Sum();
                    results.Add(new DmPredictionInfo
                    {
                        Scenario = scenario.Code,
                        ScenarioName = scenario.Name,
                        YearNo = year,
                        CalendarYear = baseYear + year,
                        FleetRisk = risk,
                        AveragePofPct = predictedPofs.Count > 0
                            ? predictedPofs.Average() * 100d
                            : 0d,
                        AverageHi = predictedPofs.Count > 0
                            ? predictedPofs.Average(x => Math.Max(1d, Math.Min(5d, 1d + x * 4d)))
                            : 0d
                    });
                }
            }

            return results;
        }

        private static double PredictPof(DmDecisionInfo item, string action, int year)
        {
            AmsV31AssetConfig config = AmsV31Config.GetAsset(item.EquipmentKey);
            double age = Math.Max(0d, item.UsageYears);
            double scale = config.WeibullScale;

            if (action == "replace" || action == "upgrade")
            {
                age = 0d;
                if (action == "upgrade")
                {
                    scale *= 1.2d;
                }
            }
            else if (action == "phased_replace" && year >= 4)
            {
                age = year - 3d;
                year = 0;
            }

            double futureAge = age + year;
            return AmsV31Config.Clamp(
                1d - Math.Exp(-Math.Pow(futureAge / scale, config.WeibullShape)),
                0d,
                1d);
        }

        private static List<DmExplanationInfo> Explain(IList<DmDecisionInfo> decisions)
        {
            double[] weights = AmsV31DecisionMath.CalculateAhpWeights();
            double minRisk = Min(decisions, x => x.Risk);
            double maxRisk = Max(decisions, x => x.Risk);
            double minBcr = Min(decisions, x => x.Bcr);
            double maxBcr = Max(decisions, x => x.Bcr);
            double minRul = Min(decisions, x => x.RULYears.GetValueOrDefault());
            double maxRul = Max(decisions, x => x.RULYears.GetValueOrDefault());
            var result = new List<DmExplanationInfo>();

            foreach (DmDecisionInfo item in decisions)
            {
                double risk = Normalize(item.Risk, minRisk, maxRisk) * weights[0];
                double bcr = Normalize(item.Bcr, minBcr, maxBcr) * weights[1];
                double hi = Normalize(item.HI, 1d, 5d) * weights[2];
                double rul = (1d - Normalize(item.RULYears.GetValueOrDefault(), minRul, maxRul)) * weights[3];
                double criticality = Normalize(item.Criticality, 0d, 1d) * weights[4];
                double pof = item.PoFRatio;
                var factors = new Dictionary<string, double>
                {
                    { "Risk", risk },
                    { "BCR", bcr },
                    { "HI", hi },
                    { "RUL", rul },
                    { "중요도", criticality },
                    { "PoF", pof }
                };
                string topFactor = factors.OrderByDescending(x => x.Value).First().Key;
                string forcedRule = GetForcedRule(item);
                string explanation = string.IsNullOrEmpty(forcedRule)
                    ? string.Format("AHP-TOPSIS {0:F4}점으로 '{1}'이 산정되었으며, 가장 큰 영향요인은 {2}입니다.", item.TopsisScore, item.Decision, topFactor)
                    : string.Format("{0}에 따라 의사결정은 '{1}'로 우선 적용되었습니다. TOPSIS 점수는 {2:F4}입니다.", forcedRule, item.Decision, item.TopsisScore);

                result.Add(new DmExplanationInfo
                {
                    Code = item.Code,
                    ProductName = item.ProductName,
                    Decision = item.Decision,
                    TopFactor = topFactor,
                    Explanation = explanation,
                    RiskContribution = risk,
                    BcrContribution = bcr,
                    HiContribution = hi,
                    RulContribution = rul,
                    CriticalityContribution = criticality,
                    PofContribution = pof
                });
            }

            return result;
        }

        private static List<DmMultiYearPlanInfo> BuildMultiYearPlan(
            IList<DmDecisionInfo> decisions,
            double annualBudget)
        {
            annualBudget = annualBudget > 0d ? annualBudget : DefaultBudget;
            var assets = decisions.Select(item =>
            {
                AmsV31AssetConfig config = AmsV31Config.GetAsset(item.EquipmentKey);
                double pof = AmsV31Config.Clamp(item.PoFRatio, 0.000001d, 0.999999d);
                double effectiveAge = config.WeibullScale
                    * Math.Pow(-Math.Log(1d - pof), 1d / config.WeibullShape);
                return new MultiYearAsset
                {
                    Code = item.Code,
                    BaseCost = item.ReplacementCost,
                    Cof = item.CoF,
                    Shape = config.WeibullShape,
                    Scale = config.WeibullScale,
                    EffectiveAge = effectiveAge,
                    InitialEffectiveAge = effectiveAge
                };
            }).ToList();

            var rows = new List<DmMultiYearPlanInfo>();
            double cumulativeCost = 0d;
            double cumulativeBenefit = 0d;
            int calendarYear = DateTime.Now.Year;
            for (int year = 1; year <= HorizonYears; year++)
            {
                double discount = 1d / Math.Pow(1d + AmsV31Config.DiscountRate, year - 1);
                var beforePof = assets.Where(x => !x.Replaced)
                    .ToDictionary(x => x.Code, x => WeibullPof(x.EffectiveAge, x.Scale, x.Shape));
                var tier1 = new List<MultiYearCandidate>();
                var tier2 = new List<MultiYearCandidate>();

                foreach (MultiYearAsset asset in assets.Where(x => !x.Replaced))
                {
                    double pof = beforePof[asset.Code];
                    double risk = pof * asset.Cof;
                    if (pof >= 0.65d || (asset.MaintenanceCount >= 3 && pof >= 0.60d))
                    {
                        tier1.Add(new MultiYearCandidate(asset, "REPLACE", asset.BaseCost,
                            risk * 0.95d * discount, risk));
                    }
                    else
                    {
                        tier2.Add(new MultiYearCandidate(asset, "MAINTAIN", asset.BaseCost * 0.15d,
                            risk * 0.30d * discount, risk));
                    }
                }

                IEnumerable<MultiYearCandidate> ordered = tier1
                    .OrderByDescending(x => x.Risk)
                    .ThenBy(x => x.Asset.Code)
                    .Concat(tier2
                        .OrderByDescending(x => x.Cost > 0d ? x.Benefit / x.Cost : 0d)
                        .ThenBy(x => x.Asset.Code));
                double spent = 0d;
                double riskReduction = 0d;
                int replaceCount = 0;
                int maintainCount = 0;
                var acted = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (MultiYearCandidate candidate in ordered)
                {
                    if (acted.Contains(candidate.Asset.Code) || candidate.Cost <= 0d
                        || spent + candidate.Cost > annualBudget + 0.000001d)
                    {
                        continue;
                    }

                    spent += candidate.Cost;
                    riskReduction += candidate.Benefit;
                    acted.Add(candidate.Asset.Code);
                    if (candidate.Action == "REPLACE")
                    {
                        candidate.Asset.Replaced = true;
                        candidate.Asset.EffectiveAge = 0d;
                        candidate.Asset.MaintenanceCount = 0;
                        replaceCount++;
                    }
                    else
                    {
                        double floor = candidate.Asset.InitialEffectiveAge * 0.5d;
                        candidate.Asset.EffectiveAge = Math.Max(
                            candidate.Asset.EffectiveAge - 3d,
                            floor);
                        candidate.Asset.MaintenanceCount++;
                        maintainCount++;
                    }
                }

                int backlog = assets.Count(x => !x.Replaced
                    && beforePof.ContainsKey(x.Code)
                    && beforePof[x.Code] > 0.60d);
                cumulativeCost += spent;
                cumulativeBenefit += riskReduction;
                rows.Add(new DmMultiYearPlanInfo
                {
                    YearNo = year,
                    CalendarYear = calendarYear + year - 1,
                    Budget = annualBudget,
                    Spent = spent,
                    UtilizationPct = annualBudget > 0d ? spent / annualBudget * 100d : 0d,
                    ReplaceCount = replaceCount,
                    MaintainCount = maintainCount,
                    UrgentBacklog = backlog,
                    RiskReduction = riskReduction,
                    CumulativeCost = cumulativeCost,
                    CumulativeBenefit = cumulativeBenefit
                });

                foreach (MultiYearAsset asset in assets.Where(x => !x.Replaced))
                {
                    asset.EffectiveAge += 1d;
                }
            }

            return rows;
        }

        private static List<DmUncertaintyInfo> CalculateUncertainty(
            IList<DmDecisionInfo> decisions)
        {
            var result = new List<DmUncertaintyInfo>();
            foreach (DmDecisionInfo item in decisions)
            {
                var random = new Random(42);
                var pofSamples = new double[1000];
                var rulSamples = new double[1000];
                AmsV31AssetConfig config = AmsV31Config.GetAsset(item.EquipmentKey);
                for (int i = 0; i < pofSamples.Length; i++)
                {
                    double shape = Uniform(random, config.WeibullShape * 0.85d, config.WeibullShape * 1.15d);
                    double scale = Uniform(random, config.WeibullScale * 0.80d, config.WeibullScale * 1.20d);
                    double logisticK = Uniform(random, AmsV31Config.LogisticK * 0.90d, AmsV31Config.LogisticK * 1.10d);
                    double logisticX0 = Uniform(random, AmsV31Config.LogisticX0 * 0.90d, AmsV31Config.LogisticX0 * 1.10d);
                    double expK = Uniform(random, config.ExponentialK * 0.90d, config.ExponentialK * 1.10d);
                    double expC = Uniform(random, config.ExponentialC * 0.95d, config.ExponentialC * 1.05d);
                    pofSamples[i] = CalculatePofWithParameters(item.HI, item.UsageYears,
                        shape, scale, logisticK, logisticX0, expK, expC,
                        config.ExponentialK > 0d && config.ExponentialC > 0d);
                    double targetAge = scale * Math.Pow(-Math.Log(1d - AmsV31Config.RulTargetPof), 1d / shape);
                    rulSamples[i] = Math.Max(targetAge - item.UsageYears, 0d);
                }

                Array.Sort(pofSamples);
                Array.Sort(rulSamples);
                result.Add(new DmUncertaintyInfo
                {
                    Code = item.Code,
                    ProductName = item.ProductName,
                    PofMeanPct = pofSamples.Average() * 100d,
                    PofCiLowerPct = Percentile(pofSamples, 5d) * 100d,
                    PofCiUpperPct = Percentile(pofSamples, 95d) * 100d,
                    RulMeanYears = rulSamples.Average(),
                    RulCiLowerYears = Percentile(rulSamples, 5d),
                    RulCiUpperYears = Percentile(rulSamples, 95d)
                });
            }

            return result;
        }

        private static List<DmSensitivityInfo> CalculateSensitivity(
            IList<DmDecisionInfo> decisions)
        {
            var definitions = new[]
            {
                new SensitivityDefinition("weibull_shape", "Weibull 형상", 0.15d),
                new SensitivityDefinition("weibull_scale", "Weibull 척도", 0.20d),
                new SensitivityDefinition("logistic_k", "Logistic 기울기", 0.10d),
                new SensitivityDefinition("logistic_x0", "Logistic 중심", 0.10d),
                new SensitivityDefinition("exponential_K", "지수 K", 0.10d),
                new SensitivityDefinition("exponential_C", "지수 C", 0.05d)
            };
            var result = new List<DmSensitivityInfo>();

            foreach (SensitivityDefinition definition in definitions)
            {
                var pofSwings = new List<double>();
                var rulSwings = new List<double>();
                foreach (DmDecisionInfo item in decisions)
                {
                    AmsV31AssetConfig config = AmsV31Config.GetAsset(item.EquipmentKey);
                    ParameterSet low = ParameterSet.From(config);
                    ParameterSet high = ParameterSet.From(config);
                    low.Set(definition.Code, 1d - definition.Change);
                    high.Set(definition.Code, 1d + definition.Change);
                    double lowPof = CalculatePofWithParameters(item.HI, item.UsageYears,
                        low.Shape, low.Scale, low.LogisticK, low.LogisticX0,
                        low.ExponentialK, low.ExponentialC, low.HasDiagnostic);
                    double highPof = CalculatePofWithParameters(item.HI, item.UsageYears,
                        high.Shape, high.Scale, high.LogisticK, high.LogisticX0,
                        high.ExponentialK, high.ExponentialC, high.HasDiagnostic);
                    double lowRul = Math.Max(low.Scale * Math.Pow(-Math.Log(0.5d), 1d / low.Shape) - item.UsageYears, 0d);
                    double highRul = Math.Max(high.Scale * Math.Pow(-Math.Log(0.5d), 1d / high.Shape) - item.UsageYears, 0d);
                    pofSwings.Add(Math.Abs(highPof - lowPof) * 100d);
                    rulSwings.Add(Math.Abs(highRul - lowRul));
                }

                result.Add(new DmSensitivityInfo
                {
                    Parameter = definition.Code,
                    ParameterName = definition.Name,
                    AveragePofSwingPct = pofSwings.Count > 0 ? pofSwings.Average() : 0d,
                    AverageRulSwingYears = rulSwings.Count > 0 ? rulSwings.Average() : 0d
                });
            }

            return result.OrderByDescending(x => x.AveragePofSwingPct).ToList();
        }

        private static List<DmInputReadinessInfo> BuildInputReadiness(IList<DmDecisionInfo> decisions)
        {
            return new List<DmInputReadinessInfo>
            {
                new DmInputReadinessInfo { Feature = "HI·PoF·Risk", Ready = decisions.Count > 0, Message = decisions.Count > 0 ? "계산 가능" : "기본정보·점검 데이터 필요" },
                new DmInputReadinessInfo { Feature = "예산 최적화·5개년 예측", Ready = decisions.Count > 0, Message = decisions.Count > 0 ? "계산 가능" : "DM 결과 필요" },
                new DmInputReadinessInfo { Feature = "DGA 진단", Ready = false, Message = "H2/CH4/C2H2/C2H4/C2H6/CO/CO2 DB 입력항목 필요" },
                new DmInputReadinessInfo { Feature = "공통원인고장(CCF)", Ready = decisions.GroupBy(x => x.EquipmentKey).Any(x => x.Count() >= 3), Message = "동일 유형 3대 이상 필요" },
                new DmInputReadinessInfo { Feature = "CIM·ISO 55001 내보내기", Ready = decisions.Count > 0, Message = decisions.Count > 0 ? "사용 가능" : "DM 결과 필요" }
            };
        }

        private static double CalculatePofWithParameters(
            double hi,
            double age,
            double shape,
            double scale,
            double logisticK,
            double logisticX0,
            double exponentialK,
            double exponentialC,
            bool hasDiagnostic)
        {
            double logistic = 1d / (1d + Math.Exp(-logisticK * (hi - logisticX0)));
            if (hasDiagnostic)
            {
                double x = exponentialC * hi;
                double aging = AmsV31Config.Clamp(exponentialK
                    * (1d + x + x * x / 2d + x * x * x / 6d), 0d, 1d);
                return AmsV31Config.Clamp(0.7d * logistic + 0.3d * aging, 0d, 1d);
            }

            double weibull = WeibullPof(age, scale, shape);
            return AmsV31Config.Clamp(0.5d * logistic + 0.5d * weibull, 0d, 1d);
        }

        private static double WeibullPof(double age, double scale, double shape)
        {
            return scale > 0d && shape > 0d
                ? AmsV31Config.Clamp(1d - Math.Exp(-Math.Pow(Math.Max(age, 0d) / scale, shape)), 0d, 1d)
                : 0d;
        }

        private static double Uniform(Random random, double minimum, double maximum)
        {
            return minimum + random.NextDouble() * (maximum - minimum);
        }

        private static double Percentile(double[] sorted, double percentile)
        {
            if (sorted == null || sorted.Length == 0) return 0d;
            double position = (sorted.Length - 1d) * percentile / 100d;
            int lower = (int)Math.Floor(position);
            int upper = (int)Math.Ceiling(position);
            if (lower == upper) return sorted[lower];
            double fraction = position - lower;
            return sorted[lower] + (sorted[upper] - sorted[lower]) * fraction;
        }

        private static string GetForcedRule(DmDecisionInfo item)
        {
            if (item.PoFRatio > 0.8d) return "PoF 80% 초과 강제규칙";
            if (item.HI >= 4.8d) return "HI 4.8 이상 강제규칙";
            if (item.TopsisScore > 0.75d) return "TOPSIS 0.75 초과 기준";
            if (item.PoFRatio > 0.6d && item.HI >= 4d) return "PoF 60% 초과 및 HI 4 이상 강제규칙";
            if (item.TopsisScore > 0.55d) return "TOPSIS 0.55 초과 기준";
            return string.Empty;
        }

        private static double Normalize(double value, double minimum, double maximum)
        {
            return maximum > minimum
                ? AmsV31Config.Clamp((value - minimum) / (maximum - minimum), 0d, 1d)
                : 0d;
        }

        private static double Min(IList<DmDecisionInfo> rows, Func<DmDecisionInfo, double> selector)
        {
            return rows.Count > 0 ? rows.Min(selector) : 0d;
        }

        private static double Max(IList<DmDecisionInfo> rows, Func<DmDecisionInfo, double> selector)
        {
            return rows.Count > 0 ? rows.Max(selector) : 0d;
        }

        private sealed class MultiYearAsset
        {
            public string Code { get; set; }
            public double BaseCost { get; set; }
            public double Cof { get; set; }
            public double Shape { get; set; }
            public double Scale { get; set; }
            public double EffectiveAge { get; set; }
            public double InitialEffectiveAge { get; set; }
            public bool Replaced { get; set; }
            public int MaintenanceCount { get; set; }
        }

        private sealed class MultiYearCandidate
        {
            public MultiYearCandidate(
                MultiYearAsset asset,
                string action,
                double cost,
                double benefit,
                double risk)
            {
                Asset = asset;
                Action = action;
                Cost = cost;
                Benefit = benefit;
                Risk = risk;
            }

            public MultiYearAsset Asset { get; private set; }
            public string Action { get; private set; }
            public double Cost { get; private set; }
            public double Benefit { get; private set; }
            public double Risk { get; private set; }
        }

        private sealed class SensitivityDefinition
        {
            public SensitivityDefinition(string code, string name, double change)
            {
                Code = code;
                Name = name;
                Change = change;
            }

            public string Code { get; private set; }
            public string Name { get; private set; }
            public double Change { get; private set; }
        }

        private sealed class ParameterSet
        {
            public double Shape { get; private set; }
            public double Scale { get; private set; }
            public double LogisticK { get; private set; }
            public double LogisticX0 { get; private set; }
            public double ExponentialK { get; private set; }
            public double ExponentialC { get; private set; }
            public bool HasDiagnostic { get; private set; }

            public static ParameterSet From(AmsV31AssetConfig config)
            {
                return new ParameterSet
                {
                    Shape = config.WeibullShape,
                    Scale = config.WeibullScale,
                    LogisticK = AmsV31Config.LogisticK,
                    LogisticX0 = AmsV31Config.LogisticX0,
                    ExponentialK = config.ExponentialK,
                    ExponentialC = config.ExponentialC,
                    HasDiagnostic = config.ExponentialK > 0d && config.ExponentialC > 0d
                };
            }

            public void Set(string parameter, double multiplier)
            {
                switch (parameter)
                {
                    case "weibull_shape": Shape *= multiplier; break;
                    case "weibull_scale": Scale *= multiplier; break;
                    case "logistic_k": LogisticK *= multiplier; break;
                    case "logistic_x0": LogisticX0 *= multiplier; break;
                    case "exponential_K": ExponentialK *= multiplier; break;
                    case "exponential_C": ExponentialC *= multiplier; break;
                }
            }
        }

        private sealed class BudgetState
        {
            public long Cost { get; set; }
            public double Benefit { get; set; }
            public List<DmBudgetActionInfo> Actions { get; set; }
        }

        private sealed class Scenario
        {
            public Scenario(string code, string name)
            {
                Code = code;
                Name = name;
            }

            public string Code { get; private set; }
            public string Name { get; private set; }
        }
    }
}
