using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Common
{
    public class VCBChk
    {
        public int Tbl_Idx { get; set; }
        public string VCB_Code { get; set; }
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
        public string CHK_Chuk_Loc { get; set; } //개폐표시기 정위치
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
        public string CHK_VCB_Count { get; set; }
        public string CHK_Cutoff_Count { get; set; }
        public string CHK_A_Rate { get; set; }
        public string CHK_Short_A_Rate { get; set; }
        public string CHK_Writer { get; set; } // 작성자 

        // 실제 알고리즘 사용 변수
        public float CHK_ContactWearPercent { get; set; }
        public float CHK_VacuumLeakCurrent { get; set; }
        public float CHK_ContactResistance { get; set; }
        public float CHK_InsulationResistance { get; set; }
        public float CHK_HotSpot { get; set; }
        public float CHK_PdPatternValue { get; set; }
        public float CHK_MotorCurrent { get; set; }
        public float CHK_AccumShortCircuitCurrent { get; set; }
        public float CHK_ShortCircuitCount { get; set; }
        public float CHK_OperationCount { get; set; }
        public float CHK_OpenCloseTime { get; set; }
        public float CHK_VisualCheck { get; set; }
        public int FoldingFunction { get; set; }
        //
        public DateTime CHK_Tbl_GetDate { get; set; }


        public string ContactWearPercentText
        {
            get
            {
                switch ((int)CHK_ContactWearPercent)
                {
                    case 1: return "2 미만 mm";
                    case 3: return "2 ~ 3 mm";  
                    case 5: return "3 초과 mm";
                    default: return "-";
                }
            }
        }

        public string VacuumLeakCurrentText
        {
            get
            {
                switch ((int)CHK_VacuumLeakCurrent)
                {
                    case 1: return "0.1 미만 mA";
                    case 3: return "0.1 ~ 0.2 mA";
                    case 5: return "0.3 초과 mA";
                    default: return "-";
                }
            }
        }

        public string ContactResistanceText
        {
            get
            {
                switch ((int)CHK_ContactResistance)
                {
                    case 1: return "≤ 5%";
                    case 3: return "5% ~ 10%";
                    case 4: return "10% ~ 20%";
                    case 5: return "> 20%";
                    default: return "-";
                }
            }
        }

        public string InsulationResistanceText
        {
            get
            {
                switch ((int)CHK_InsulationResistance)
                {
                    case 1: return "500 이상 MΩ";
                    case 2: return "400 ~ 500 MΩ";
                    case 3: return "300 ~ 400 MΩ";
                    case 4: return "200 ~ 300 MΩ";
                    case 5: return "200 미만 MΩ";
                    default: return "-";
                }
            }
        }

        public string HotSpotText
        {
            get
            {
                switch ((int)CHK_HotSpot)
                {
                    case 1: return "없음";
                    case 5: return "발견";
                    default: return "-";
                }
            }
        }

        public string PdPatternValueText
        {
            get
            {
                switch ((int)CHK_PdPatternValue)
                {
                    case 1: return "없음";
                    case 4: return "있음(성장X)";
                    case 5: return "있음";
                    default: return "-";
                }
            }
        }

        public string MotorCurrentText
        {
            get
            {
                switch ((int)CHK_MotorCurrent)
                {
                    case 1: return "≤ 2%";
                    case 3: return "2% ~ 5%";
                    case 4: return "5% ~ 10%";
                    case 5: return "> 10%";
                    default: return "-";
                }
            }
        }

        public string AccumShortCircuitCurrentText
        {
            get
            {
                switch ((int)CHK_AccumShortCircuitCurrent)
                {
                    case 1: return "≤ 20%";
                    case 2: return "20% ~ 40%";
                    case 3: return "40% ~ 70%";
                    case 4: return "70% ~ 100%";
                    case 5: return "> Limit";
                    default: return "-";
                }
            }
        }

        public string ShortCircuitCountText
        {
            get
            {
                switch ((int)CHK_ShortCircuitCount)
                {
                    case 1: return "≤ 20%";
                    case 2: return "20% ~ 40%";
                    case 3: return "40% ~ 70%";
                    case 4: return "70% ~ 100%";
                    case 5: return "> Limit";
                    default: return "-";
                }
            }
        }

        public string OperationCountText
        {
            get
            {
                switch ((int)CHK_OperationCount)
                {
                    case 1: return "≤ 20%";
                    case 2: return "20% ~ 40%";
                    case 3: return "40% ~ 70%";
                    case 4: return "70% ~ 100%";
                    case 5: return "> Limit";
                    default: return "-";
                }
            }
        }

        public string OpenCloseTimeText
        {
            get
            {
                switch ((int)CHK_OpenCloseTime)
                {
                    case 1: return "≤ 2%";
                    case 3: return "2% ~ 5%";
                    case 4: return "5% ~ 10%";
                    case 5: return "> 10%";
                    default: return "-";
                }
            }
        }

        public string VisualCheckText
        {
            get
            {
                switch ((int)CHK_VisualCheck)
                {
                    case 1: return "오염, 부식, 균열 없음";
                    case 3: return "오염, 부식, 미약한 균열 발생";
                    case 4: return "오염, 부식이 축적";
                    case 5: return "오염, 부식, 균열이 성능에 영향";
                    default: return "-";
                }
            }
        }

    }
}