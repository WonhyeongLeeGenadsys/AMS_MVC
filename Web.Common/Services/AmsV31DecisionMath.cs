using System;
using System.Collections.Generic;
using System.Linq;

namespace Web.Common
{
    /// <summary>
    /// v3.1.0의 AHP 고유벡터법과 TOPSIS 계산을 외부 패키지 없이 재현한다.
    /// </summary>
    public static class AmsV31DecisionMath
    {
        private static readonly double[,] PairwiseMatrix =
        {
            { 1d,    2d,    2d,    3d,    4d },
            { 0.5d,  1d,    1d,    3d,    3d },
            { 0.5d,  1d,    1d,    2d,    3d },
            { 0.33d, 0.33d, 0.5d,  1d,    1d },
            { 0.25d, 0.33d, 0.33d, 1d,    1d }
        };

        public static double[] CalculateAhpWeights()
        {
            int n = PairwiseMatrix.GetLength(0);
            var weights = Enumerable.Repeat(1d / n, n).ToArray();

            for (int iteration = 0; iteration < 1000; iteration++)
            {
                var next = new double[n];
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        next[i] += PairwiseMatrix[i, j] * weights[j];
                    }
                }

                double sum = next.Sum();
                if (sum <= 0d)
                {
                    break;
                }

                for (int i = 0; i < n; i++)
                {
                    next[i] /= sum;
                }

                double delta = next.Select((value, i) => Math.Abs(value - weights[i])).Max();
                weights = next;
                if (delta < 1e-12d)
                {
                    break;
                }
            }

            return weights;
        }

        public static double CalculateConsistencyRatio(double[] weights)
        {
            if (weights == null || weights.Length != 5)
            {
                return 0d;
            }

            double lambdaSum = 0d;
            for (int i = 0; i < 5; i++)
            {
                double aw = 0d;
                for (int j = 0; j < 5; j++)
                {
                    aw += PairwiseMatrix[i, j] * weights[j];
                }
                if (weights[i] > 0d)
                {
                    lambdaSum += aw / weights[i];
                }
            }

            double lambdaMax = lambdaSum / 5d;
            double consistencyIndex = (lambdaMax - 5d) / 4d;
            return consistencyIndex / 1.12d;
        }

        public static IList<AmsV31TopsisResult> Rank(double[,] matrix)
        {
            if (matrix == null)
            {
                return new List<AmsV31TopsisResult>();
            }

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            if (rows == 0 || cols != 5)
            {
                return new List<AmsV31TopsisResult>();
            }

            double[] weights = CalculateAhpWeights();
            var weighted = new double[rows, cols];
            for (int col = 0; col < cols; col++)
            {
                double norm = 0d;
                for (int row = 0; row < rows; row++)
                {
                    double value = Sanitize(matrix[row, col]);
                    norm += value * value;
                }
                norm = Math.Sqrt(norm);
                if (norm <= 0d) norm = 1d;

                for (int row = 0; row < rows; row++)
                {
                    weighted[row, col] = Sanitize(matrix[row, col]) / norm * weights[col];
                }
            }

            var idealBest = new double[cols];
            var idealWorst = new double[cols];
            for (int col = 0; col < cols; col++)
            {
                var values = Enumerable.Range(0, rows).Select(row => weighted[row, col]).ToList();
                bool minimize = col == 3; // RUL만 낮을수록 우선
                idealBest[col] = minimize ? values.Min() : values.Max();
                idealWorst[col] = minimize ? values.Max() : values.Min();
            }

            var scores = new double[rows];
            for (int row = 0; row < rows; row++)
            {
                double bestDistance = 0d;
                double worstDistance = 0d;
                for (int col = 0; col < cols; col++)
                {
                    bestDistance += Math.Pow(weighted[row, col] - idealBest[col], 2d);
                    worstDistance += Math.Pow(weighted[row, col] - idealWorst[col], 2d);
                }

                bestDistance = Math.Sqrt(bestDistance);
                worstDistance = Math.Sqrt(worstDistance);
                double denominator = bestDistance + worstDistance;
                scores[row] = denominator > 0d ? worstDistance / denominator : 0d;
            }

            int[] order = Enumerable.Range(0, rows)
                .OrderByDescending(index => scores[index])
                .ThenBy(index => index)
                .ToArray();
            var ranks = new int[rows];
            for (int index = 0; index < order.Length; index++)
            {
                ranks[order[index]] = index + 1;
            }

            return Enumerable.Range(0, rows)
                .Select(index => new AmsV31TopsisResult
                {
                    Index = index,
                    Score = scores[index],
                    Rank = ranks[index]
                })
                .ToList();
        }

        private static double Sanitize(double value)
        {
            if (double.IsNaN(value) || double.IsNegativeInfinity(value)) return 0d;
            if (double.IsPositiveInfinity(value)) return 1e10d;
            return Math.Max(0d, value);
        }
    }

    public sealed class AmsV31TopsisResult
    {
        public int Index { get; set; }
        public double Score { get; set; }
        public int Rank { get; set; }
    }
}
