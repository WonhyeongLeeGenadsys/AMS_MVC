namespace Web.Common
{
    public sealed class DmDecisionInfo
    {
        public int? Priority { get; set; }
        public string Sort { get; set; }
        public string Code { get; set; }
        public string SerialNo { get; set; }
        public string Name { get; set; }
        public string ProductName { get; set; }
        public string AssetType { get; set; }
        public string EquipmentKey { get; set; }
        public int UsageYears { get; set; }
        public double HI { get; set; }
        public double PoFRatio { get; set; }
        public double ReplacementCost { get; set; }
        /// <summary>목포대 RISKMATRIX에 저장된 운영 CoF(USD).</summary>
        public double CoF { get; set; }
        /// <summary>숭실대 DM 4개 영향 항목으로 산정한 참고 CoF(KRW).</summary>
        public double DmCofKrw { get; set; }
        public double CofTotalKrw { get; set; }
        public double RawCofFinancial { get; set; }
        public double RawCofReliability { get; set; }
        public double RawCofSafety { get; set; }
        public double RawCofEnvironmental { get; set; }
        public double CofFinancial { get; set; }
        public double CofReliability { get; set; }
        public double CofSafety { get; set; }
        public double CofEnvironmental { get; set; }
        public double CofCens { get; set; }
        public double CofSaidiPenalty { get; set; }
        public double CofSaifiPenalty { get; set; }
        public double SaidiContribution { get; set; }
        public double SaifiContribution { get; set; }
        public double CustomersAffected { get; set; }
        public double Risk { get; set; }
        public double NpvValue { get; set; }
        public double NpvBenefits { get; set; }
        public double NpvCosts { get; set; }
        public double Bcr { get; set; }
        public double RoiPct { get; set; }
        public double RiskMitigation { get; set; }
        public double AnnualMaintenanceSaving { get; set; }
        public double AnnualEfficiencyBenefit { get; set; }
        public double AnnualBenefits { get; set; }
        public double TotalBenefits { get; set; }
        public double DiscountedAnnualBenefits { get; set; }
        public double ExtendedLifetimeValue { get; set; }
        public double InstallationCost { get; set; }
        public double DisposalCost { get; set; }
        public double TotalCosts { get; set; }
        public double DiscountRatePct { get; set; }
        public double InflationRatePct { get; set; }
        public int EvaluationPeriodYears { get; set; }
        public double? RULYears { get; set; }
        public double Criticality { get; set; }
        public double TopsisScore { get; set; }
        public int TopsisRank { get; set; }
        public double AhpConsistencyRatio { get; set; }
        public int Severity { get; set; }
        public string Decision { get; set; }
        public string Urgency { get; set; }
        public string RecommendedAction { get; set; }
        public double DMScore { get; set; }
    }
}
