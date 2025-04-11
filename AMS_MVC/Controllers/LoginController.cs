using AMS_MVC.Models;
using AMS_MVC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

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
                Session["User_Buseo"] = user.Buseo;
                return Json(new { result = "success" }); // 로그인 성공
            }
            return Json(new { result = "fail" });
        }

        // 로그아웃 액션
        public ActionResult Logout()
        {
            // 세션의 모든 값을 제거합니다.
            Session.Clear();
            Session.Abandon();

            // 폼 인증 사용 시, 인증 쿠키를 제거할 수 있습니다.
            // (폼 인증을 사용하지 않으면 이 부분은 생략해도 됩니다.)
            if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
            {
                HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, "");
                authCookie.Expires = DateTime.Now.AddYears(-1);
                Response.Cookies.Add(authCookie);
            }

            // 추가로, 사용자 관련 쿠키가 있다면 제거하는 것도 고려합니다.
            // 예를 들어, "savedUserId" 쿠키 같은 경우도 삭제합니다.
            if (Request.Cookies["savedUserId"] != null)
            {
                HttpCookie savedIdCookie = new HttpCookie("savedUserId", "");
                savedIdCookie.Expires = DateTime.Now.AddYears(-1);
                Response.Cookies.Add(savedIdCookie);
            }

            // 로그아웃 후 로그인 페이지나 원하는 페이지로 리다이렉트
            return RedirectToAction("Index", "Login");
        }
    }
}