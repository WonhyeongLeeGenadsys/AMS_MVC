using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LaModule
{
    public enum AlgorithmType
    {
        None,
        Weibull,        // 형상모수, 척도모수를 사용하는 계산 방식
        FailureRate,    // 고장률만 사용하는 계산 방식
    }

    public class LaAlgorithm
    {
        // 계산 결과를 외부에서 접근할 수 있도록 공개하는 속성들
        public double[] TimeValues { get; private set; }
        public double[] Reliability { get; private set; }
        public double[] HazardNormalized { get; private set; }
        public double[] PdfNormalized { get; private set; }
        public double B3Life { get; private set; }

        private int numPoints = 100;  // 시간 축에 사용할 점의 개수

        /// <summary>
        /// 고장률만 주어진 경우의 계산
        /// B3Life = -ln(0.03) / failureRate
        /// </summary>
        public void SetFailureRate(double failureRate)
        {
            // 고장률 데이터를 이용하여 B3 수명 계산
            B3Life = CalculateB3Life(failureRate);

            // 0부터 B3Life * 2까지 선형 분포의 시간축 생성
            double[] tValues = GenerateLinspace(0, B3Life * 2, numPoints);

            // B3Life * 2 이하의 시간만 사용
            double max_time = Math.Min(tValues.Max(), B3Life * 2);
            double[] filteredT = tValues.Where(value => value <= max_time).ToArray();

            // 신뢰도 함수: R(t) = exp(-λt)
            Reliability = filteredT.Select(value => Math.Exp(-failureRate * value)).ToArray();

            // 고장률 함수: 상수값인 λ (정규화 시 모두 1)
            double[] hazard_rate = filteredT.Select(val => failureRate).ToArray();
            double max_hazard = hazard_rate.Max();
            HazardNormalized = hazard_rate.Select(val => val / max_hazard).ToArray();

            // 확률 밀도 함수: f(t) = λ * exp(-λt)
            var pdf = filteredT.Select(value => failureRate * Math.Exp(-failureRate * value)).ToArray();
            double max_pdf = pdf.Max();
            PdfNormalized = pdf.Select(val => val / max_pdf).ToArray();

            // 최종 시간값 배열 세팅
            TimeValues = filteredT;
        }

        /// <summary>
        /// Weibull 방식을 이용한 B3 수명 계산
        /// B3Life = η * (-ln(0.03))^(1/β)
        /// </summary>
        private double CalculateB3Life(double beta, double eta)
        {
            double lifetime = eta * Math.Pow(-Math.Log(0.03), 1 / beta);
            lifetime = Math.Round(lifetime, 2, MidpointRounding.AwayFromZero);
            return lifetime;
        }

        /// <summary>
        /// 고장률을 이용한 B3 수명 계산
        /// B3Life = -ln(0.03) / failure_rate
        /// </summary>
        private double CalculateB3Life(double failure_rate)
        {
            double result = -Math.Log(0.03) / failure_rate;
            return result;
        }

        /// <summary>
        /// 선형 구간 배열 생성: 시작값부터 끝값까지 numPoints개의 점을 선형적으로 생성합니다.
        /// </summary>
        private double[] GenerateLinspace(double start, double end, int numPoints)
        {
            if (numPoints < 2)
                throw new ArgumentException("numPoints must be at least 2");

            return Enumerable.Range(0, numPoints)
                             .Select(i => start + (end - start) * i / (numPoints - 1))
                             .ToArray();
        }
    }
}
