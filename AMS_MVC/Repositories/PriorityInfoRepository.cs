using AMS_MVC.Database;
using AMS_MVC.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AMS_MVC.Repositories
{
    public class PriorityInfoRepository
    {
        /// <summary>
        /// 특정 기본정보 테이블에 대해 우선순위 리스트를 반환합니다. (기존 메서드)
        /// </summary>
        public List<dynamic> GetPriority(
            string basicInfoTable,
            string codeField,
            string alias,
            string entityName,
            string sortValue)
        {
            using (var dbHelper = new DBHelper())
            {
                string query = $@"
                SELECT
                    ROW_NUMBER() OVER (
                        ORDER BY 
                            CAST(r.COF AS DECIMAL(18,2)) * CAST(r.POF AS DECIMAL(18,2)) DESC,
                            DATEDIFF(YEAR, {alias}.INSTALL_DATE, GETDATE()) DESC
                    ) AS Priority,
                    '{sortValue}'   AS Sort,
                    {alias}.{codeField} AS Code,
                    {alias}.SERIAL_NO      AS Serial_No,
                    '{entityName}'         AS Name,
                    {alias}.INSTALL_DATE   AS Install_Date,
                    {alias}.OPERATING_DATE AS Operating_Date,
                    DATEDIFF(YEAR, {alias}.INSTALL_DATE, GETDATE()) AS UsagePeriod,
                    {alias}.PRICE   AS Price,
                    {alias}.RATED_V AS Rated_V,
                    {alias}.RATED_A AS Rated_A,
                    {alias}.MAKE_COMPANY AS Make_Company,
                    {alias}.WRITER       AS Writer,
                    r.COF AS CoF,
                    r.POF AS PoF,
                    CAST(r.COF AS DECIMAL(18,2)) * CAST(r.POF AS DECIMAL(18,2)) AS RiskScore,
                    r.HI AS HI
                FROM {basicInfoTable} {alias}
                LEFT JOIN RISKMATRIX r
                  ON {alias}.{codeField} = r.CODE;
                ";
                return dbHelper.Conn.Query(query).AsList();
            }
        }

        /// <summary>
        /// 모든 장비(VCB, ITR, DCCB, DC Cable, Sub Module)에 대한 우선순위 리스트를 반환합니다.
        /// — 최신 리스크값만, 최신 설치일만 남기도록 CTE 적용
        /// </summary>
        public List<PriorityInfo> GetPriorityInfo()
        {
            // 테이블별 설정: Alias, 코드필드, 엔티티명, Sort(AC/DC)
            var configs = new[]
            {
                new { Table = "VCB_BASICINFO",     CodeField = "VCB_CODE",       Alias = "b", EntityName = "VCB",        Sort = "AC" },
                new { Table = "ITR_BASICINFO",     CodeField = "ITR_CODE",       Alias = "i", EntityName = "Interface TR",Sort = "AC" },
                new { Table = "DCCB_BASICINFO",    CodeField = "DCCB_CODE",      Alias = "d", EntityName = "DC CB",      Sort = "DC" },
                new { Table = "DCCABLE_BASICINFO", CodeField = "DCCABLE_CODE",   Alias = "c", EntityName = "DC Cable",   Sort = "DC" },
                new { Table = "SUBMODULE_BASICINFO",CodeField = "SUBMODULE_CODE", Alias = "s", EntityName = "Sub Module", Sort = "DC" }
            };

            // 1) LatestRisk CTE: 장비별 가장 최근 리스크 한 건만
            var withLatestRisk = @"
            WITH LatestRisk AS (
                SELECT CODE, COF, POF, HI
                FROM (
                    SELECT
                    CODE,
                    COF,
                    POF,
                    HI,
                ROW_NUMBER() OVER (
                PARTITION BY CODE
                ORDER BY LASTTIME DESC
                ) AS rn
                FROM RISKMATRIX
                ) t 
                WHERE rn = 1
                ), CombinedData AS (
                ";

            // 2) CombinedData CTE: 기본정보 UNION ALL + LatestRisk JOIN
            var unionQueries = configs.Select(cfg => $@"
    SELECT
        '{cfg.Sort}'                         AS Sort,
        {cfg.Alias}.{cfg.CodeField}         AS Code,
        {cfg.Alias}.SERIAL_NO               AS Serial_No,
        '{cfg.EntityName}'                  AS Name,
        {cfg.Alias}.INSTALL_DATE            AS Install_Date,
        {cfg.Alias}.OPERATING_DATE          AS Operating_Date,
        DATEDIFF(YEAR, {cfg.Alias}.INSTALL_DATE, GETDATE()) AS UsagePeriod,
        {cfg.Alias}.PRICE                   AS Price,
        {cfg.Alias}.RATED_V                 AS Rated_V,
        {cfg.Alias}.RATED_A                 AS Rated_A,
        {cfg.Alias}.MAKE_COMPANY            AS Make_Company,
        {cfg.Alias}.WRITER                  AS Writer,
        lr.COF,
        lr.POF,
        lr.HI,
        CAST(lr.COF AS DECIMAL(18,2)) * CAST(lr.POF AS DECIMAL(18,2)) AS RiskScore
    FROM {cfg.Table} {cfg.Alias}
    LEFT JOIN LatestRisk lr
      ON {cfg.Alias}.{cfg.CodeField} = lr.CODE"
            ).ToList();

            var withNumbered = @"
), Numbered AS (
    SELECT
      *,
      ROW_NUMBER() OVER (
        PARTITION BY Code
        ORDER BY Install_Date DESC
      ) AS rn
    FROM CombinedData
)
SELECT
    ROW_NUMBER() OVER (
      ORDER BY RiskScore DESC,
               UsagePeriod DESC
    ) AS Priority,
    Sort,
    Code,
    Serial_No,
    Name,
    Install_Date,
    Operating_Date,
    UsagePeriod,
    Price,
    Rated_V,
    Rated_A,
    Make_Company,
    Writer,
    COF,
    POF,
    HI
FROM Numbered
WHERE rn = 1;
";

            // 전체 쿼리 조립
            var fullQuery = string.Concat(
                withLatestRisk,
                string.Join("\nUNION ALL\n", unionQueries),
                withNumbered
            );

            using (var dbHelper = new DBHelper())
            {
                return dbHelper.Conn
                    .Query<PriorityInfo>(fullQuery)
                    .AsList();
            }
        }
    }
}
