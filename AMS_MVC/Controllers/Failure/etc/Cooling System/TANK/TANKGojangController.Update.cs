
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class TANKGojangController : Controller
    {
        [HttpGet]
        public ActionResult TANKGojangUpdate(string tankCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(tankCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("TANKGojangTotalList");
            }

            // 고장 이력 조회
            var result = tankGojangRepository.GetTANKFHDetailByTANKCode(tankCode, tblIdx, out var tankGojangList);

            if (!result.IsSuccess || tankGojangList == null || !tankGojangList.Any())
            {
                return HttpNotFound("TANK 고장이력 정보를 찾을 수 없습니다.");
            }

            var detailRecord = tankGojangList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 고장 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Gojang/etc/Cooling System/TANK/TANKGojangUpdate.cshtml", detailRecord);
        }

        [HttpPost]
        public ActionResult TANKGojangUpdate(TANKFailureHistory model)
        {
            Result result = new Result(true);
            try
            {
                model.Fail_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";

                result = tankGojangRepository.UpdateTANKFHRepo(model);
                if (!result.IsSuccess)
                {
                    result.Message = "TANK 고장이력 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("TANKGojangUpdate Error", ex.Message);
            }

            return Json(result);
        }
    }
}
