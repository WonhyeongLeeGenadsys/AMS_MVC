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
        private Dictionary<string, int> GetRiskMatrixInternal(IEnumerable<string> codePrefixes)
        {
            using (var db = new DBHelper())
            {
                var clauses = new List<string>();
                var parameters = new DynamicParameters();
                int idx = 0;

                foreach (var pre in codePrefixes)
                {
                    var name = $"p{idx++}";
                    clauses.Add($"CODE LIKE @{name}");
                    parameters.Add(name, $"{pre}%");
                }

                var sql = $@"
                SELECT Cof AS X, PoF AS Y, COUNT(*) AS Count
                FROM RISKMATRIX
                WHERE {string.Join(" OR ", clauses)}
                GROUP BY Cof, PoF";

                var rows = db.Conn.Query(sql, parameters)
                            .Select(r => new { Key = $"{r.X},{r.Y}", Value = (int)r.Count });

                return rows.ToDictionary(r => r.Key, r => r.Value);
            }
        }
        public Dictionary<string, int> GetRiskMatrixPofCof(string prefix = null)
        {
            string[] codePrefixes;

            if (string.IsNullOrEmpty(prefix))
            {
                // ALL
                codePrefixes = new[] { "VCB", "ITR", "DCCB", "DCCABLE", "SUBMODULE" };
            }
            else if (prefix == "AC")
            {
                codePrefixes = new[] { "VCB", "ITR" };
            }
            else if (prefix == "DC")
            {
                codePrefixes = new[] { "DCCB", "DCCABLE", "SUBMODULE" };
            }
            else
            {
                // VCB, ITR 같은 개별 prefix
                codePrefixes = new[] { prefix };
            }

            return GetRiskMatrixInternal(codePrefixes);
        }

        /// <summary>
        /// 단일 코드 조회하기!!
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Dictionary<string, int> GetRiskMatrixPofCofByCode(string code)
        {
            const string sql = @"
            SELECT Cof AS X, PoF AS Y
            FROM RISKMATRIX
            WHERE CODE = @Code";
            return GetRiskMatrixInternal(new[] { code });
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

        public Riskmatrix GetLatestRiskMatrixByCode(string code)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                const string query = @"
                    SELECT TOP(1) *
                    FROM RISKMATRIX
                    WHERE CODE = @Code
                    ORDER BY 
                        CASE WHEN LASTTIME IS NULL THEN 0 ELSE 1 END DESC,
                        LASTTIME DESC";
                return dbHelper.Conn.QueryFirstOrDefault<Riskmatrix>(query, new { Code = code });
            }
        }

        /// <summary>
        /// 종합정보 건전도 도넛에서 사용할 HI 추출 함수
        /// </summary>
        /// <param name="codePrefixes"></param>
        /// <returns></returns>
        public Dictionary<string, int> GetAllHIByCode(IEnumerable<string> codePrefixes)
        {
            using (var db = new DBHelper())
            {
                var clauses = new List<string>();
                var parameters = new DynamicParameters();
                int i = 0;
                foreach (var pre in codePrefixes)
                {
                    var name = $"p{i++}";
                    clauses.Add($"CODE LIKE @{name}");
                    parameters.Add(name, $"{pre}%");
                }

                var sql = $@"
                SELECT HI, COUNT(*) AS Count
                FROM RISKMATRIX
                WHERE {string.Join(" OR ", clauses)}
                GROUP BY HI";

                var rows = db.Conn.Query(sql, parameters);
                var result = new Dictionary<string, int>();
                foreach (var r in rows)
                {
                    if (r.HI == null) continue;
                    result[r.HI.ToString()] = (int)r.Count;
                }
                return result;
            }
        }

        public Dictionary<string, int> GetLatestHIByCode(IEnumerable<string> codePrefixes)
        {
            using (var db = new DBHelper())
            {
                var clauses = new List<string>();
                var parameters = new DynamicParameters();
                int i = 0;
                foreach (var pre in codePrefixes)
                {
                    var name = $"p{i++}";
                    clauses.Add($"CODE LIKE @{name}");
                    parameters.Add(name, $"{pre}%");
                }
                var where = string.Join(" OR ", clauses);

                var sql = $@"
                WITH Latest AS (
                    SELECT
                        CODE,
                        HI,
                ROW_NUMBER() OVER(PARTITION BY CODE ORDER BY LASTTIME DESC) AS rn
                FROM RISKMATRIX
                WHERE {where}

                )
                SELECT
                    CAST(HI AS INT)   AS HI,    
                    COUNT(*)         AS Cnt
                FROM Latest
                WHERE rn = 1
                    AND HI IS NOT NULL
                GROUP BY CAST(HI AS INT);";

                var list = db.Conn.Query(sql, parameters)
                    .Select(r => new {
                        HI = (int)r.HI,
                        Cnt = (int)r.Cnt
                    });

                return list.ToDictionary(x => x.HI.ToString(), x => x.Cnt);
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

        public Result UpdateRiskMatrixHI(string code, int newHI, decimal newCof, decimal newPof)
        {
            var res = new Result(true);
            try
            {
                using (var db = new DBHelper())
                using (var conn = db.Conn)
                {
                    // 1) TBL_IDX, HI, Pof, LASTTIME 모두 가져오기
                    const string selectSql = @"
SELECT TOP 1 
    TBL_IDX,
    HI,
    Cof,
    Pof,
    LASTTIME
  FROM RISKMATRIX
 WHERE CODE = @Code
ORDER BY 
    CASE WHEN LASTTIME IS NULL THEN 0 ELSE 1 END DESC,  -- NULL은 가장 먼저
    LASTTIME DESC";
                    var latest = conn.QueryFirstOrDefault<(int TblIdx, int? HI, string Cof, string Pof, DateTime? LASTTIME)>(
                        selectSql, new { Code = code });

                    var today = DateTime.Today;
                    string cofText = newCof.ToString("F2");
                    string pofText = newPof.ToString("F6"); // 소수점 6자리

                    if (latest.TblIdx != 0 && latest.LASTTIME == null)
                    {
                        //  초기 BASICINFO 생성 때 들어간 행 
                        const string updateInitialSql = @"
UPDATE RISKMATRIX
   SET HI      = @HI,
       Cof     = @Cof,
       Pof     = @Pof,
       LASTTIME = GETDATE()
 WHERE TBL_IDX = @TblIdx";
                        conn.Execute(updateInitialSql, new
                        {
                            TblIdx = latest.TblIdx,
                            HI = newHI,
                            Cof = cofText,
                            Pof = pofText
                        });
                        res.Message = $"[{code}] 초기 행 업데이트 (HI={newHI}, Cof = {cofText}, PoF={pofText})";
                    }
                    else if (latest.LASTTIME.HasValue && latest.LASTTIME.Value.Date == today)
                    {
                        // 같은 날 이미 업데이트된 행
                        const string updateTodaySql = @"
UPDATE RISKMATRIX
   SET HI      = @HI,
       Cof     = @Cof,
       Pof     = @Pof,
       LASTTIME = GETDATE()
 WHERE TBL_IDX = @TblIdx";
                        conn.Execute(updateTodaySql, new
                        {
                            TblIdx = latest.TblIdx,
                            HI = newHI,
                            Cof = cofText,
                            Pof = pofText
                        });
                        res.Message = $"[{code}] 오늘({today:yyyy-MM-dd}) 행 업데이트 (HI={newHI}, CoF = {cofText}, PoF={pofText})";
                    }
                    else
                    {
                        // 새로운 날짜면 INSERT
                        const string insertSql = @"
INSERT INTO RISKMATRIX (CODE, HI, Cof, Pof, LASTTIME)
VALUES (@Code, @HI, @Cof, @Pof, GETDATE())";
                        conn.Execute(insertSql, new
                        {
                            Code = code,
                            HI = newHI,
                            Cof = cofText,
                            Pof = pofText
                        });
                        res.Message = $"[{code}] 새 날짜({today:yyyy-MM-dd}) 행 추가 (HI={newHI}, Cof = {cofText}, PoF={pofText})";
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
        /// 각 CODE별로 5대장비의 최신 데이터만(CoF, PoF, Code, LastTime)만 Riskmatrix 표기
        /// </summary>
        public IEnumerable<Riskmatrix> GetLatestRiskPoints()
        {
            using (var db = new DBHelper())
            {
                const string sql = @"
WITH Latest AS (
    SELECT 
      CODE      AS Code,
      Cof       AS Cof,
      Pof       AS Pof,
      HI        AS HI,
      LASTTIME  AS LastTime,
      ROW_NUMBER() OVER(PARTITION BY CODE ORDER BY LASTTIME DESC) AS rn
    FROM RISKMATRIX
    WHERE 
      CODE LIKE 'VCB%'        OR
      CODE LIKE 'ITR%'        OR
      CODE LIKE 'DCCB%'       OR
      CODE LIKE 'DCCABLE%'    OR
      CODE LIKE 'SUBMODULE%'
)
SELECT Code, Cof, Pof, HI, LastTime
FROM Latest
WHERE rn = 1;
";
                return db.Conn.Query<Riskmatrix>(sql);
            }
        }

        //Cof CODE에서 VCB, DCCB 장비 이름만 앞에 단어 찾기
        public void UpdateCoFByPrefix(string codePrefix, decimal newCoF)
        {
            var pattern = codePrefix + "%";
            const string sql = @"
            UPDATE RISKMATRIX
               SET CoF = @CoF
             WHERE CODE LIKE @Pattern";
            using (var db = new DBHelper())
            {
                db.Conn.Execute(sql, new { Pattern = pattern, CoF = newCoF });
            }
        }

        /// <summary>
        /// prefix 에 따라 CODE LIKE 조건을 걸어서
        /// CODE, LASTTIME, HI 이력을 시간 순으로 반환
        /// prefix == null  → 전체
        /// prefix == "AC"  → VCB, ITR
        /// prefix == "DC"  → DCCB, DCCABLE, SUBMODULE
        /// prefix == "VCB" → VCB 단일
        /// </summary>
        public IEnumerable<Riskmatrix> GetRiskMatrixHistory(string prefix = null)
        {
            string[] codePrefixes;
            if (string.IsNullOrEmpty(prefix))
                codePrefixes = new[] { "VCB", "ITR", "DCCB", "DCCABLE", "SUBMODULE" };
            else if (prefix == "AC")
                codePrefixes = new[] { "VCB", "ITR" };
            else if (prefix == "DC")
                codePrefixes = new[] { "DCCB", "DCCABLE", "SUBMODULE" };
            else
                codePrefixes = new[] { prefix };

            var clauses = codePrefixes
                .Select((p, i) => $"CODE LIKE @p{i}")
                .ToArray();
            var sql = $@"
            SELECT
                CODE,
                LASTTIME,
                HI,
                POF,
                COF
            FROM RISKMATRIX
            WHERE {string.Join(" OR ", clauses)}
            ORDER BY CODE, LASTTIME";

            var dp = new DynamicParameters();
            for (int i = 0; i < codePrefixes.Length; i++)
                dp.Add($"p{i}", codePrefixes[i] + "%");

            using (var db = new DBHelper())
                return db.Conn.Query<Riskmatrix>(sql, dp);
        }


    }
}