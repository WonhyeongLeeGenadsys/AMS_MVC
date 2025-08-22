//using System.Linq;
//using AMS_MVC.Models;
//using AMS_MVC.Utlity;

//namespace AMS_MVC.Services
//{
//    public class ITRChkScoreCalculator
//    {
//            public (decimal HI, decimal PoF) CalculateHiPof(ITRChk1 chk, decimal alpha = 1.00m)
//        {

//            int[] scores = new int[]
//            {
//                // DGA
//                chk.CHK1_H2,
//                chk.CHK1_C2H2,
//                chk.CHK1_C2H4,
//                chk.CHK1_CH4,
//                chk.CHK1_C2H6,
//                chk.CHK1_CO,
//                chk.CHK1_CO2,

//                // 절연 강도/노후도 등
//                chk.CHK1_Dielectric_Strength,
//                //chk.CHK1_Remain_Life, // 잔여 수명 주석처리 사용자가 추가시에 입력안함
//                chk.CHK1_Age,
//                chk.CHK1_Gojang_History,

//                // 절연진동/기계적 시험
//                chk.CHK1_Doble,
//                chk.CHK1_SFRA,

//                // 절연저항
//                chk.CHK1_HV_E,
//                chk.CHK1_LV_E,
//                chk.CHK1_TV_E,
//                chk.CHK1_HV_LV,
//                chk.CHK1_HV_TV,
//                chk.CHK1_LV_TV
//            };

//            int maxGrade = scores.Max();
//            int frequency = scores.Count(s => s == maxGrade);

//            return HiPofTable.GetHiPof(maxGrade, frequency, alpha);
//        }

//        /// <summary>
//        /// 정밀점검(ITRChk2)에 포함된 모든 수치형 변수 중 최대값을 Folding Function으로 계산
//        /// </summary>
//        public (decimal HI, decimal PoF) CalculateHiPof(ITRChk2 chk, decimal alpha = 1.00m) //alpha는 보정계수 0.99 고정
//        {
//            int[] scores = new int[]
//            {
//                chk.CHK2_Computerized_Price,
//                chk.CHK2_Water_Content,
//                chk.CHK2_Furfural,
//                chk.CHK2_Excitation_Current,
//                chk.CHK2_Short_Current,
//                chk.CHK2_Voltage_Ratio,
//                chk.CHK2_PD
//            };

//            int maxGrade = scores.Max();
//            int frequency = scores.Count(s => s == maxGrade);

//            return HiPofTable.GetHiPof(maxGrade, frequency, alpha);
//        }
//    }
//}

// Services/ITRChkScoreCalculator.cs
using AMS_MVC.Models;
using AMS_MVC.Utlity;
using System.Linq;

namespace AMS_MVC.Services
{
    public class ITRChkScoreCalculator
    {
        // 기존: ITRChk1 전용
        public (decimal HI, decimal PoF) CalculateHiPof(ITRChk1 chk, decimal alpha = 1.00m)
        {
            int[] s1 = GetScores(chk);
            int max = s1.Max();
            int freq = s1.Count(v => v == max);
            return HiPofTable.GetHiPof(max, freq, alpha);
        }

        // 기존: ITRChk2 전용
        public (decimal HI, decimal PoF) CalculateHiPof(ITRChk2 chk, decimal alpha = 1.00m)
        {
            int[] s2 = GetScores(chk);
            int max = s2.Max();
            int freq = s2.Count(v => v == max);
            return HiPofTable.GetHiPof(max, freq, alpha);
        }

        // 추가: 1+2 함께 계산
        public (decimal HI, decimal PoF) CalculateHiPofCombined(ITRChk1 c1, ITRChk2 c2, decimal alpha = 1.00m)
        {
            int[] all = GetScores(c1).Concat(GetScores(c2)).ToArray();
            int max = all.Max();
            int freq = all.Count(v => v == max);
            return HiPofTable.GetHiPof(max, freq, alpha);
        }

        // 점수 추출 헬퍼들
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
