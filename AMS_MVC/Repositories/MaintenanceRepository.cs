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
        public List<dynamic> GetMonthlyMaintenanceVCBCounts()
        {
            using(DBHelper dbHelper = new DBHelper())
            {
                const string query = @"
                SELECT 
                    FORMAT(MR_DATE, 'yyyy-MM') AS Month, 
                    COUNT(*) AS Count,
                    'VCB' AS Type
                FROM VCB_MAINTENANCE_HISTORY
                WHERE MR_DATE IS NOT NULL
                GROUP BY FORMAT(MR_DATE, 'yyyy-MM');";
                return dbHelper.Conn.Query(query).ToList();
            }
        }

        public List<dynamic> GetMonthlyMaintenanceITRCounts()
        {
            using(DBHelper dbHelper = new DBHelper())
            {
                const string query = @"
                SELECT 
                    FORMAT(MR_DATE, 'yyyy-MM') AS Month, 
                    COUNT(*) AS Count,
                    'Interface TR' AS Type
                FROM INTERFACETR_MAINTENANCE_HISTORY
                WHERE MR_DATE IS NOT NULL
                GROUP BY FORMAT(MR_DATE, 'yyyy-MM');";
                return dbHelper.Conn.Query(query).ToList();
            }
        }

        public List<dynamic> GetMonthlyMaintenanceCounts()
        {
            using(DBHelper dbHelper = new DBHelper())
            {
                const string query = @"
                SELECT 
                    FORMAT(MR_DATE, 'yyyy-MM') AS Month, 
                    COUNT(*) AS Count,
                    'VCB' AS Type
                FROM VCB_MAINTENANCE_HISTORY
                WHERE MR_DATE IS NOT NULL
                GROUP BY FORMAT(MR_DATE, 'yyyy-MM')
                
                UNION ALL
                
                SELECT 
                    FORMAT(MR_DATE, 'yyyy-MM') AS Month, 
                    COUNT(*) AS Count,
                    'Interface TR' AS Type
                FROM INTERFACETR_MAINTENANCE_HISTORY
                WHERE MR_DATE IS NOT NULL
                GROUP BY FORMAT(MR_DATE, 'yyyy-MM')
                
                ORDER BY Month, Type;";
                return dbHelper.Conn.Query(query).ToList();
            }
        }
    }
}