using System;
using System.Collections.Generic;

namespace Web.Common
{
    public static class HiPofTable
    {
        public static (decimal HI, decimal PoF) GetHiPof(
            int maxGrade,
            int frequency,
            string equipmentKey,
            decimal frequencyCoefficient = 0.99m)
        {
            int grade = Math.Max(1, Math.Min(5, maxGrade));
            int actualFrequency = Math.Max(1, frequency);
            decimal hi = grade;

            // v3.1 Folding Function: 실제 최악등급 빈도를 사용한다.
            // 기존 테이블 방식은 frequency가 5를 넘으면 잘라서 HI가 과소평가됐다.
            if (grade != 1 && grade != 5 && actualFrequency >= 2)
            {
                decimal ratio = (actualFrequency - 1) / 5m;
                hi = grade + (frequencyCoefficient * ratio * ratio);
            }

            hi = Math.Max(1m, Math.Min(5m, hi));
            decimal pof = (decimal)(AmsV31Config.CalculateDiagnosticPof((double)hi, equipmentKey) * 100d);

            return (Math.Round(hi, 2), Math.Round(pof, 4));
        }

        // 기존 호출부 호환용. 장비 종류가 없는 경우 VCB 계수를 사용한다.
        public static (decimal HI, decimal PoF) GetHiPof(int maxGrade, int frequency, decimal frequencyCoefficient = 0.99m)
        {
            return GetHiPof(maxGrade, frequency, "VCB", frequencyCoefficient);
        }
    }
}
