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
        private readonly DBHelper mDb;

        public PriorityInfoRepository()
        {
            mDb = new DBHelper();
        }

        public List<dynamic> GetPriorityVCB()
        {
            const string query = @"
        SELECT
            ROW_NUMBER() OVER (
                ORDER BY 
                    DATEDIFF(YEAR, v.INSTALL_DATE, GETDATE()) DESC,
                    v.INSTALL_DATE ASC 
            ) AS Priority, 
            'AC' AS Sort, 
            v.VCB_CODE AS Code, 
            v.SERIAL_NO AS Serial_No, 
            'VCB' AS Name, 
            v.INSTALL_DATE AS Install_Date, 
            v.OPERATING_DATE AS Operating_Date, 
            DATEDIFF(YEAR, v.INSTALL_DATE, GETDATE()) AS UsagePeriod, 
            v.PRICE AS Price, 
            v.RATED_V AS Rated_V, 
            v.RATED_A AS Rated_A,
            v.MAKE_COMPANY AS Make_Company,
            v.WRITER AS Writer,
            r.COF AS CoF,
            r.POF AS PoF, 
            CAST(r.COF AS INT) * CAST(r.POF AS INT) AS RiskScore 
        FROM VCB_BASICINFO v 
        LEFT JOIN RISKMATRIX r ON v.VCB_CODE = r.CODE";

            return mDb.Conn.Query(query).AsList();
        }
        public List<dynamic> GetPriorityITR()
        {
            const string query = @"
        SELECT
            ROW_NUMBER() OVER (
                ORDER BY 
                    DATEDIFF(YEAR, i.INSTALL_DATE, GETDATE()) DESC,
                    i.INSTALL_DATE ASC 
            ) AS Priority, 
                    'AC' AS Sort, 
                    i.ITR_CODE AS Code, 
                    i.SERIAL_NO AS Serial_No, 
                    'Interface TR' AS Name, 
                    i.INSTALL_DATE AS Install_Date, 
                    i.OPERATING_DATE AS Operating_Date, 
                    DATEDIFF(YEAR, i.INSTALL_DATE, GETDATE()) AS UsagePeriod, 
                    i.PRICE AS Price, 
                    i.RATED_V AS Rated_V, 
                    i.RATED_A AS Rated_A,
                    i.MAKE_COMPANY AS Make_Company,
                    i.WRITER AS Writer,
                    r.COF AS CoF, 
                    r.POF AS PoF, 
                    CAST(r.COF AS INT) * CAST(r.POF AS INT) AS RiskScore 
                FROM INTERFACETR_BASICINFO i LEFT JOIN RISKMATRIX r ON i.ITR_CODE = r.CODE";

            return mDb.Conn.Query(query).AsList();
        }
        public List<PriorityInfo> GetPriorityInfo()
        {
            const string query = @"
            WITH CombinedData AS (
                SELECT 
                    'AC' AS Sort, 
                    b.VCB_CODE AS Code, 
                    b.SERIAL_NO AS Serial_No, 
                    'VCB' AS Name, 
                    b.INSTALL_DATE AS Install_Date, 
                    b.OPERATING_DATE AS Operating_Date, 
                    DATEDIFF(YEAR, b.INSTALL_DATE, GETDATE()) AS UsagePeriod, 
                    b.PRICE AS Price, 
                    b.RATED_V AS Rated_V, 
                    b.RATED_A AS Rated_A,
                    b.MAKE_COMPANY AS Make_Company,
                    b.WRITER AS Writer,
                    r.COF AS CoF,
                    r.POF AS PoF, 
                    CAST(r.COF AS INT) * CAST(r.POF AS INT) AS RiskScore 
                FROM VCB_BASICINFO b LEFT JOIN RISKMATRIX r ON b.VCB_CODE = r.CODE
                UNION ALL
                SELECT 
                    'AC' AS Sort, 
                    i.ITR_CODE AS Code, 
                    i.SERIAL_NO AS Serial_No, 
                    'Interface TR' AS Name, 
                    i.INSTALL_DATE AS Install_Date, 
                    i.OPERATING_DATE AS Operating_Date, 
                    DATEDIFF(YEAR, i.INSTALL_DATE, GETDATE()) AS UsagePeriod, 
                    i.PRICE AS Price, 
                    i.RATED_V AS Rated_V, 
                    i.RATED_A AS Rated_A,
                    i.MAKE_COMPANY AS Make_Company,
                    i.WRITER AS Writer,
                    r.COF AS CoF, 
                    r.POF AS PoF, 
                    CAST(r.COF AS INT) * CAST(r.POF AS INT) AS RiskScore 
                FROM INTERFACETR_BASICINFO i LEFT JOIN RISKMATRIX r ON i.ITR_CODE = r.CODE
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
                PoF
            FROM CombinedData;";

            return mDb.Conn.Query<PriorityInfo>(query).AsList();
        }
    }
}
