using System.Linq;

namespace Web.Common
{
    public class ITRChkScoreCalculator
    {
        // ITRChk1 전용
        public (decimal HI, decimal PoF) CalculateHiPof(ITRChk1 chk, decimal alpha = 1.00m)
        {
            int[] s1 = GetScores(chk);

            LogHelper.WriteLog("ITRChkScore",
                "[ITRChk1 Input] " +
                $"H2={chk.CHK1_H2}, C2H2={chk.CHK1_C2H2}, C2H4={chk.CHK1_C2H4}, CH4={chk.CHK1_CH4}, C2H6={chk.CHK1_C2H6}, " +
                $"CO={chk.CHK1_CO}, CO2={chk.CHK1_CO2}, 절연내력={chk.CHK1_Dielectric_Strength}, 경년={chk.CHK1_Age}, 고장이력={chk.CHK1_Gojang_History}, " +
                $"Doble={chk.CHK1_Doble}, SFRA={chk.CHK1_SFRA}, HV_E={chk.CHK1_HV_E}, LV_E={chk.CHK1_LV_E}, TV_E={chk.CHK1_TV_E}, " +
                $"HV_LV={chk.CHK1_HV_LV}, HV_TV={chk.CHK1_HV_TV}, LV_TV={chk.CHK1_LV_TV}");

            int max = s1.Max();
            int freq = s1.Count(v => v == max);

            return HiPofTable.GetHiPof(max, freq, alpha);
        }

        // ITRChk2 전용
        public (decimal HI, decimal PoF) CalculateHiPof(ITRChk2 chk, decimal alpha = 1.00m)
        {
            int[] s2 = GetScores(chk);

            LogHelper.WriteLog("ITRChkScore",
                "[ITRChk2 Input] " +
                $"가격={chk.CHK2_Computerized_Price}, 수분={chk.CHK2_Water_Content}, Furfural={chk.CHK2_Furfural}, " +
                $"여자전류={chk.CHK2_Excitation_Current}, 단락전류={chk.CHK2_Short_Current}, 권수전압비={chk.CHK2_Voltage_Ratio}, PD={chk.CHK2_PD}");

            int max = s2.Max();
            int freq = s2.Count(v => v == max);

            return HiPofTable.GetHiPof(max, freq, alpha);
        }


        // ITRChk1 + ITRChk2 통합
        public (decimal HI, decimal PoF) CalculateHiPofCombined(ITRChk1 c1, ITRChk2 c2, decimal alpha = 1.00m)
        {
            int[] all = GetScores(c1).Concat(GetScores(c2)).ToArray();

            LogHelper.WriteLog("ITRChkScore",
                "[Combined Input]\n" +
                $"[ITRChk1] H2={c1.CHK1_H2}, C2H2={c1.CHK1_C2H2}, C2H4={c1.CHK1_C2H4}, CH4={c1.CHK1_CH4}, C2H6={c1.CHK1_C2H6}, " +
                $"CO={c1.CHK1_CO}, CO2={c1.CHK1_CO2}, 절연내력={c1.CHK1_Dielectric_Strength}, 경년={c1.CHK1_Age}, 고장={c1.CHK1_Gojang_History}, " +
                $"Doble={c1.CHK1_Doble}, SFRA={c1.CHK1_SFRA}, HV_E={c1.CHK1_HV_E}, LV_E={c1.CHK1_LV_E}, TV_E={c1.CHK1_TV_E}, " +
                $"HV_LV={c1.CHK1_HV_LV}, HV_TV={c1.CHK1_HV_TV}, LV_TV={c1.CHK1_LV_TV}\n" +
                $"[ITRChk2] 가격={c2.CHK2_Computerized_Price}, 수분={c2.CHK2_Water_Content}, Furfural={c2.CHK2_Furfural}, " +
                $"여자전류={c2.CHK2_Excitation_Current}, 단락전류={c2.CHK2_Short_Current}, 권수전압비={c2.CHK2_Voltage_Ratio}, PD={c2.CHK2_PD}");

            int max = all.Max();
            int freq = all.Count(v => v == max);

            return HiPofTable.GetHiPof(max, freq, alpha);
        }

        // 점수 Helper
        private static int[] GetScores(ITRChk1 chk) => new[]
        {
            chk.CHK1_H2, chk.CHK1_C2H2, chk.CHK1_C2H4, chk.CHK1_CH4, chk.CHK1_C2H6, chk.CHK1_CO, chk.CHK1_CO2,
            chk.CHK1_Dielectric_Strength, chk.CHK1_Age, chk.CHK1_Gojang_History,
            chk.CHK1_Doble, chk.CHK1_SFRA,
            chk.CHK1_HV_E, chk.CHK1_LV_E, chk.CHK1_TV_E, chk.CHK1_HV_LV, chk.CHK1_HV_TV, chk.CHK1_LV_TV
        };

        private static int[] GetScores(ITRChk2 chk) => new[]
        {
            chk.CHK2_Computerized_Price, chk.CHK2_Water_Content, chk.CHK2_Furfural,
            chk.CHK2_Excitation_Current, chk.CHK2_Short_Current, chk.CHK2_Voltage_Ratio, chk.CHK2_PD
        };
    }
}
