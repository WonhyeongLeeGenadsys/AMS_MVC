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
    public partial class HEATEXCHANGERGojangController : Controller
    {
        [HttpGet]
        public ActionResult HEATEXCHANGERGojangUpdate(string heatexchangerCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(heatexchangerCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("HEATEXCHANGERGojangTotalList");
            }

            // 고장 이력 조회
            var result = heatexchangerGojangRepository.GetHEATEXCHANGERFHDetailByHEATEXCHANGERCode(heatexchangerCode, tblIdx, out var heatexchangerGojangList);

            if (!result.IsSuccess || heatexchangerGojangList == null || !heatexchangerGojangList.Any())
            {
                return HttpNotFound("HEATEXCHANGER 고장이력 정보를 찾을 수 없습니다.");
            }

            var detailRecord = heatexchangerGojangList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 고장 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Gojang/etc/Cooling System/HEATEXCHANGER/HEATEXCHANGERGojangUpdate.cshtml", detailRecord);
        }

        [HttpPost]
        public ActionResult HEATEXCHANGERGojangUpdate(HEATEXCHANGERFailureHistory model)
        {
            Result result = new Result(true);
            try
            {
                model.Fail_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";

                result = heatexchangerGojangRepository.UpdateHEATEXCHANGERFHRepo(model);
                if (!result.IsSuccess)
                {
                    result.Message = "HEATEXCHANGER 고장이력 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("HEATEXCHANGERGojangUpdate Error", ex.Message);
            }

            return Json(result);
        }
    }
}
