using AMS_MVC.Database;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS_MVC.Repositories
{
    public class MaintenanceRepository
    {
        public List<dynamic> GetMonthlyMaintenanceCounts(string maintenanceTable, string type)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                string query = $@"
            SELECT 
                FORMAT(MR_DATE, 'yyyy-MM') AS [Month], 
                COUNT(*) AS [Count],
                '{type}' AS [Type]
            FROM {maintenanceTable}
            WHERE MR_DATE IS NOT NULL
            GROUP BY FORMAT(MR_DATE, 'yyyy-MM');";
                return dbHelper.Conn.Query(query).ToList();
            }
        }


        public List<dynamic> GetMonthlyMaintenanceCounts()
        {
            var configs = new[]
            {
        new { Table = "VCB_MAINTENANCE_HISTORY",       Type = "VCB" },
        new { Table = "ITR_MAINTENANCE_HISTORY", Type = "Interface TR" },
        new { Table = "DCCB_MAINTENANCE_HISTORY",      Type = "DCCB" },
        new { Table = "DCCABLE_MAINTENANCE_HISTORY",   Type = "DC Cable" },
        new { Table = "SUBMODULE_MAINTENANCE_HISTORY", Type = "Sub Module" }
    };

            var unionQueries = configs.Select(cfg => $@"
        SELECT 
            FORMAT(MR_DATE, 'yyyy-MM') AS Month, 
            COUNT(*) AS Count,
            '{cfg.Type}' AS Type
        FROM {cfg.Table}
        WHERE MR_DATE IS NOT NULL
        GROUP BY FORMAT(MR_DATE, 'yyyy-MM')
    ").ToList();

            string fullQuery = $@"
        SELECT * FROM (
            {string.Join(" UNION ALL ", unionQueries)}
        ) T
        ORDER BY Month, Type;";

            using (DBHelper dbHelper = new DBHelper())
            {
                return dbHelper.Conn.Query(fullQuery).ToList();
            }
        }
    }
}