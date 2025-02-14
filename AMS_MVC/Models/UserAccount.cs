using AMS_MVC.Database;
using AMS_MVC.Utlity;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Common.Log;

namespace AMS_MVC.Models
{
    public class UserAccount
    {
        public int Tbl_Idx { get; set; }
        public string Id { get; set; }
        public string Pw { get; set; }
        public string User_Name { get; set; }
        public int Permission { get; set; }
        public string Buseo { get; set; }
        public string Phone_No { get; set; }
        public DateTime Tbl_GetDate { get; set; }
    }    
}