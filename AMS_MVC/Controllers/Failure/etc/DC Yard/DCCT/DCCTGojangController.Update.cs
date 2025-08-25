
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class DCCTGojangController : Controller
    {
        [HttpGet]
        public ActionResult DCCTGojangUpdate(string dcctCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(dcctCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("DCCTGojangTotalList");
            }

            // 고장 이력 조회
            var result = dcctGojangRepository.GetDCCTFHDetailByDCCTCode(dcctCode, tblIdx, out var dcctGojangList);

            if (!result.IsSuccess || dcctGojangList == null || !dcctGojangList.Any())
            {
                return HttpNotFound("DCCT 고장이력 정보를 찾을 수 없습니다.");
            }

            var detailRecord = dcctGojangList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 고장 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Gojang/etc/DC Yard/DCCT/DCCTGojangUpdate.cshtml", detailRecord);
        }

        [HttpPost]
        public ActionResult DCCTGojangUpdate(DCCTFailureHistory model)
        {
            Result result = new Result(true);
            try
            {
                model.Fail_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";

                result = dcctGojangRepository.UpdateDCCTFHRepo(model);
                if (!result.IsSuccess)
                {
                    result.Message = "DCCT 고장이력 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("DCCTGojangUpdate Error", ex.Message);
            }

            return Json(result);
        }
    }
}
