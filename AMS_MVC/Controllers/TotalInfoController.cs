using AMS_MVC.Models;
using AMS_MVC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers
{
    public class TotalInfoController : Controller
    {
        // GET: TotalInfo/Index
        public ActionResult Index()
        {
            ViewBag.Title = "종합정보";
            return View();
        }

        [HttpPost]
        public JsonResult GetRiskmatrixData(string prefix)
        {
            try
            {
                var repository = new RiskmatrixRepository();
                var riskData = repository.GetMatrixByCodePrefix(prefix);
                return Json(riskData);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetRiskMatrixPofCof()
        {
            try
            {
                var repository = new RiskmatrixRepository();
                var pofCof = repository.GetRiskMatrixPofCof();
                return Json(pofCof);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetPriorityInfo()
        {
            try
            {
                var priorityRepo = new PriorityInfoRepository();
                var priorityData = priorityRepo.GetPriorityInfo();
                return Json(priorityData);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetMonthlyMaintenanceData()
        {
            try
            {
                var repo = new MaintenanceRepository();
                var data = repo.GetMonthlyMaintenanceCounts();
                return Json(data);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("TotalInfoController", $"GetMonthlyMaintenanceData Error: {ex.Message}");
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetGojangList()
        {
            try
            {
                var gojangRepo = new GojangRepository();
                var gojangData = gojangRepo.GetGojangAll();
                return Json(gojangData);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("TotalInfoController", $"GetGojangList Error: {ex.Message}");
                return Json(new { error = "데이터를 가져오는 중 오류 발생", details = ex.Message });
            }
        }

        //[HttpPost]
        //public JsonResult PageLoad()
        //{
        //    try
        //    {
        //        LogHelper.WriteLog("TotalInfoController", "PageLoad 실행");
        //        var userRepository = new UserRepository();
        //        List<UserAccount> users;
        //        var res = userRepository.GetAllUsers(out users);

        //        if (!res.IsSuccess)
        //        {
        //            LogHelper.WriteLog("TotalInfoController", res.Message);
        //            return Json(new { error = res.Message });
        //        }
        //        return Json(users);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { error = ex.Message });
        //    }
        //}

        //[HttpPost]
        //public JsonResult CreateUser(UserAccount user)
        //{
        //    try
        //    {
        //        LogHelper.WriteLog("TotalInfoController", $"CreateUser ID: {user.Id}");
        //        var userRepository = new UserRepository();
        //        userRepository.CreateUser(user);
        //        return Json(new { message = "생성 성공" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { error = ex.Message });
        //    }
        //}

        //[HttpPost]
        //public JsonResult UpdateUser(UserAccount user)
        //{
        //    try
        //    {
        //        LogHelper.WriteLog("TotalInfoController", $"UpdateUser ID: {user.Id}");
        //        var userRepository = new UserRepository();
        //        userRepository.UpdateUser(user);
        //        return Json(new { message = "수정 성공" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { error = ex.Message });
        //    }
        //}

        //[HttpPost]
        //public JsonResult DeleteUser(string userId)
        //{
        //    try
        //    {
        //        LogHelper.WriteLog("TotalInfoController", $"DeleteUser ID: {userId}");
        //        var userRepository = new UserRepository();
        //        userRepository.DeleteUser(userId);
        //        return Json(new { message = "삭제 성공" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { error = ex.Message });
        //    }
        //}

        public ActionResult Battery()
        {
            return View();
        }

        public ActionResult Check()
        {
            return View();
        }

        public ActionResult Connection()
        {
            return View();
        }
    }
}