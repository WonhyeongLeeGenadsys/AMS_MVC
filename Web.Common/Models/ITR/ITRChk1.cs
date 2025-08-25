using System;

namespace Web.Common
{
    public class ITRChk1
    {
        public int Tbl_Idx { get; set; }
        public string ITR_Code { get; set; }
        public string CHK1_Gongsa_Name { get; set; }
        public string CHK1_Weather { get; set; }
        public string CHK1_Temp { get; set; }
        public string CHK1_Hum { get; set; }
        public string CHK1_Company { get; set; }
        public string CHK1_Worker { get; set; }
        public string CHK1_Manager { get; set; }
        public string CHK1_Urgent_No { get; set; }
        public string CHK1_Type { get; set; }
        public DateTime? CHK1_Start_Date { get; set; }
        public DateTime? CHK1_End_Date { get; set; }

        // DGA
        public int CHK1_H2 { get; set; }
        public int CHK1_C2H2 { get; set; }
        public int CHK1_C2H4 { get; set; }
        public int CHK1_CH4 { get; set; }
        public int CHK1_C2H6 { get; set; }
        public int CHK1_CO { get; set; }
        public int CHK1_CO2 { get; set; }

        // 절연 손상/노후도
        public int CHK1_Dielectric_Strength { get; set; }
        public int CHK1_Remain_Life { get; set; }
        public int CHK1_Age { get; set; }
        public int CHK1_Gojang_History { get; set; }

        // 절연진동/기계적 시험
        public int CHK1_Doble { get; set; }
        public int CHK1_SFRA { get; set; }

        // 절연저항
        public int CHK1_HV_E { get; set; }
        public int CHK1_LV_E { get; set; }
        public int CHK1_TV_E { get; set; }
        public int CHK1_HV_LV { get; set; }
        public int CHK1_HV_TV { get; set; }
        public int CHK1_LV_TV { get; set; }
        public int FoldingFunction { get; set; }

        public string CHK1_Writer { get; set; }
        public DateTime CHK1_Tbl_GetDate { get; set; }

        public string H2Text
        {
            get
            {
                switch (CHK1_H2)
                {
                    case 1: return "≤200";
                    case 2: return "≤400";
                    case 3: return "≤800";
                    case 4: return ">800";
                    default: return "-";
                }
            }
        }
        public string C2H2Text
        {
            get
            {
                switch (CHK1_C2H2)
                {
                    case 1: return "≤10";
                    case 2: return "≤20";
                    case 3: return "≤60";
                    case 4: return "≤120";
                    case 5: return ">120";
                    default: return "-";
                }
            }
        }
        public string C2H4Text
        {
            get
            {
                switch (CHK1_C2H4)
                {
                    case 1: return "≤100";
                    case 2: return "≤200";
                    case 3: return "≤500";
                    case 4: return ">500";
                    default: return "-";
                }
            }
        }
        public string CH4Text
        {
            get
            {
                switch (CHK1_CH4)
                {
                    case 1: return "≤150";
                    case 2: return "≤250";
                    case 3: return "≤750";
                    case 4: return ">750";
                    default: return "-";
                }
            }
        }
        public string C2H6Text
        {
            get
            {
                switch (CHK1_C2H6)
                {
                    case 1: return "≤200";
                    case 2: return "≤350";
                    case 3: return "≤750";
                    case 4: return ">750";
                    default: return "-";
                }
            }
        }
        public string COText
        {
            get
            {
                switch (CHK1_CO)
                {
                    case 1: return "≤800";
                    case 2: return "≤1200";
                    case 3: return ">1200";
                    default: return "-";
                }
            }
        }
        public string CO2Text
        {
            get
            {
                switch (CHK1_CO2)
                {
                    case 1: return "≤5000";
                    case 2: return "≤7000";
                    case 3: return ">7000";
                    default: return "-";
                }
            }
        }

        public string DielectricStrengthText
        {
            get
            {
                switch (CHK1_Dielectric_Strength)
                {
                    case 1: return ">40";
                    case 3: return "≥30";
                    case 5: return "<30";
                    default: return "-";
                }
            }
        }
        public string RemainLifeText
        {
            get
            {
                switch (CHK1_Remain_Life)
                {
                    case 1: return "≥20";
                    case 2: return "≥10";
                    case 3: return "≥5";
                    case 4: return "≥3";
                    case 5: return "≥1";
                    default: return "-";
                }
            }
        }
        public string AgeText
        {
            get
            {
                switch (CHK1_Age)
                {
                    case 1: return "≤20%";
                    case 2: return "≤40%";
                    case 3: return "≤60%";
                    case 4: return "≤80%";
                    case 5: return ">80%";
                    default: return "-";
                }
            }
        }
        public string GojangHistoryText
        {
            get
            {
                switch (CHK1_Gojang_History)
                {
                    case 1: return "NOT";
                    case 5: return "OCCUR";
                    default: return "-";
                }
            }
        }

        public string DobleText
        {
            get
            {
                switch (CHK1_Doble)
                {
                    case 1: return "GOOD";
                    case 2: return "OBSERVE";
                    case 3: return "CAREFUL";
                    case 4: return "POOR";
                    default: return "-";
                }
            }
        }
        public string SFAText
        {
            get
            {
                switch (CHK1_SFRA)
                {
                    case 1: return "≤0.1";
                    case 3: return "≤0.2";
                    default: return "-";
                }
            }
        }

        // 절연저항
        private string ResolveInsulation(int grade)
        {
            switch (grade)
            {
                case 1: return "≥1000MΩ";
                case 5: return "<1000MΩ";
                default: return "-";
            }
        }
        public string HVEText => ResolveInsulation(CHK1_HV_E);
        public string LVEText => ResolveInsulation(CHK1_LV_E);
        public string TVEText => ResolveInsulation(CHK1_TV_E);
        public string HVLVText => ResolveInsulation(CHK1_HV_LV);
        public string HVTVText => ResolveInsulation(CHK1_HV_TV);
        public string LTVText => ResolveInsulation(CHK1_LV_TV);
    }
}
