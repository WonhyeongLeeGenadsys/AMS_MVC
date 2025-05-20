using AMS_MVC.Models;
using AMS_MVC.Utlity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers
{
    public partial class LAGojangController : Controller
    {
        [HttpGet]
        public ActionResult LAGojangUpdate(string laCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(laCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("LAGojangTotalList");
            }

            // 고장 이력 조회
            var result = laGojangRepository.GetLAFHDetailByLACode(laCode, tblIdx, out var laGojangList);

            if (!result.IsSuccess || laGojangList == null || !laGojangList.Any())
            {
                return HttpNotFound("LA 고장이력 정보를 찾을 수 없습니다.");
            }

            var detailRecord = laGojangList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 고장 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Gojang/etc/AC Yard/LA/LAGojangUpdate.cshtml", detailRecord);
        }

        [HttpPost]
        public ActionResult LAGojangUpdate(LAFailureHistory model)
        {
            Result result = new Result(true);
            try
            {
                model.Fail_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";

                result = laGojangRepository.UpdateLAFHRepo(model);
                if (!result.IsSuccess)
                {
                    result.Message = "LA 고장이력 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("LAGojangUpdate Error", ex.Message);
            }

            return Json(result);
        }
    }
}
