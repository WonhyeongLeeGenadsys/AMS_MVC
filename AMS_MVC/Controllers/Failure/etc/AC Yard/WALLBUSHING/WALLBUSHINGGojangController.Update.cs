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
    public partial class WALLBUSHINGGojangController : Controller
    {
        [HttpGet]
        public ActionResult WALLBUSHINGGojangUpdate(string wallbushingCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(wallbushingCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("WALLBUSHINGGojangTotalList");
            }

            // 고장 이력 조회
            var result = wallbushingGojangRepository.GetWALLBUSHINGFHDetailByWALLBUSHINGCode(wallbushingCode, tblIdx, out var wallbushingGojangList);

            if (!result.IsSuccess || wallbushingGojangList == null || !wallbushingGojangList.Any())
            {
                return HttpNotFound("WALLBUSHING 고장이력 정보를 찾을 수 없습니다.");
            }

            var detailRecord = wallbushingGojangList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 고장 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Gojang/etc/AC Yard/WALLBUSHING/WALLBUSHINGGojangUpdate.cshtml", detailRecord);
        }

        [HttpPost]
        public ActionResult WALLBUSHINGGojangUpdate(WALLBUSHINGFailureHistory model)
        {
            Result result = new Result(true);
            try
            {
                model.Fail_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";

                result = wallbushingGojangRepository.UpdateWALLBUSHINGFHRepo(model);
                if (!result.IsSuccess)
                {
                    result.Message = "WALLBUSHING 고장이력 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("WALLBUSHINGGojangUpdate Error", ex.Message);
            }

            return Json(result);
        }
    }
}
