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
    public partial class CTGojangController : Controller
    {
        [HttpGet]
        public ActionResult CTGojangUpdate(string ctCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(ctCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("CTGojangTotalList");
            }

            // 고장 이력 조회
            var result = ctGojangRepository.GetCTFHDetailByCTCode(ctCode, tblIdx, out var ctGojangList);

            if (!result.IsSuccess || ctGojangList == null || !ctGojangList.Any())
            {
                return HttpNotFound("CT 고장이력 정보를 찾을 수 없습니다.");
            }

            var detailRecord = ctGojangList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 고장 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Gojang/etc/AC Yard/CT/CTGojangUpdate.cshtml", detailRecord);
        }

        [HttpPost]
        public ActionResult CTGojangUpdate(CTFailureHistory model)
        {
            Result result = new Result(true);
            try
            {
                model.Fail_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";

                result = ctGojangRepository.UpdateCTFHRepo(model);
                if (!result.IsSuccess)
                {
                    result.Message = "CT 고장이력 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("CTGojangUpdate Error", ex.Message);
            }

            return Json(result);
        }
    }
}
