using System;
using System.Collections.Generic;
using System.Linq;

namespace Web.Common
{
    public static class HiPofTable
    {
        private static readonly Dictionary<(int maxGrade, int frequency), decimal> HiTable = new Dictionary<(int, int), decimal>
        {
            {(1, 1), 1.00m},

            {(2, 1), 2.00m},
            {(2, 2), 2.0396m},
            {(2, 3), 2.1584m},
            {(2, 4), 2.3564m},
            {(2, 5), 2.6336m},

            {(3, 1), 3.00m},
            {(3, 2), 3.0396m},
            {(3, 3), 3.1584m},
            {(3, 4), 3.3564m},
            {(3, 5), 3.6336m},

            {(4, 1), 4.00m},
            {(4, 2), 4.0396m},
            {(4, 3), 4.1584m},
            {(4, 4), 4.3564m},
            {(4, 5), 4.6336m},

            {(5, 1), 5.00m},
            {(5, 2), 5.00m},  
            {(5, 3), 5.00m},  
            {(5, 4), 5.00m},  
            {(5, 5), 5.00m}  
        };

        public static (decimal HI, decimal PoF) GetHiPof(int maxGrade, int frequency, decimal alpha = 1.00m)
        {
            // 1) 해당 maxGrade에 대해 테이블에 정의된 모든 빈도(frequency) 값을 찾는다
            var freqs = HiTable.Keys
                               .Where(k => k.maxGrade == maxGrade)
                               .Select(k => k.frequency)
                               .ToList();

            // 정의된 빈도가 하나도 없으면, 기본적으로 (maxGrade,1) 만 있다고 가정
            if (!freqs.Any())
                freqs = new List<int> { 1 };

            // 2) 조회할 빈도를 [1 .. freqs.Max()] 범위로 clamp
            var freqMax = freqs.Max();
            var lookupFreq = Math.Min(Math.Max(frequency, 1), freqMax);

            // 3) clamp된 빈도로 HI 조회
            if (!HiTable.TryGetValue((maxGrade, lookupFreq), out decimal hi))
            {
                // 혹시 실패하면, maxGrade 값 자체로 처리 (또는 1.0m)
                hi = maxGrade;
            }

            // 4) 원본 검증 데이터셋의 로지스틱 공식으로 PoF(%) 계산
            hi *= alpha;
            var pof = CalculatePofPercent(hi);

            return (hi, pof);
        }

        private static decimal CalculatePofPercent(decimal hi)
        {
            double exponent = -5d * ((double)hi - 4d);
            double probabilityRatio = 1d / (1d + Math.Exp(exponent));
            return (decimal)(probabilityRatio * 100d);
        }
    }
}
