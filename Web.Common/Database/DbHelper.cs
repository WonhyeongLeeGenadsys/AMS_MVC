using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Web.Common
{
    public class DBHelper : IDisposable
    {
        private SqlConnection mConn;
        public SqlConnection Conn => mConn;

        public DBHelper()
        {
            // 무조건 DefaultDB만 사용
            string connStr = ConfigurationManager
                .ConnectionStrings["DefaultDB"]
                .ConnectionString;

            mConn = new SqlConnection(connStr);
            mConn.Open();
        }

        public void Dispose()
        {
            if (mConn != null)
            {
                mConn.Close();
                mConn.Dispose();
                mConn = null;
            }
        }
    }
}