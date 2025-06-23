using System;
using System.Collections.Generic;
using System.Linq;

namespace AMS_MVC.Utlity
{
    public static class HiPofTable
    {
        private static readonly Dictionary<(int maxGrade, int frequency), decimal> HiTable = new Dictionary<(int, int), decimal>
        {
            {(1, 1), 1.00m},

            {(2, 1), 2.00m},
            {(2, 2), 2.04m},
            {(2, 3), 2.16m},
            {(2, 4), 2.36m},
            {(2, 5), 2.63m},

            {(3, 1), 3.00m},
            {(3, 2), 3.04m},
            {(3, 3), 3.16m},
            {(3, 4), 3.36m},
            {(3, 5), 3.63m},

            {(4, 1), 4.00m},
            {(4, 2), 4.04m},
            {(4, 3), 4.16m},
            {(4, 4), 4.36m},
            {(4, 5), 4.63m},

            {(5, 1), 5.00m},
            {(5, 2), 5.00m},  
            {(5, 3), 5.00m},  
            {(5, 4), 5.00m},  
            {(5, 5), 5.00m}  
        };

        private static readonly Dictionary<decimal, decimal> PofTable = new Dictionary<decimal, decimal>
        {
            { 1.00m, 0.0000m },
            { 2.00m, 0.0045m },
            { 2.04m, 0.0055m },
            { 2.16m, 0.0100m },
            { 2.36m, 0.0270m },
            { 2.63m, 0.1078m },
            { 3.00m, 0.6693m },
            { 3.04m, 0.8146m },
            { 3.16m, 1.4658m },
            { 3.36m, 3.8494m },
            { 3.63m, 13.8000m },
            { 4.00m, 50.0000m },
            { 4.04m, 54.9339m },
            { 4.16m, 68.8261m },
            { 4.36m, 85.5944m },
            { 4.63m, 95.9612m },
            { 5.00m, 100m }
        };

        public static (decimal HI, decimal PoF) GetHiPof(int maxGrade, int frequency, decimal alpha = 1.0m)
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

            // 4) alpha 곱하고 가장 근접한 HI 값 찾은 뒤 PoF 조회
            hi *= alpha;
            var nearestHi = FindNearestHI(hi);
            var pof = PofTable[nearestHi];

            return (nearestHi, pof);
        }

        private static decimal FindNearestHI(decimal hi)
        {
            decimal nearest = 1.00m;
            decimal minDiff = decimal.MaxValue;

            foreach (var key in PofTable.Keys)
            {
                var diff = System.Math.Abs(key - hi);
                if (diff < minDiff)
                {
                    minDiff = diff;
                    nearest = key;
                }
            }
            return nearest;
        }
    }
}
