using AMS_MVC.Database;
using AMS_MVC.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS_MVC.Repositories
{
    public class PriorityInfoRepository
    {
        public List<dynamic> GetPriority(string basicInfoTable, string codeField, string nameValue, string alias)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                string query = $@"
        SELECT
            ROW_NUMBER() OVER (
                ORDER BY 
                    DATEDIFF(YEAR, {alias}.INSTALL_DATE, GETDATE()) DESC,
                    {alias}.INSTALL_DATE ASC 
            ) AS Priority, 
            'AC' AS Sort, 
            {alias}.{codeField} AS Code, 
            {alias}.SERIAL_NO AS Serial_No, 
            '{nameValue}' AS Name, 
            {alias}.INSTALL_DATE AS Install_Date, 
            {alias}.OPERATING_DATE AS Operating_Date, 
            DATEDIFF(YEAR, {alias}.INSTALL_DATE, GETDATE()) AS UsagePeriod, 
            {alias}.PRICE AS Price, 
            {alias}.RATED_V AS Rated_V, 
            {alias}.RATED_A AS Rated_A,
            {alias}.MAKE_COMPANY AS Make_Company,
            {alias}.WRITER AS Writer,
            r.COF AS CoF,
            r.POF AS PoF, 
            CAST(r.COF AS INT) * CAST(r.POF AS INT) AS RiskScore,
            r.HI AS HI
        FROM {basicInfoTable} {alias}
        LEFT JOIN RISKMATRIX r ON {alias}.{codeField} = r.CODE";

                return dbHelper.Conn.Query(query).AsList();
            }
        }
        public List<PriorityInfo> GetPriorityInfo()
        {
            // 각 항목에 대한 구성 배열
            var configs = new[]
            {
        new { BasicTable = "VCB_BASICINFO", CodeField = "VCB_CODE", Alias = "b", EntityName = "VCB" },
        new { BasicTable = "ITR_BASICINFO", CodeField = "ITR_CODE", Alias = "i", EntityName = "ITR" },
        new { BasicTable = "DCCB_BASICINFO", CodeField = "DCCB_CODE", Alias = "d", EntityName = "DCCB" },
        new { BasicTable = "DCCABLE_BASICINFO", CodeField = "DCCABLE_CODE", Alias = "c", EntityName = "DCCABLE" },
        new { BasicTable = "SUBMODULE_BASICINFO", CodeField = "SUBMODULE_CODE", Alias = "s", EntityName = "SUBMODULE" }
    };

            var unionQueries = new List<string>();

            foreach (var cfg in configs)
            {
                // 각 항목은 동일한 필드를 반환함
                string part = $@"
    SELECT 
        'AC' AS Sort, 
        {cfg.Alias}.{cfg.CodeField} AS Code, 
        {cfg.Alias}.SERIAL_NO AS Serial_No, 
        '{cfg.EntityName}' AS Name, 
        {cfg.Alias}.INSTALL_DATE AS Install_Date, 
        {cfg.Alias}.OPERATING_DATE AS Operating_Date, 
        DATEDIFF(YEAR, {cfg.Alias}.INSTALL_DATE, GETDATE()) AS UsagePeriod, 
        {cfg.Alias}.PRICE AS Price, 
        {cfg.Alias}.RATED_V AS Rated_V, 
        {cfg.Alias}.RATED_A AS Rated_A,
        {cfg.Alias}.MAKE_COMPANY AS Make_Company,
        {cfg.Alias}.WRITER AS Writer,
        r.COF AS CoF,
        r.POF AS PoF, 
        r.HI AS HI,
        CAST(r.COF AS INT) * CAST(r.POF AS INT) AS RiskScore
    FROM {cfg.BasicTable} {cfg.Alias}
    LEFT JOIN RISKMATRIX r ON {cfg.Alias}.{cfg.CodeField} = r.CODE";
                unionQueries.Add(part);
            }

            string fullQuery = @"
WITH CombinedData AS (
" + string.Join(" UNION ALL ", unionQueries) + @"
)
SELECT 
    ROW_NUMBER() OVER (ORDER BY RiskScore DESC, UsagePeriod DESC) AS Priority,
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
    CoF,
    PoF,
    HI
FROM CombinedData;";

            using (DBHelper dbHelper = new DBHelper())
            {
                return dbHelper.Conn.Query<PriorityInfo>(fullQuery).AsList();
            }
        }

    }
}
