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
    public partial class PTGojangController : Controller
    {
        [HttpGet]
        public ActionResult PTGojangUpdate(string ptCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(ptCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("PTGojangTotalList");
            }

            // 고장 이력 조회
            var result = ptGojangRepository.GetPTFHDetailByPTCode(ptCode, tblIdx, out var ptGojangList);

            if (!result.IsSuccess || ptGojangList == null || !ptGojangList.Any())
            {
                return HttpNotFound("PT 고장이력 정보를 찾을 수 없습니다.");
            }

            var detailRecord = ptGojangList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 고장 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Gojang/etc/AC Yard/PT/PTGojangUpdate.cshtml", detailRecord);
        }

        [HttpPost]
        public ActionResult PTGojangUpdate(PTFailureHistory model)
        {
            Result result = new Result(true);
            try
            {
                model.Fail_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";

                result = ptGojangRepository.UpdatePTFHRepo(model);
                if (!result.IsSuccess)
                {
                    result.Message = "PT 고장이력 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("PTGojangUpdate Error", ex.Message);
            }

            return Json(result);
        }
    }
}
