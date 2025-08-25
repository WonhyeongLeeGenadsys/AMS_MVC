
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class ZIGZAGTRChkController : Controller
    {
        public ActionResult ZIGZAGTRChkUpdate(string zigzagtrCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(zigzagtrCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("ZIGZAGTRChkTotalList");
            }

            var result = zigzagtrChkRepository.GetZIGZAGTRChkDetailByZIGZAGTRCode(zigzagtrCode, tblIdx, out var zigzagtrChkList);

            if (!result.IsSuccess || zigzagtrChkList == null || !zigzagtrChkList.Any())
            {
                return HttpNotFound("ZIGZAGTR 보통점검 정보를 찾을 수 없습니다.");
            }

            var companies = new List<Company>();
            if (companyRepository.GetAllCompanies(out companies).IsSuccess && companies != null)
            {
                ViewBag.Companies = companies;
            }
            else
            {
                ViewBag.ErrorMessage = "제작사 정보를 불러올 수 없습니다.";
            }

            var detailRecord = zigzagtrChkList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 보통점검 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Check/etc/Grounding/ZIGZAGTR/ZIGZAGTRChkUpdate.cshtml", detailRecord); ;
        }

        [HttpPost]
        public ActionResult ZIGZAGTRChkUpdate(ZIGZAGTRChk model)
        {
            Result result = new Result(true);
            try
            {
                if (!result.IsSuccess)
                {
                    result.Message = "ZIGZAGTR 보통점검 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("ZIGZAGTRChkUpdate Error", ex.Message);
            }

            var res = zigzagtrChkRepository.UpdateZIGZAGTRChkInfoRepo(model);
            return Json(new { success = res.IsSuccess, message = result.Message });
        }
    }
}