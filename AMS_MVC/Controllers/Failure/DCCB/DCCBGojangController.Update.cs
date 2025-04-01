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
    public partial class DCCBGojangController : Controller
    {
        [HttpGet]
        public ActionResult DCCBGojangUpdate(string dccbCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(dccbCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("DCCBGojangTotalList");
            }

            // 고장 이력 조회
            var result = dccbGojangRepository.GetDCCBFHDetailByDCCBCode(dccbCode, tblIdx, out var dccbGojangList);

            if (!result.IsSuccess || dccbGojangList == null || !dccbGojangList.Any())
            {
                return HttpNotFound("DCCB 고장이력 정보를 찾을 수 없습니다.");
            }

            var detailRecord = dccbGojangList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 고장 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Gojang/DCCB/DCCBGojangUpdate.cshtml", detailRecord);
        }

        [HttpPost]
        public ActionResult DCCBGojangUpdate(DCCBFailureHistory model)
        {
            Result result = new Result(true);
            try
            {
                model.Fail_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";

                result = dccbGojangRepository.UpdateDCCBFHRepo(model);
                if (!result.IsSuccess)
                {
                    result.Message = "DCCB 고장이력 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("DCCBGojangUpdate Error", ex.Message);
            }

            return Json(result);
        }
    }
}
