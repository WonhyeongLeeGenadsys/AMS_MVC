using AMS_MVC.Models;
using AMS_MVC.Services;
using AMS_MVC.Utlity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Check
{
    public partial class VCBChkController : Controller
    {
        public ActionResult VCBChkUpdate(string vcbCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(vcbCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("VCBChkTotalList");
            }

            var result = vcbChkRepository.GetVCBChkDetailByVCBCode(vcbCode, tblIdx, out var vcbChkList);

            if (!result.IsSuccess || vcbChkList == null || !vcbChkList.Any())
            {
                return HttpNotFound("VCB 보통점검 정보를 찾을 수 없습니다.");
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

            var detailRecord = vcbChkList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 보통점검 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Check/VCB/VCBChkUpdate.cshtml", detailRecord); ;
        }

        [HttpPost]
        public ActionResult VCBChkUpdate(VCBChk model)
        {
            var result = new Result(true);
            try
            {
                // 1) 작성자, 날짜 처리 (필요하다면)
                model.CHK_Writer = Session["User_Name"]?.ToString() ?? "Anonymous";
                if (model.CHK_Tbl_GetDate < new DateTime(1753, 1, 1))
                    model.CHK_Tbl_GetDate = DateTime.Now;

                // 2) FoldingFunction 재계산
                var scoreCalc = new VCBChkScoreCalculator();
                model.FoldingFunction = scoreCalc.CalculateFoldingFunction(model);

                // 3) VCBChk 테이블 UPDATE
                var upd = vcbChkRepository.UpdateVCBChkInfoRepo(model);
                if (!upd.IsSuccess)
                {
                    result.IsSuccess = false;
                    result.Message = "VCB 보통점검 정보를 수정하지 못했습니다: " + upd.Message;
                }
                else
                {
                    // 4) RiskMatrix HI 업데이트
                    var riskUpd = riskMatrixRepository.UpdateRiskMatrixHI(model.VCB_Code, model.FoldingFunction);
                    if (!riskUpd.IsSuccess)
                    {
                        result.IsSuccess = false;
                        result.Message = "수정은 성공했으나, RiskMatrix HI 업데이트에 실패했습니다: " + riskUpd.Message;
                    }
                    else
                    {
                        result.Message = "수정 및 RiskMatrix HI 반영이 완료되었습니다.";
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("VCBChkUpdate Error", ex.ToString());
            }

            return Json(result);
        }

    }
}