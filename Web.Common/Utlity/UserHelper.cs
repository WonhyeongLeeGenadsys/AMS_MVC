using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Web.Common.Utlity
{
    public static class UserHelper
    {
        public static string GetUserName()
        {
            if (HttpContext.Current.Session["User_Name"] != null)
            {
                return HttpContext.Current.Session["User_Name"].ToString();
            }
            return string.Empty;
        }
    }
}
