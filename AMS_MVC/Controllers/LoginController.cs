using AMS_MVC.Models;
using AMS_MVC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers
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
                return Json(new { result = "success" }); // 로그인 성공
            }

            return Json(new { result = "fail" });
        }
    }
}