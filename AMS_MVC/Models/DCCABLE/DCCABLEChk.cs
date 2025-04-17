using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS_MVC.Models
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
        public string CHK_DCCABLE_Count { get; set; }
        public string CHK_Cutoff_Count { get; set; }
        public string CHK_A_Rate { get; set; }
        public string CHK_Short_A_Rate { get; set; }
        public string CHK_Writer { get; set; }
        public float CHK_Partial_Discharge { get; set; }     // 부분방전
        public float CHK_Rated_Voltage { get; set; }         // 공칭전압
        public float CHK_Tan_Delta { get; set; }             // Tan Delta
        public float CHK_Resistance { get; set; }            // 저항
        public float CHK_TDR { get; set; }                   // TDR
        public int FoldingFunction { get; set; }

        public DateTime CHK_Tbl_GetDate { get; set; }


    }
}
