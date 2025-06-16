using System.Collections.Generic;

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
        };

        private static readonly Dictionary<decimal, decimal> PofTable = new Dictionary<decimal, decimal>
        {
            { 1.00m, 0.000000m },
            { 2.00m, 0.000045m },
            { 2.04m, 0.000055m },
            { 2.16m, 0.000100m },
            { 2.36m, 0.000270m },
            { 2.63m, 0.001078m },
            { 3.00m, 0.006693m },
            { 3.04m, 0.008146m },
            { 3.16m, 0.014658m },
            { 3.36m, 0.038494m },
            { 3.63m, 0.138000m },
            { 4.00m, 0.500000m },
            { 4.04m, 0.549339m },
            { 4.16m, 0.688261m },
            { 4.36m, 0.855944m },
            { 4.63m, 0.959612m },
            { 5.00m, 1.000000m }
        };

        public static (decimal HI, decimal PoF) GetHiPof(int maxGrade, int frequency, decimal alpha = 1.0m)
        {
            if (!HiTable.TryGetValue((maxGrade, frequency), out decimal hi))
                hi = 1.00m; 

            hi *= alpha;

            decimal nearestHi = FindNearestHI(hi);

            decimal pof = PofTable.TryGetValue(nearestHi, out decimal value) ? value : 0.0m;

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
