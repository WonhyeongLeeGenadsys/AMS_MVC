using System;

namespace Web.Common
{
    public class SUBMODULEChk
    {
        public int Tbl_Idx { get; set; }
        public string SUBMODULE_Code { get; set; }
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

        // 실제 알고리즘 사용 변수 (1, 3, 10, 30, 100 중 하나)
        public float CHK_CE_Voltage { get; set; }        // V_ce
        public float CHK_G_Voltage { get; set; }         // V_g
        public float CHK_On_Resistance { get; set; }     // R_on
        public float CHK_Thermal_Resistance { get; set; }// R_th
        public float CHK_C_Current { get; set; }         // I_c
        public float CHK_OnOff_Time { get; set; }        // T_on,off

        public float CHK_Insulation_Resistance { get; set; } // 절연저항
        public float CHK_ESR { get; set; }                    // ESR
        public float CHK_Capacitance { get; set; }            // 커패시턴스
        public float CHK_Temperature { get; set; }            // 온도

        public int FoldingFunction { get; set; }
        public DateTime? CHK_Update_Time { get; set; }

        public DateTime CHK_Tbl_GetDate { get; set; }

        public string CE_VoltageText
        {
            get
            {
                switch ((int)CHK_CE_Voltage)
                {
                    case 1: return "0.9 ~ 1.0 PU";
                    case 2: return "1.0 ~ 1.1 PU";
                    case 3: return "1.1 ~ 1.2 PU";
                    case 4: return "1.2 ~ 1.25 PU";
                    case 5: return "> 1.25 PU";
                    default: return "-";
                }
            }
        }

        public string G_VoltageText
        {
            get
            {
                switch ((int)CHK_G_Voltage)
                {
                    case 1: return "0 ~ 0.5 V";
                    case 2: return "0.5 ~ 1.0 V";
                    case 3: return "1 ~ 3 V";
                    case 4: return "3 ~ 5 V";
                    case 5: return "> 5 V";
                    default: return "-";
                }
            }
        }

        public string OnResistanceText
        {
            get
            {
                switch ((int)CHK_On_Resistance)
                {
                    case 1: return "0.9 ~ 1.0 PU";
                    case 2: return "1.0 ~ 1.1 PU";
                    case 3: return "1.1 ~ 1.2 PU";
                    case 4: return "1.2 ~ 1.25 PU";
                    case 5: return "> 1.25 PU";
                    default: return "-";
                }
            }
        }

        public string ThermalResistanceText
        {
            get
            {
                switch ((int)CHK_Thermal_Resistance)
                {
                    case 1: return "0.9 ~ 1.0 PU";
                    case 2: return "1.0 ~ 1.1 PU";
                    case 3: return "1.1 ~ 1.2 PU";
                    case 4: return "1.2 ~ 1.25 PU";
                    case 5: return "> 1.25 PU";
                    default: return "-";
                }
            }
        }

        public string C_CurrentText
        {
            get
            {
                switch ((int)CHK_C_Current)
                {
                    case 1: return "0.9 ~ 1.0 PU";
                    case 2: return "1.0 ~ 1.1 PU";
                    case 3: return "1.1 ~ 1.2 PU";
                    case 4: return "1.2 ~ 1.25 PU";
                    case 5: return "> 1.25 PU";
                    default: return "-";
                }
            }
        }

        public string OnOffTimeText
        {
            get
            {
                switch ((int)CHK_OnOff_Time)
                {
                    case 1: return "0.9 ~ 1.0 PU";
                    case 2: return "1.0 ~ 1.1 PU";
                    case 3: return "1.1 ~ 1.2 PU";
                    case 4: return "1.2 ~ 1.25 PU";
                    case 5: return "> 1.25 PU";
                    default: return "-";
                }
            }
        }

        public string InsulationResistanceText
        {
            get
            {
                switch ((int)CHK_Insulation_Resistance)
                {
                    case 1: return "500 MΩ 이상";
                    case 2: return "400 ~ 500 MΩ";
                    case 3: return "200 ~ 300 MΩ";
                    case 4: return "100 ~ 200 MΩ";
                    case 5: return "100 MΩ 미만";
                    default: return "-";
                }
            }
        }

        public string ESRText
        {
            get
            {
                switch ((int)CHK_ESR)
                {
                    case 1: return "초기값 대비 1.0×";
                    case 2: return "초기값 대비 1.25×";
                    case 3: return "초기값 대비 1.5×";
                    case 4: return "초기값 대비 1.75×";
                    case 5: return "초기값 대비 2.0× 초과";
                    default: return "-";
                }
            }
        }

        public string CapacitanceText
        {
            get
            {
                switch ((int)CHK_Capacitance)
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

        public string TemperatureText
        {
            get
            {
                switch ((int)CHK_Temperature)
                {
                    case 1: return "권장온도 80% 이내";
                    case 2: return "권장온도 85% 이내";
                    case 3: return "권장온도 95% 이내";
                    case 4: return "권장온도 100% 이내";
                    case 5: return "권장온도 초과";
                    default: return "-";
                }
            }
        }
    }
}
