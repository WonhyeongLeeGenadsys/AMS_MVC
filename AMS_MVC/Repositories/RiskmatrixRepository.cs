using AMS_MVC.Database;
using AMS_MVC.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS_MVC.Repositories
{
    public class RiskmatrixRepository
    {
        private readonly DBHelper mDb;

        public RiskmatrixRepository()
        {
            mDb = new DBHelper();
        }

        public Dictionary<string, int> GetRiskMatrixPofCof(string codePrefix = null)
        {
            const string query = @"
        SELECT CoF, PoF
        FROM RISKMATRIX
        WHERE(@CodePrefix IS NULL OR CODE LIKE @CodePattern)";

            //매개변수 설정
            var parameters = new { CodePrefix = codePrefix, CodePattern = $"{codePrefix}%" };

            //데이터 조회
            var data = mDb.Conn.Query(query, parameters).ToList();

            // 5x5 Matrix 초기화
            var matrix = new int[5, 5];

            //데이터 매핑하기
            foreach (var item in data)
            {
                int cof = Clamp(int.Parse(item.CoF) - 1, 0, 4); // 0-based index로 변환
                int pof = Clamp(int.Parse(item.PoF) - 1, 0, 4);
                matrix[pof, cof]++;
            }

            // Dictionary 형태로 반환하기
            var result = new Dictionary<string, int>();
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    result[$"{i},{j}"] = matrix[i, j];
                }
            }
            return result;
        }
        private int Clamp(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        /// <summary>
        /// CODE테이블의 앞글자(EX: V or I) 검증해서 데이터 불러오기
        /// </summary>
        /// <param name="codePrefix"></param>
        /// <returns></returns>
        public List<Riskmatrix> GetMatrixByCodePrefix(string codePrefix)
        {
            const string query = @"
            SELECT *
            FROM RISKMATRIX
            WHERE CODE LIKE @CodePrefix";

            return mDb.Conn.Query<Riskmatrix>(query, new { CodePrefix = codePrefix + "%" }).AsList();
        }
    }
}