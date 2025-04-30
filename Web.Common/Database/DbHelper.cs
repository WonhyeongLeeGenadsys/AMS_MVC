using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace AMS_MVC.Database
{
    public class DBHelper:IDisposable
    {
        private SqlConnection mConn;
        public SqlConnection Conn { get { return mConn; } }
        public DBHelper()
        {
            //string connStr = ConfigurationManager.ConnectionStrings["MYDBConnectionString"].ConnectionString;
            //string connStr = ConfigurationManager.ConnectionStrings["MiniSetting"].ConnectionString;
            string connStr = ConfigurationManager.ConnectionStrings["AMS"].ConnectionString;

            mConn = new SqlConnection(connStr);
            mConn.Open();
        }
        public void Dispose()
        {
            if (mConn != null)
            {
                mConn.Close(); // 연결 닫기
                mConn.Dispose();
                mConn = null;
            }
        }
    }
}