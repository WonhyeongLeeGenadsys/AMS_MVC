using AMS_MVC.Database;
using AMS_MVC.Models;
using AMS_MVC.Utlity;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS_MVC.Repositories
{
    public class RiskmatrixRepository
    {
        private Dictionary<string, int> GetRiskMatrixInternal(string query, object parameters)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                var data = dbHelper.Conn.Query(query, parameters).ToList();

                // 5x5 Matrix 초기화 (인덱스 0~4)
                var matrix = new int[5, 5];

                // 기존: CoF, PoF 값을 0-based 인덱스로 변환 후 집계
                foreach (var item in data)
                {
                    // item.CoF, item.PoF 가 null 이면 "0"으로 간주
                    var cofStr = item.CoF?.ToString() ?? "0";
                    var pofStr = item.PoF?.ToString() ?? "0";

                    int cof = Clamp(int.Parse(cofStr) - 1, 0, 4);
                    int pof = Clamp(int.Parse(pofStr) - 1, 0, 4);

                    matrix[pof, cof]++;
                }

                // 결과를 Dictionary<string, int>로 변환 (키: "pofIndex,cofIndex")
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
        }

        private int Clamp(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        /// <summary>
        /// VCB 코드로 Riskmatrix 데이터를 조회 (단일 VCB에 해당하는 데이터)
        /// </summary>
        public Dictionary<string, int> GetRiskMatrixPofCofByCode(string code)
        {
            const string query = @"
                SELECT CoF, PoF
                FROM RISKMATRIX
                WHERE CODE = @Code";
            var parameters = new { Code = code };
            return GetRiskMatrixInternal(query, parameters);
        }

        /// <summary>
        /// codePrefix (예: "VCB")로 Riskmatrix 데이터를 조회 (여러 건 누적)
        /// </summary>
        public Dictionary<string, int> GetRiskMatrixPofCof(string codePrefix = null)
        {
            const string query = @"
                SELECT CoF, PoF
                FROM RISKMATRIX
                WHERE (@CodePrefix IS NULL OR CODE LIKE @CodePattern)";
            var parameters = new
            {
                CodePrefix = codePrefix,
                CodePattern = codePrefix != null ? $"{codePrefix}%" : null
            };
            return GetRiskMatrixInternal(query, parameters);
        }

        /// <summary>
        /// 해당 VCB_CODE의 Riskmatrix 행 전체를 조회 (HI 등 추가 속성 포함)
        /// </summary>
        public Riskmatrix GetRiskMatrixByCode(string code)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                const string query = @"
                    SELECT *
                    FROM RISKMATRIX
                    WHERE CODE = @Code";
                return dbHelper.Conn.QueryFirstOrDefault<Riskmatrix>(query, new { Code = code });
            }
        }

        /// <summary>
        /// HI 값을 기준으로 집계한 데이터를 반환
        /// HI 값별 건수를 Dictionary<string, int> 형태로 반환
        /// </summary>
        public Dictionary<string, int> GetAggregatedHI(string codePrefix = null)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                const string query = @"
            SELECT HI, COUNT(*) AS Count
            FROM RISKMATRIX
            WHERE (@CodePrefix IS NULL OR CODE LIKE @CodePattern)
            GROUP BY HI";
                var parameters = new
                {
                    CodePrefix = codePrefix,
                    CodePattern = codePrefix != null ? $"{codePrefix}%" : null
                };
                var data = dbHelper.Conn.Query(query, parameters).ToList();

                var result = new Dictionary<string, int>();

                foreach (var item in data)
                {
                    // HI가 null 이면 건너뛰기
                    if (item.HI == null)
                        continue;

                    var hiKey = item.HI.ToString();    
                    var count = Convert.ToInt32(item.Count);

                    result[hiKey] = count;
                }

                return result;
            }
        }
        public IEnumerable<int> GetHIList(string codePrefix = null)
        {
            using (var db = new DBHelper())
            {
                // CODE가 prefix + 숫자 로 되어 있다고 가정
                var sql = @"
                SELECT CAST(HI AS INT)
                FROM RISKMATRIX
                WHERE CODE LIKE @Pattern
                  AND HI IS NOT NULL";
                return db.Conn
                         .Query<int>(sql, new { Pattern = codePrefix + "%" })
                         .ToList();
            }
        }

        public IEnumerable<dynamic> GetDevicesByDateRange(string dateType, DateTime start, DateTime end)
        {
            using (DBHelper dbHelper = new DBHelper())
            {            
                string columnName = "";
                switch (dateType.ToLower())
                {
                    case "install":
                        columnName = "Install_Date";
                        break;
                    case "press":
                        columnName = "Pressurized_Date";
                        break;
                    case "oper":
                    default:
                        columnName = "Operating_Date";
                        break;
                }

                var query = $@"
                    SELECT 'VCB' as EquipmentType, b.*, r.HI
                    FROM RISKMATRIX r
                    INNER JOIN VCB_BASICINFO b ON r.CODE = b.VCB_CODE
                    WHERE {columnName} BETWEEN @StartDate AND @EndDate

                    UNION ALL
                    SELECT 'DCCB' as EquipmentType, b.*, r.HI
                    FROM RISKMATRIX r
                    INNER JOIN DCCB_BASICINFO b ON r.CODE = b.DCCB_CODE
                    WHERE {columnName} BETWEEN @StartDate AND @EndDate

                    UNION ALL
                    SELECT 'DCCABLE' as EquipmentType, b.*, r.HI
                    FROM RISKMATRIX r
                    INNER JOIN DCCABLE_BASICINFO b ON r.CODE = b.DCCABLE_CODE
                    WHERE {columnName} BETWEEN @StartDate AND @EndDate

                    UNION ALL
                    SELECT 'ITR' as EquipmentType, b.*, r.HI
                    FROM RISKMATRIX r
                    INNER JOIN ITR_BASICINFO b ON r.CODE = b.ITR_CODE
                    WHERE {columnName} BETWEEN @StartDate AND @EndDate

                    UNION ALL
                    SELECT 'SUBMODULE' as EquipmentType, b.*, r.HI
                    FROM RISKMATRIX r
                    INNER JOIN SUBMODULE_BASICINFO b ON r.CODE = b.SUBMODULE_CODE
                    WHERE {columnName} BETWEEN @StartDate AND @EndDate
                ";

                var result = dbHelper.Conn.Query(query, new
                {
                    StartDate = start,
                    EndDate = end
                });
                return result;
            }
        }

        public Result UpdateRiskMatrixHI(string code, int newHI)
        {
            var res = new Result(true);
            try
            {
                using (var dbHelper = new DBHelper())
                using (var conn = dbHelper.Conn)
                {
                    // 1) UPDATE 시도
                    const string updateSql = @"
                UPDATE RISKMATRIX
                   SET HI = @HI
                 WHERE CODE = @Code";
                    int affected = conn.Execute(updateSql, new { HI = newHI, Code = code });

                    if (affected > 0)
                    {
                        // 기존 레코드가 있어서 업데이트 된 경우
                        res.Message = $"RiskMatrix HI가 {code}에 대해 갱신되었습니다 (HI = {newHI}).";
                    }
                    else
                    {
                        // 업데이트된 행이 없으면 INSERT
                        const string insertSql = @"
                    INSERT INTO RISKMATRIX (CODE, HI)
                    VALUES (@Code, @HI)";
                        int inserted = conn.Execute(insertSql, new { HI = newHI, Code = code });
                        if (inserted > 0)
                        {
                            res.Message = $"RiskMatrix에 신규 행이 추가되었습니다 (CODE = {code}, HI = {newHI}).";
                        }
                        else
                        {
                            res.IsSuccess = false;
                            res.Message = $"RiskMatrix HI 갱신 및 신규 추가 모두 실패: {code}";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "RiskMatrix HI 업데이트 오류: " + ex.Message;
            }
            return res;
        }
    }
}
