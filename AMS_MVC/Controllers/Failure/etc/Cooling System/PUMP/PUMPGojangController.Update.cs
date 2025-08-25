
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class PUMPGojangController : Controller
    {
        [HttpGet]
        public ActionResult PUMPGojangUpdate(string pumpCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(pumpCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("PUMPGojangTotalList");
            }

            // 고장 이력 조회
            var result = pumpGojangRepository.GetPUMPFHDetailByPUMPCode(pumpCode, tblIdx, out var pumpGojangList);

            if (!result.IsSuccess || pumpGojangList == null || !pumpGojangList.Any())
            {
                return HttpNotFound("PUMP 고장이력 정보를 찾을 수 없습니다.");
            }

            var detailRecord = pumpGojangList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 고장 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Gojang/etc/Cooling System/PUMP/PUMPGojangUpdate.cshtml", detailRecord);
        }

        [HttpPost]
        public ActionResult PUMPGojangUpdate(PUMPFailureHistory model)
        {
            Result result = new Result(true);
            try
            {
                model.Fail_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";

                result = pumpGojangRepository.UpdatePUMPFHRepo(model);
                if (!result.IsSuccess)
                {
                    result.Message = "PUMP 고장이력 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("PUMPGojangUpdate Error", ex.Message);
            }

            return Json(result);
        }
    }
}
