using System;

namespace AMS_MVC.Models
{
    public class DCCBChk
    {
        public int Tbl_Idx { get; set; }
        public string DCCB_Code { get; set; }
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
        public string CHK_Loc { get; set; }
        public string CHK_Chuk_Loc { get; set; }
        public string CHK_Con_Status { get; set; }
        public string CHK_Bolt_Nut_Status { get; set; }
        public string CHK_Contact_Volume { get; set; }
        public string CHK_Vacuum_Degree { get; set; }
        public string CHK_Coil_A { get; set; }
        public string CHK_Contact_R { get; set; }
        public string CHK_Main_Circuit { get; set; }
        public string CHK_Control_Circuit { get; set; }
        public string CHK_Input_Time { get; set; }
        public string CHK_Open_Time { get; set; }
        public string CHK_3_Phase_Open_Gap { get; set; }
        public string CHK_Chattering_Time { get; set; }
        public string CHK_O_C_O { get; set; }
        public string CHK_Operate_Time { get; set; }
        public string CHK_OC_Test { get; set; }
        public string CHK_Indicator { get; set; }
        public string CHK_DCCB_Count { get; set; }
        public string CHK_Cutoff_Count { get; set; }
        public string CHK_A_Rate { get; set; }
        public string CHK_Short_A_Rate { get; set; }
        public string CHK_Writer { get; set; }

        public float CHK_MainCircuit_InsulationStrength { get; set; }  // 주 회로 절연내력 시험
        public float CHK_LeakTest { get; set; }                        // 기밀 시험
        public float CHK_MechanicalOperation { get; set; }             // 기계적 동작 시험 (속도)
        public float CHK_AuxControlCircuit { get; set; }               // 보조/제어회로 시험 (응답시간)

        public float CHK_CE_Voltage { get; set; }      // V_ce
        public float CHK_G_Voltage { get; set; }       // V_g
        public float CHK_On_Resistance { get; set; }   // R_on
        public float CHK_Thermal_Resistance { get; set; } // R_th
        public float CHK_C_Current { get; set; }       // I_c
        public float CHK_OnOff_Time { get; set; }      // T_on,off

        public int FoldingFunction { get; set; }
        public DateTime CHK_Tbl_GetDate { get; set; }

        public string MainCircuitInsulationStrengthText
        {
            get
            {
                switch ((int)CHK_MainCircuit_InsulationStrength)
                {
                    case 1: return "≥ 1.0 PU";
                    case 2: return "0.9 ~ 1.0 PU";
                    case 3: return "0.8 ~ 0.9 PU";
                    case 4: return "0.7 ~ 0.8 PU";
                    case 5: return "< 0.7 PU";
                    default: return "-";
                }
            }
        }

        public string LeakTestText
        {
            get
            {
                switch ((int)CHK_LeakTest)
                {
                    case 1: return "초기값 0.5% 이하";
                    case 2: return "초기값 0.5 ~ 0.7%";
                    case 3: return "초기값 0.7 ~ 0.85%";
                    case 4: return "초기값 0.85 ~ 1.0%";
                    case 5: return "초기값 1.0% 초과";
                    default: return "-";
                }
            }
        }

        public string MechanicalOperationText
        {
            get
            {
                switch ((int)CHK_MechanicalOperation)
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

        public string AuxControlCircuitText
        {
            get
            {
                switch ((int)CHK_AuxControlCircuit)
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
    }
}
