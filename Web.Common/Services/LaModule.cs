using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Web.Common
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
        /// Weibull 계산: 형상모수와 척도모수를 사용
        /// </summary>
        /// <param name="shape">형상모수 (β)</param>
        /// <param name="scale">척도모수 (η)</param>
        /// <param name="years">계산할 기간(년) - 참고용 (여기선 사용되지 않음)</param>
        public void SetWeibull(double shape, double scale, double years)
        {
            // B3 수명 계산: B3Life = η * (-ln(0.03))^(1/β)
            B3Life = CalculateB3Life(shape, scale);

            // 시간축 생성: 0부터 scale*2까지 선형 분포
            TimeValues = GenerateLinspace(0, scale * 2, numPoints);
            double max_time = Math.Min(TimeValues.Max(), B3Life * 2);
            var filteredT = TimeValues.Where(value => value <= max_time).ToArray();

            // 신뢰도 함수: R(t) = exp(-(t/η)^β)
            Reliability = filteredT.Select(value => Math.Exp(-Math.Pow(value / scale, shape))).ToArray();

            // 고장률 함수: h(t) = (β/η) * (t/η)^(β-1)
            var hazard_rate = filteredT.Select(val => (shape / scale) * Math.Pow(val / scale, shape - 1)).ToArray();
            double max_hazard = hazard_rate.Max();
            HazardNormalized = hazard_rate.Select(val => val / max_hazard).ToArray();

            // 확률 밀도 함수: f(t) = (β/η) * (t/η)^(β-1) * exp(-(t/η)^β)
            var pdf = filteredT.Select(val =>
                (shape / scale) * Math.Pow(val / scale, shape - 1) * Math.Exp(-Math.Pow(val / scale, shape))
            ).ToArray();
            double max_pdf = pdf.Max();
            PdfNormalized = pdf.Select(val => val / max_pdf).ToArray();
        }

        /// <summary>
        /// 고장률 기반 계산: 고장률만 주어진 경우
        /// </summary>
        /// <param name="failureRate">고장률 (λ)</param>
        public void SetFailureRate(double failureRate)
        {
            // B3 수명 계산: B3Life = -ln(0.03) / λ
            B3Life = CalculateB3Life(failureRate);

            // 시간축: 0부터 2*B3Life까지 선형 분포
            TimeValues = GenerateLinspace(0, B3Life * 2, numPoints);
            // 신뢰도 함수: R(t) = exp(-λt)
            Reliability = TimeValues.Select(t => Math.Exp(-failureRate * t)).ToArray();

            // 고장률은 상수: λ (정규화 시 1)
            double[] hazard_rate = TimeValues.Select(t => failureRate).ToArray();
            double max_hazard = hazard_rate.Max();
            HazardNormalized = hazard_rate.Select(val => val / max_hazard).ToArray();

            // 확률 밀도 함수: f(t) = λ * exp(-λt)
            var pdf = TimeValues.Select(t => failureRate * Math.Exp(-failureRate * t)).ToArray();
            double max_pdf = pdf.Max();
            PdfNormalized = pdf.Select(val => val / max_pdf).ToArray();
        }

        /// <summary>
        /// Weibull 방식을 이용한 B3 수명 계산
        /// B3Life = η * (-ln(0.03))^(1/β)
        /// </summary>
        /// <param name="beta">형상모수 (β)</param>
        /// <param name="eta">척도모수 (η)</param>
        /// <returns>B3 수명 (시간 단위)</returns>
        private double CalculateB3Life(double beta, double eta)
        {
            double lifetime = eta * Math.Pow(-Math.Log(0.03), 1 / beta);
            lifetime = Math.Round(lifetime, 2, MidpointRounding.AwayFromZero);
            return lifetime;
        }

        /// <summary>
        /// 고장률만을 이용한 B3 수명 계산
        /// B3Life = -ln(0.03) / λ
        /// </summary>
        /// <param name="failure_rate">고장률 (λ)</param>
        /// <returns>B3 수명</returns>
        private double CalculateB3Life(double failure_rate)
        {
            return -Math.Log(0.03) / failure_rate;
        }

        /// <summary>
        /// 선형 구간 배열 생성: 시작 값부터 끝 값까지 numPoints개의 점을 생성
        /// </summary>
        /// <param name="start">시작 값</param>
        /// <param name="end">끝 값</param>
        /// <param name="numPoints">생성할 점의 개수 (최소 2)</param>
        /// <returns>선형 구간 배열</returns>
        private double[] GenerateLinspace(double start, double end, int numPoints)
        {
            if (numPoints < 2)
            {
                throw new ArgumentException("numPoints must be at least 2");
            }
            return Enumerable.Range(0, numPoints)
                             .Select(i => start + (end - start) * i / (numPoints - 1))
                             .ToArray();
        }
    }
}
