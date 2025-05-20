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
    public partial class ZIGZAGTRGojangController : Controller
    {
        [HttpGet]
        public ActionResult ZIGZAGTRGojangUpdate(string zigzagtrCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(zigzagtrCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("ZIGZAGTRGojangTotalList");
            }

            // 고장 이력 조회
            var result = zigzagtrGojangRepository.GetZIGZAGTRFHDetailByZIGZAGTRCode(zigzagtrCode, tblIdx, out var zigzagtrGojangList);

            if (!result.IsSuccess || zigzagtrGojangList == null || !zigzagtrGojangList.Any())
            {
                return HttpNotFound("ZIGZAGTR 고장이력 정보를 찾을 수 없습니다.");
            }

            var detailRecord = zigzagtrGojangList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 고장 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Gojang/etc/Grounding/ZIGZAGTR/ZIGZAGTRGojangUpdate.cshtml", detailRecord);
        }

        [HttpPost]
        public ActionResult ZIGZAGTRGojangUpdate(ZIGZAGTRFailureHistory model)
        {
            Result result = new Result(true);
            try
            {
                model.Fail_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";

                result = zigzagtrGojangRepository.UpdateZIGZAGTRFHRepo(model);
                if (!result.IsSuccess)
                {
                    result.Message = "ZIGZAGTR 고장이력 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("ZIGZAGTRGojangUpdate Error", ex.Message);
            }

            return Json(result);
        }
    }
}
