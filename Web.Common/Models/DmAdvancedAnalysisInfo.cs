using System.Collections.Generic;

namespace Web.Common
{
    public sealed class DmBudgetActionInfo
    {
        public string Code { get; set; }
        public string ProductName { get; set; }
        public string Action { get; set; }
        public double Cost { get; set; }
        public double Benefit { get; set; }
        public double BenefitCostRatio { get; set; }
    }

    public sealed class DmBudgetOptimizationInfo
    {
        public double TotalBudget { get; set; }
        public double TotalCost { get; set; }
        public double TotalBenefit { get; set; }
        public double RemainingBudget { get; set; }
        public double UtilizationPct { get; set; }
        public int ReplaceCount { get; set; }
        public int MaintainCount { get; set; }
        public int DoNothingCount { get; set; }
        public List<DmBudgetActionInfo> Actions { get; set; }
    }

    public sealed class DmPredictionInfo
    {
        public string Scenario { get; set; }
        public string ScenarioName { get; set; }
        public int YearNo { get; set; }
        public int CalendarYear { get; set; }
        public double FleetRisk { get; set; }
        public double AveragePofPct { get; set; }
        public double AverageHi { get; set; }
    }

    public sealed class DmExplanationInfo
    {
        public string Code { get; set; }
        public string ProductName { get; set; }
        public string Decision { get; set; }
        public string TopFactor { get; set; }
        public string Explanation { get; set; }
        public double RiskContribution { get; set; }
        public double BcrContribution { get; set; }
        public double HiContribution { get; set; }
        public double RulContribution { get; set; }
        public double CriticalityContribution { get; set; }
        public double PofContribution { get; set; }
    }

    public sealed class DmMultiYearPlanInfo
    {
        public int YearNo { get; set; }
        public int CalendarYear { get; set; }
        public double Budget { get; set; }
        public double Spent { get; set; }
        public double UtilizationPct { get; set; }
        public int ReplaceCount { get; set; }
        public int MaintainCount { get; set; }
        public int UrgentBacklog { get; set; }
        public double RiskReduction { get; set; }
        public double CumulativeCost { get; set; }
        public double CumulativeBenefit { get; set; }
    }

    public sealed class DmUncertaintyInfo
    {
        public string Code { get; set; }
        public string ProductName { get; set; }
        public double PofMeanPct { get; set; }
        public double PofCiLowerPct { get; set; }
        public double PofCiUpperPct { get; set; }
        public double RulMeanYears { get; set; }
        public double RulCiLowerYears { get; set; }
        public double RulCiUpperYears { get; set; }
    }

    public sealed class DmSensitivityInfo
    {
        public string Parameter { get; set; }
        public string ParameterName { get; set; }
        public double AveragePofSwingPct { get; set; }
        public double AverageRulSwingYears { get; set; }
    }

    public sealed class DmInputReadinessInfo
    {
        public string Feature { get; set; }
        public bool Ready { get; set; }
        public string Message { get; set; }
    }

    public sealed class DmAdvancedAnalysisInfo
    {
        public DmBudgetOptimizationInfo Budget { get; set; }
        public List<DmPredictionInfo> Predictions { get; set; }
        public List<DmExplanationInfo> Explanations { get; set; }
        public List<DmMultiYearPlanInfo> MultiYearPlan { get; set; }
        public List<DmUncertaintyInfo> Uncertainty { get; set; }
        public List<DmSensitivityInfo> Sensitivity { get; set; }
        public List<DmInputReadinessInfo> InputReadiness { get; set; }
        public int PredictionHorizonYears { get; set; }
        public string CalculationVersion { get; set; }
    }
}
