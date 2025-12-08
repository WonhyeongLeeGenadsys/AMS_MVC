using System;

namespace Web.Common
{
    public class DCCABLEChk
    {
        public int Tbl_Idx { get; set; }
        public string DCCABLE_Code { get; set; }
        public string CHK_Gongsa_Name { get; set; }
        public string CHK_Weather { get; set; }
        public string CHK_Temp { get; set; }
        public string CHK_Hum { get; set; }
        public string CHK_Company { get; set; }
        public string CHK_Worker { get; set; }
        public string CHK_Manager { get; set; }
        public string CHK_Urgent_No { get; set; }
        public string CHK_Type { get; set; }
        public DateTime? CHK_Start_Date { get; set; }
        public DateTime? CHK_End_Date { get; set; }
        public string CHK_Writer { get; set; }

        public float CHK_Partial_Discharge { get; set; }   // 부분방전 (PD)
        public float CHK_Rated_Voltage { get; set; }   // 공칭전압 (PU)
        public float CHK_Tan_Delta { get; set; }   // Tan Delta
        public float CHK_Resistance { get; set; }   // 저항 측정 (Ω)
        public float CHK_TDR { get; set; }   // 시간영역 반사 (PU)

        public int FoldingFunction { get; set; }
        public DateTime? CHK_Update_Time { get; set; }

        public DateTime CHK_Tbl_GetDate { get; set; }

        /// <summary>부분방전</summary>
        public string PartialDischargeText
        {
            get
            {
                switch ((int)CHK_Partial_Discharge)
                {
                    case 1: return "10 pC 이하";
                    case 2: return "10 ~ 25 pC";
                    case 3: return "25 ~ 50 pC";
                    case 4: return "50 ~ 100 pC";
                    case 5: return "100 pC 초과";
                    default: return "-";
                }
            }
        }

        /// <summary>공칭전압</summary>
        public string RatedVoltageText
        {
            get
            {
                switch ((int)CHK_Rated_Voltage)
                {
                    case 1: return "PU ±2.5% 이내";
                    case 2: return "PU ±5% 이내";
                    case 3: return "PU ±7.5% 이내";
                    case 4: return "PU ±10% 이내";
                    case 5: return "PU ±10% 초과";
                    default: return "-";
                }
            }
        }

        /// <summary>Tan Delta</summary>
        public string TanDeltaText
        {
            get
            {
                switch ((int)CHK_Tan_Delta)
                {
                    case 1: return "0.001 이하";
                    case 2: return "0.001 ~ 0.004";
                    case 3: return "0.004 ~ 0.008";
                    case 4: return "0.008 ~ 0.01";
                    case 5: return "0.01 초과";
                    default: return "-";
                }
            }
        }

        /// <summary>저항 측정</summary>
        public string ResistanceText
        {
            get
            {
                switch ((int)CHK_Resistance)
                {
                    case 1: return "10 GΩ 이상";
                    case 2: return "5 ~ 10 GΩ";
                    case 3: return "1 ~ 5 GΩ";
                    case 4: return "500 MΩ ~ 1000 MΩ";
                    case 5: return "500 MΩ 이하";
                    default: return "-";
                }
            }
        }

        /// <summary>시간영역 반사 (TDR)</summary>
        public string TDRText
        {
            get
            {
                switch ((int)CHK_TDR)
                {
                    case 1: return "PU 5% 이내";
                    case 2: return "PU 5 ~ 7.5% 이내";
                    case 3: return "PU 7.5 ~ 10% 이내";
                    case 4: return "PU 10 ~ 15% 이내";
                    case 5: return "PU 15% 초과";
                    default: return "-";
                }
            }
        }
    }
}
