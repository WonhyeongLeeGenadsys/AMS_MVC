using System.Linq;
using System.Web.Mvc;
using AMS_MVC.Models;

namespace AMS_MVC.Controllers
{
    public class UserAccountController : Controller
    {
        private UserAccountRepository repo = new UserAccountRepository();

        public ActionResult GetUserById(string id)
        {
            var user = repo.GetUserById(id);

            return View(user);
        }

        public ActionResult GetAllUsers()
        {
            var result = repo.GetAllUsers(out var users);
            return View(users);
        }

        [HttpPost]
        public ActionResult CreateUser(UserAccount newUser)
        {
            if (ModelState.IsValid)
            {
                var result = repo.CreateUser(newUser);
                if (result.IsSuccess)
                {
                    return RedirectToAction("GetAllUsers");
                }
                else
                {
                    ViewBag.ErrorMessage = result.Message;
                }
            }
            return View(newUser);
        }

        [HttpPost]
        public ActionResult UpdateUser(UserAccount updatedUser)
        {
            if (ModelState.IsValid)
            {
                var result = repo.UpdateUser(updatedUser);
                if (result.IsSuccess)
                {
                    return RedirectToAction("GetAllUsers");
                }
                else
                {
                    ViewBag.ErrorMessage = result.Message;
                }
            }
            return View(updatedUser);
        }

        [HttpPost]
        public ActionResult DeleteUser(string id)
        {
            var result = repo.DeleteUser(id);
            if (result.IsSuccess)
            {
                return RedirectToAction("GetAllUsers");
            }
            else
            {
                ViewBag.ErrorMessage = result.Message;
            }
            return RedirectToAction("GetAllUsers");
        }
    }
}
