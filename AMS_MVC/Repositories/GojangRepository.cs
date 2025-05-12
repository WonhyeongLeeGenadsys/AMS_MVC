using AMS_MVC.Database;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS_MVC.Repositories
{
    public class GojangRepository
    {
        // [1] 특정 테이블(= 특정 장치)만 조회
        public List<dynamic> GetGojangData(
            string failureHistoryTable,
            string basicInfoTable,
            string codeField,
            string basicInfoAlias,
            string entityName)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                string query = $@"
    SELECT
        ROW_NUMBER() OVER (ORDER BY CAST(F.FAIL_PERIOD AS INT) DESC) AS Priority,
        {basicInfoAlias}.{codeField} AS Code,
        '{entityName}' AS Name,
        {basicInfoAlias}.SERIAL_NO AS Serial_No,
        F.FAIL_WEATHER AS Weather,
        F.FAIL_TEMP AS Temp,
        F.FAIL_HUM AS Hum,
        F.FAIL_REASON AS Reason,
        F.FAIL_STATUS AS Status,
        F.FAIL_PART AS Part,
        F.FAIL_PERIOD AS Period,
        F.FAIL_FINDER AS Finder,
        F.FAIL_REPAIR_DATE AS Date
    FROM {failureHistoryTable} F
    LEFT JOIN {basicInfoTable} {basicInfoAlias} 
           ON F.{codeField} = {basicInfoAlias}.{codeField}";

                return dbHelper.Conn.Query(query).AsList();
            }
        }


        public List<dynamic> GetGojangAll()
        {
            // 1) 각 항목(테이블) 설정
            var configs = new[]
            {
                new { FailureTable = "VCB_FAILURE_HISTORY",      BasicTable = "VCB_BASICINFO",      CodeField = "VCB_CODE",      Alias = "VCB",      EntityName = "VCB" },
                new { FailureTable = "ITR_FAILURE_HISTORY", BasicTable = "ITR_BASICINFO", CodeField = "ITR_CODE",      Alias = "ITR",     EntityName = "ITR" },
                new { FailureTable = "DCCB_FAILURE_HISTORY",     BasicTable = "DCCB_BASICINFO",     CodeField = "DCCB_CODE",     Alias = "DCCB",    EntityName = "DCCB" },
                new { FailureTable = "DCCABLE_FAILURE_HISTORY",   BasicTable = "DCCABLE_BASICINFO",   CodeField = "DCCABLE_CODE", Alias = "DCCABLE", EntityName = "DCCABLE" },
                new { FailureTable = "SUBMODULE_FAILURE_HISTORY", BasicTable = "SUBMODULE_BASICINFO", CodeField = "SUBMODULE_CODE", Alias = "SUBMODULE", EntityName = "SUBMODULE" }
            };

            var unionQueries = new List<string>();

            foreach (var cfg in configs)
            {
                string part = $@"
                    SELECT
                        CAST(F.FAIL_PERIOD AS INT) AS FailPeriod,  -- 통합 정렬용 (INT 변환)
                        {cfg.Alias}.{cfg.CodeField} AS Code,
                        '{cfg.EntityName}' AS Name,
                        {cfg.Alias}.SERIAL_NO AS Serial_No,
                        F.FAIL_WEATHER AS Weather,
                        F.FAIL_TEMP AS Temp,
                        F.FAIL_HUM AS Hum,
                        F.FAIL_REASON AS Reason,
                        F.FAIL_STATUS AS Status,
                        F.FAIL_PART AS Part,
                        F.FAIL_PERIOD AS Period,     -- 원본 문자열
                        F.FAIL_FINDER AS Finder,
                        F.FAIL_REPAIR_DATE AS [Date]
                    FROM {cfg.FailureTable} F
                    LEFT JOIN {cfg.BasicTable} {cfg.Alias} 
                           ON F.{cfg.CodeField} = {cfg.Alias}.{cfg.CodeField}
                ";
                unionQueries.Add(part);
            }

            string fullQuery = $@"
                WITH CombinedData AS
                (
                    {string.Join(" UNION ALL ", unionQueries)}
                )
                SELECT
                    ROW_NUMBER() OVER (ORDER BY FailPeriod DESC) AS Priority,
                    Code,
                    Name,
                    Serial_No,
                    Weather,
                    Temp,
                    Hum,
                    Reason,
                    Status,
                    Part,
                    Period,
                    Finder,
                    [Date]
                FROM CombinedData
                ORDER BY FailPeriod DESC;  -- 혹은 Priority ASC
            ";

            using (DBHelper dbHelper = new DBHelper())
            {
                return dbHelper.Conn.Query(fullQuery).AsList();
            }
        }
    }
}    