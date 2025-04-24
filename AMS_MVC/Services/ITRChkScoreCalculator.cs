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
            int[] scores = new int[]
            {
                // DGA
                chk.CHK1_H2,
                chk.CHK1_C2H2,
                chk.CHK1_C2H4,
                chk.CHK1_CH4,
                chk.CHK1_C2H6,
                chk.CHK1_CO,
                chk.CHK1_CO2,

                // 절연 강도/노후도 등
                chk.CHK1_Dielectric_Strength,
                chk.CHK1_Remain_Life,
                chk.CHK1_Age,
                chk.CHK1_Gojang_History,

                // 절연진동/기계적 시험
                chk.CHK1_Doble,
                chk.CHK1_SFRA,

                // 절연저항
                chk.CHK1_HV_E,
                chk.CHK1_LV_E,
                chk.CHK1_TV_E,
                chk.CHK1_HV_LV,
                chk.CHK1_HV_TV,
                chk.CHK1_LV_TV
            };

            return scores.Max();
        }

        /// <summary>
        /// 정밀점검(ITRChk2)에 포함된 모든 수치형 변수 중 최대값을 Folding Function으로 계산
        /// </summary>
        public int CalculateFoldingFunction(ITRChk2 chk)
        {
            int[] scores = new int[]
            {
                chk.CHK2_Acid_Value,
                chk.CHK2_Computerized_Price,
                chk.CHK2_Water_Content,
                chk.CHK2_Furfural,
                chk.CHK2_Excitation_Current,
                chk.CHK2_Short_Current,
                chk.CHK2_Voltage_Ratio,
                chk.CHK2_PD
            };

            return scores.Max();
        }
    }
}
