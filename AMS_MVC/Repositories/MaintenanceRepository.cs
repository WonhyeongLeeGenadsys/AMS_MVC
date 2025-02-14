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
        private readonly DBHelper mDb;

        public MaintenanceRepository()
        {
            mDb = new DBHelper();
        }

        public List<dynamic> GetMonthlyMaintenanceVCBCounts()
        {
            const string query = @"
                SELECT 
                    FORMAT(MR_DATE, 'yyyy-MM') AS Month, 
                    COUNT(*) AS Count,
                    'VCB' AS Type
                FROM VCB_MAINTENANCE_HISTORY
                WHERE MR_DATE IS NOT NULL
                GROUP BY FORMAT(MR_DATE, 'yyyy-MM');";
            return mDb.Conn.Query(query).ToList();
        }

        public List<dynamic> GetMonthlyMaintenanceITRCounts()
        {
            const string query = @"
                SELECT 
                    FORMAT(MR_DATE, 'yyyy-MM') AS Month, 
                    COUNT(*) AS Count,
                    'Interface TR' AS Type
                FROM INTERFACETR_MAINTENANCE_HISTORY
                WHERE MR_DATE IS NOT NULL
                GROUP BY FORMAT(MR_DATE, 'yyyy-MM');";
            return mDb.Conn.Query(query).ToList();
        }

        public List<dynamic> GetMonthlyMaintenanceCounts()
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
            return mDb.Conn.Query(query).ToList();
        }
    }
}