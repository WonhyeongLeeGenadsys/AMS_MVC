using System;
using System.Linq;
using AMS_MVC.Models;

namespace AMS_MVC.Services
{
    public class ITRChkScoreCalculator
    {
        /// <summary>
        /// 보통점검(ITRChk1)에 포함된 모든 수치형 변수 중 최대값을 Folding Function으로 계산
        /// </summary>
        public int CalculateFoldingFunction(ITRChk1 chk)
        {
            var scores = new float[]
            {
                // DGA
                float.Parse(chk.CHK1_H2),
                float.Parse(chk.CHK1_C2H2),
                float.Parse(chk.CHK1_C2H4),
                float.Parse(chk.CHK1_CH4),
                float.Parse(chk.CHK1_C2H6),
                float.Parse(chk.CHK1_CO),
                float.Parse(chk.CHK1_CO2),

                // 유전체강도
                float.Parse(chk.CHK1_Dielectric_Strength),

                // 절연 손상／노후도
                float.Parse(chk.CHK1_Remain_Life),
                float.Parse(chk.CHK1_Age),
                float.Parse(chk.CHK1_Gojang_History),

                // 절연진동／기계적 시험
                float.Parse(chk.CHK1_Doble),
                float.Parse(chk.CHK1_SFRA),

                // 절연저항
                float.Parse(chk.CHK1_HV_E),
                float.Parse(chk.CHK1_LV_E),
                float.Parse(chk.CHK1_TV_E),
                float.Parse(chk.CHK1_HV_LV),
                float.Parse(chk.CHK1_HV_TV),
                float.Parse(chk.CHK1_LV_TV)
            };

            // 최대값을 int로 리턴
            return (int)scores.Max();
        }

        /// <summary>
        /// 정밀점검(ITRChk2)에 포함된 모든 수치형 변수 중 최대값을 Folding Function으로 계산
        /// </summary>
        public int CalculateFoldingFunction(ITRChk2 chk)
        {
            var scores = new float[]
            {
                float.Parse(chk.CHK2_Computerized_Price),
                float.Parse(chk.CHK2_Water_Content),
                float.Parse(chk.CHK2_Furfural),
                float.Parse(chk.CHK2_Excitation_Current),
                float.Parse(chk.CHK2_Short_Current),
                float.Parse(chk.CHK2_Voltage_Ratio),
                float.Parse(chk.CHK2_PD)
            };

            return (int)scores.Max();
        }
    }
}
