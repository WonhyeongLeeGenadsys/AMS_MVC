
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Web.Common;

namespace AMS_MVC
{
    public class LoginController : Controller
    {
        // GET: Login
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ValidateLogin(string userId, string password)
        {
            UserAccountRepository repo = new UserAccountRepository();
            UserAccount user = repo.GetUserById(userId);

            if(user != null && user.Pw == password)
            {
                Session["UserId"] = user.Id;
                Session["User_Name"] = user.User_Name;
                Session["User_Buseo"] = user.Buseo;

                string dbKey = "DefaultDB"; // 기본값 설정

                switch (userId.ToLower())
                {
                    case "test1":
                        dbKey = "DB_test1";
                        break;
                    case "test2":
                        dbKey = "DB_test2";
                        break;
                    case "test3":
                        dbKey = "DB_test3";
                        break;
                    case "test4":
                        dbKey = "DB_test4";
                        break;
                    case "mini": 
                        dbKey = "MiniDB";
                        break;
                }                
                Session["DBKey"] = dbKey;

                return Json(new { result = "success" }); 
            }
            return Json(new { result = "fail" });
        }

        // 로그아웃 
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();

            if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
            {
                HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, "");
                authCookie.Expires = DateTime.Now.AddYears(-1);
                Response.Cookies.Add(authCookie);
            }

            if (Request.Cookies["savedUserId"] != null)
            {
                HttpCookie savedIdCookie = new HttpCookie("savedUserId", "");
                savedIdCookie.Expires = DateTime.Now.AddYears(-1);
                Response.Cookies.Add(savedIdCookie);
            }

            return RedirectToAction("Index", "Login");
        }
    }
}