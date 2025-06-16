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

        public Result UpdateRiskMatrixHI(string code, int newHI, decimal newPof)
        {
            var res = new Result(true);
            try
            {
                using (var db = new DBHelper())
                using (var conn = db.Conn)
                {
                    // 가장 최근 행 조회
                    const string selectSql = @"
                SELECT TOP 1 HI, Pof, LASTTIME
                  FROM RISKMATRIX
                 WHERE CODE = @Code
              ORDER BY LASTTIME DESC";
                    var latest = conn.QueryFirstOrDefault<(int? HI, string Pof, DateTime? LASTTIME)>(selectSql, new { Code = code });

                    var today = DateTime.Today;
                    string pofText = newPof.ToString("F6"); // 소수점 6자리 고정

                    if (!latest.HI.HasValue)
                    {
                        const string insertSql = @"
                    INSERT INTO RISKMATRIX (CODE, HI, Pof, LASTTIME)
                    VALUES (@Code, @HI, @Pof, GETDATE())";
                        conn.Execute(insertSql, new { Code = code, HI = newHI, Pof = pofText });
                        res.Message = $"[{code}] 신규 행 추가 (HI={newHI}, PoF={pofText})";
                    }
                    else if (latest.LASTTIME.Value.Date == today)
                    {
                        const string updateSql = @"
                    UPDATE RISKMATRIX
                       SET HI = @HI
                         , Pof = @Pof
                         , LASTTIME = GETDATE()
                     WHERE CODE = @Code
                       AND CAST(LASTTIME AS DATE) = @Today";
                        conn.Execute(updateSql, new { Code = code, HI = newHI, Pof = pofText, Today = today });
                        res.Message = $"[{code}] 오늘({today:yyyy-MM-dd}) 행 업데이트 (HI={newHI}, PoF={pofText})";
                    }
                    else
                    {
                        const string insertSql = @"
                    INSERT INTO RISKMATRIX (CODE, HI, Pof, LASTTIME)
                    VALUES (@Code, @HI, @Pof, GETDATE())";
                        conn.Execute(insertSql, new { Code = code, HI = newHI, Pof = pofText });
                        res.Message = $"[{code}] 새로운 날짜({today:yyyy-MM-dd}) 행 추가 (HI={newHI}, PoF={pofText})";
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "RiskMatrix HI·PoF 업데이트 오류: " + ex.Message;
            }
            return res;
        }

        /// <summary>
        /// 각 CODE별 최신 한 건(CoF, PoF, Code, LastTime)만 Riskmatrix 모델로
        /// </summary>
        public IEnumerable<Riskmatrix> GetLatestRiskPoints(string codePrefix = null)
        {
            using (var db = new DBHelper())
            {
                const string sql = @"
WITH Latest AS (
    SELECT 
      CODE    AS Code,
      CoF     AS Cof,
      PoF     AS Pof,
      LASTTIME,
      ROW_NUMBER() OVER(PARTITION BY CODE ORDER BY LASTTIME DESC) AS rn
    FROM RISKMATRIX
    WHERE (@CodePrefix IS NULL OR CODE LIKE @Pattern)
)
SELECT Code, Cof, Pof, LASTTIME
  FROM Latest
 WHERE rn = 1;
";
                return db.Conn.Query<Riskmatrix>(sql, new
                {
                    CodePrefix = codePrefix,
                    Pattern = codePrefix + "%"
                });
            }
        }
    }
}
