using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace AMS_MVC.Database
{
    public class DBHelper
    {
        private SqlConnection mConn;
        public SqlConnection Conn { get { return mConn; } }
        public DBHelper()
        {
            string connStr = ConfigurationManager.ConnectionStrings["MYDBConnectionString"].ConnectionString;
            mConn = new SqlConnection(connStr);
            mConn.Open();
        }
    }
}