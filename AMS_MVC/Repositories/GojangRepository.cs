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
        private readonly DBHelper mDb;

        public GojangRepository()
        {
            mDb = new DBHelper();
        }
        public List<dynamic> GetGojangVCB()
        {
            const string query = @"
    SELECT
        ROW_NUMBER() OVER (ORDER BY CAST(F.FAIL_PERIOD AS INT) DESC) AS Priority,
        V.VCB_CODE AS Code,
        'VCB' AS Name,
        V.SERIAL_NO AS Serial_No,
        F.FAIL_WEATHER AS Weather,
        F.FAIL_TEMP AS Temp,
        F.FAIL_HUM AS Hum,
        F.FAIL_REASON AS Reason,
        F.FAIL_STATUS AS Status,
        F.FAIL_PART AS Part,
        F.FAIL_PERIOD AS Period,
        F.FAIL_FINDER AS Finder,
        F.FAIL_REGISTRAR AS Registrar,
        F.FAIL_DATE AS Date
    FROM VCB_FAILURE_HISTORY F
    LEFT JOIN VCB_BASICINFO V ON F.VCB_CODE = V.VCB_CODE";

            return mDb.Conn.Query(query).AsList();
        }

        public List<dynamic> GetGojangITR()
        {
            const string query = @"
    SELECT
        ROW_NUMBER() OVER (ORDER BY CAST(F.FAIL_PERIOD AS INT) DESC) AS Priority,
        I.ITR_CODE AS Code,
        'Interface TR' AS Name,
        I.SERIAL_NO AS Serial_No,
        F.FAIL_WEATHER AS Weather,
        F.FAIL_TEMP AS Temp,
        F.FAIL_HUM AS Hum,
        F.FAIL_REASON AS Reason,
        F.FAIL_STATUS AS Status,
        F.FAIL_PART AS Part,
        F.FAIL_PERIOD AS Period,
        F.FAIL_FINDER AS Finder, 
        F.FAIL_REGISTRAR AS Registrar,
        F.FAIL_DATE AS Date
    FROM INTERFACETR_FAILURE_HISTORY F LEFT JOIN INTERFACETR_BASICINFO I ON F.ITR_CODE = I.ITR_CODE";

            return mDb.Conn.Query(query).AsList();
        }

        public List<dynamic> GetGojangAll()
        {
            const string query = @"
WITH CombinedData AS (
    SELECT
        V.VCB_CODE AS Code,
        'VCB' AS Name,
        V.SERIAL_NO AS Serial_No,
        F.FAIL_WEATHER AS Weather,
        F.FAIL_TEMP AS Temp,
        F.FAIL_HUM AS Hum,
        F.FAIL_REASON AS Reason,
        F.FAIL_STATUS AS Status,
        F.FAIL_PART AS Part,
        F.FAIL_PERIOD AS Period,
        F.FAIL_FINDER AS Finder,
        F.FAIL_REGISTRAR AS Registrar,
        F.FAIL_DATE AS Date
    FROM VCB_FAILURE_HISTORY F  LEFT JOIN VCB_BASICINFO V ON F.VCB_CODE = V.VCB_CODE

    UNION ALL

    SELECT
        I.ITR_CODE AS Code,
        'Interface TR' AS Name,
        I.SERIAL_NO AS Serial_No,
        F.FAIL_WEATHER AS Weather,
        F.FAIL_TEMP AS Temp,
        F.FAIL_HUM AS Hum,
        F.FAIL_REASON AS Reason,
        F.FAIL_STATUS AS Status,
        F.FAIL_PART AS Part,
        F.FAIL_PERIOD AS Period,
        F.FAIL_FINDER AS Finder, 
        F.FAIL_REGISTRAR AS Registrar,
        F.FAIL_DATE AS Date
    FROM INTERFACETR_FAILURE_HISTORY F LEFT JOIN INTERFACETR_BASICINFO I ON F.ITR_CODE = I.ITR_CODE
)
SELECT
    ROW_NUMBER() OVER (ORDER BY DATEDIFF(DAY, Date, GETDATE()) DESC) AS Priority,
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
    Registrar,
    Date
FROM CombinedData;";

            return mDb.Conn.Query(query).AsList();

        }
    }
}