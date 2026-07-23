using System;

namespace Web.Common
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
        public string CHK_Writer { get; set; }

        public float CHK_MainCircuit_InsulationStrength { get; set; }    // 주회로 절연내력
        public float CHK_MainCircuit_PD { get; set; }                    // 주회로 부분방전
        public float CHK_Machine_Part_Operation_Time { get; set; }       // 기계부 동작시간
        public float CHK_Mechanical_Vibration_acceleration { get; set; } // 기계부 진동/가속도
        public float CHK_Relay_Auxiliary_Contact_Resistance { get; set; }// 릴레이 보조접점 저항

        public float CHK_CE_Voltage { get; set; }      // V_ce
        public float CHK_G_Voltage { get; set; }       // V_g
        public float CHK_C_Current { get; set; }       // I_c
        public float CHK_OnOff_Time { get; set; }      // T_on,off

        public int FoldingFunction { get; set; }
        public DateTime? CHK_Update_Time { get; set; }

        public DateTime CHK_Tbl_GetDate { get; set; }

        public string MainCircuitInsulationStrengthText
        {
            get
            {
                switch ((int)CHK_MainCircuit_InsulationStrength)
                {
                    case 1: return "200 이상";
                    case 5: return "200 미만";
                    default: return "-";
                }
            }
        }

        public string MainCircuitPDText
        {
            get
            {
                switch ((int)CHK_MainCircuit_PD)
                {
                    case 1: return "미검출";
                    case 3: return "0 ~ 5 pC";
                    case 4: return "5 ~ 10 pC";
                    case 5: return "10 pC 초과";
                    default: return "-";
                }
            }
        }

        public string MachinePartOperationTimeText
        {
            get
            {
                switch ((int)CHK_Machine_Part_Operation_Time)
                {
                    case 1: return "1.0 P.U 이내";
                    case 3: return "1.0 ~ 1.25 P.U";
                    case 4: return "1.25 ~ 1.5 P.U";
                    case 5: return "1.5 P.U 초과";
                    default: return "-";
                }
            }
        }

        public string MechanicalVibrationAccelerationText
        {
            get
            {
                switch ((int)CHK_Mechanical_Vibration_acceleration)
                {
                    case 1: return "1.1 P.U 이내";
                    case 3: return "1.1 ~ 1.5 P.U";
                    case 4: return "1.5 ~ 2.0 P.U";
                    case 5: return "2.0 P.U 초과";
                    default: return "-";
                }
            }
        }

        public string RelayAuxiliaryContactResistanceText
        {
            get
            {
                switch ((int)CHK_Relay_Auxiliary_Contact_Resistance)
                {
                    case 1: return "1.0 P.U 이내";
                    case 3: return "1.0 ~ 1.25 P.U";
                    case 4: return "1.25 ~ 1.5 P.U";
                    case 5: return "1.5 P.U 초과";
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
                    case 1: return "1.0 P.U 이하";
                    case 3: return "1.0 ~ 1.2 P.U";
                    case 4: return "1.2 ~ 1.25 PU";
                    case 5: return "1.25 P.U 초과";
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
                    case 1: return "1.0 P.U 이하";
                    case 3: return "1.0 ~ 1.5 P.U";
                    case 5: return "1.5 P.U 초과";
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
                    case 1: return "1.0 P.U 이하";
                    case 3: return "1.0 ~ 1.2 P.U";
                    case 4: return "1.2 ~ 1.25 PU";
                    case 5: return "1.25 P.U 초과";
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
                    case 1: return "1.0 P.U 이하";
                    case 3: return "1.0 ~ 1.2 P.U";
                    case 4: return "1.2 ~ 1.25 PU";
                    case 5: return "1.25 P.U 초과";
                    default: return "-";
                }
            }
        }
    }
}
