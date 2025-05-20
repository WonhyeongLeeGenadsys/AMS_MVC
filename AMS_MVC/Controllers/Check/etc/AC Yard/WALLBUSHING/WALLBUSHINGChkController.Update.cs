using AMS_MVC.Models;
using AMS_MVC.Utlity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Check
{
    public partial class WALLBUSHINGChkController : Controller
    {
        public ActionResult WALLBUSHINGChkUpdate(string wallbushingCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(wallbushingCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("WALLBUSHINGChkTotalList");
            }

            var result = wallbushingChkRepository.GetWALLBUSHINGChkDetailByWALLBUSHINGCode(wallbushingCode, tblIdx, out var wallbushingChkList);

            if (!result.IsSuccess || wallbushingChkList == null || !wallbushingChkList.Any())
            {
                return HttpNotFound("WALLBUSHING 보통점검 정보를 찾을 수 없습니다.");
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

            var detailRecord = wallbushingChkList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 보통점검 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Check/etc/AC Yard/WALLBUSHING/WALLBUSHINGChkUpdate.cshtml", detailRecord); ;
        }

        [HttpPost]
        public ActionResult WALLBUSHINGChkUpdate(WALLBUSHINGChk model)
        {
            Result result = new Result(true);
            try
            {
                if (!result.IsSuccess)
                {
                    result.Message = "WALLBUSHING 보통점검 정보를 수정하지 못했습니다.";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("WALLBUSHINGChkUpdate Error", ex.Message);
            }

            var res = wallbushingChkRepository.UpdateWALLBUSHINGChkInfoRepo(model);
            return Json(new { success = res.IsSuccess, message = result.Message });
        }
    }
}