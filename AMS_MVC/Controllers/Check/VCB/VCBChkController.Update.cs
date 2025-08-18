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
                model.CHK_Writer = Session["User_Name"]?.ToString() ?? "Anonymous";
                if (model.CHK_Tbl_GetDate < new DateTime(1753, 1, 1))
                    model.CHK_Tbl_GetDate = DateTime.Now;

                var scoreCalc = new VCBChkScoreCalculator();
                var (hi, pofRaw) = scoreCalc.CalculateHiPof(model, alpha: 0.99m);
                model.FoldingFunction = (int)Math.Round(hi);

                var upd = vcbChkRepository.UpdateVCBChkInfoRepo(model);
                if (!upd.IsSuccess)
                {
                    result.IsSuccess = false;
                    result.Message = "VCB 보통점검 정보를 수정하지 못했습니다: " + upd.Message;
                }
                else
                {
                    var cofModel = cofRepo.GetLatest(model.VCB_Code) ?? cofRepo.GetLatest("VCB");
                    decimal baseCof = cofModel?.Total_Cof ?? 0m;

                    decimal pofPercent = (pofRaw <= 1m) ? pofRaw * 100m : pofRaw;
                    if (pofPercent < 0m) pofPercent = 0m;
                    if (pofPercent > 100m) pofPercent = 100m;

                    decimal adjustedCof = Math.Round(baseCof * (pofPercent / 100m), 2);

                    var rm = riskMatrixRepository.UpdateRiskMatrixHI(
                        model.VCB_Code,
                        (int)Math.Round(hi),
                        adjustedCof,
                        pofPercent
                    );

                    LogHelper.WriteLog("VCBChkUpdate",
                        $"[UpdateRiskMatrixHI] code={model.VCB_Code}, hi={(int)Math.Round(hi)}, baseCof={baseCof}, pof%={pofPercent}, adjustedCof={adjustedCof}, ok={rm.IsSuccess}");

                    if (!rm.IsSuccess)
                    {
                        result.IsSuccess = false;
                        result.Message = "수정은 성공했으나, RiskMatrix 업데이트 실패: " + rm.Message;
                    }
                    else
                    {
                        result.Message = "수정 및 RiskMatrix 반영이 완료되었습니다.";
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