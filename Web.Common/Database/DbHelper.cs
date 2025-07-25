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
            // ASP.NET 컨텍스트가 있으면 세션에서 DBKey 가져오고,
            // 없으면 "DefaultDB"를 사용하도록 변경
            string dbKey = "DefaultDB";
            if (HttpContext.Current?.Session != null
             && HttpContext.Current.Session["DBKey"] != null)
            {
                dbKey = HttpContext.Current.Session["DBKey"].ToString();
            }

            string connStr = ConfigurationManager
                .ConnectionStrings[dbKey]
                .ConnectionString;

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