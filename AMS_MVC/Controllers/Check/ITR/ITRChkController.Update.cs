// Controllers/Check/ITRChkController.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AMS_MVC.Models;
using AMS_MVC.Services;
using AMS_MVC.Utlity;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Check
{
    public partial class ITRChkController : Controller
    {
        public ActionResult ITRChk1Update(string itrCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(itrCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("ITRChkTotalList");
            }

            var result = _chk1Repo.GetITRChk1DetailByITRCode(itrCode, tblIdx, out var itrChkList);

            if (!result.IsSuccess || itrChkList == null || !itrChkList.Any())
            {
                return HttpNotFound("ITR 보통점검 정보를 찾을 수 없습니다.");
            }

            var companies = new List<Company>();
            if (_companyRepo.GetAllCompanies(out companies).IsSuccess && companies != null)
            {
                ViewBag.Companies = companies;
            }
            else
            {
                ViewBag.ErrorMessage = "제작사 정보를 불러올 수 없습니다.";
            }

            var detailRecord = itrChkList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 보통점검 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Check/ITR/ITRChk1Update.cshtml", detailRecord); ;
        }

        [HttpPost]
        public ActionResult ITRChk1Update(ITRChk1 model)
        {
            var result = new Result(true);
            try
            {
                // 1) 작성자, 날짜 처리
                model.CHK1_Writer = Session["User_Name"]?.ToString() ?? "Anonymous";
                if (model.CHK1_Tbl_GetDate < new DateTime(1753, 1, 1))
                    model.CHK1_Tbl_GetDate = DateTime.Now;

                // 2) FoldingFunction 재계산
                //model.FoldingFunction = _scoreCalc.CalculateFoldingFunction(model);

                var scoreCalc = new ITRChkScoreCalculator();
                var (hi, pof) = scoreCalc.CalculateHiPof(model, alpha: 0.99m);
                model.FoldingFunction = (int)Math.Round(hi);

                // 3) DB 수정
                var upd = _chk1Repo.UpdateITRChk1InfoRepo(model);
                if (!upd.IsSuccess)
                {
                    result.IsSuccess = false;
                    result.Message = "ITR 보통점검 정보 수정 실패: " + upd.Message;
                }
                else
                {
                    // 4) 반대 검사(정밀) 최신 점수 조회
                    var other = _chk2Repo.GetLatestFoldingFunction(model.ITR_Code);
                    int hi2 = other.HasValue
                        ? Math.Max(model.FoldingFunction, other.Value)
                        : model.FoldingFunction;

                    // 5) RiskMatrix HI 업데이트
                    var riskUpd = _riskRepo.UpdateRiskMatrixHI(model.ITR_Code, hi2, pof);
                    if (!riskUpd.IsSuccess)
                    {
                        result.IsSuccess = false;
                        result.Message = "수정은 성공했으나 RiskMatrix HI 갱신에 실패했습니다: " + riskUpd.Message;
                    }
                    else
                    {
                        result.Message = "ITR 보통점검 수정 및 RiskMatrix HI 갱신이 완료되었습니다.";
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "오류 발생: " + ex.Message;
                LogHelper.WriteLog("ITRChk1Update Error", ex.ToString());
            }

            return Json(new { success = result.IsSuccess, message = result.Message });
        }

        public ActionResult ITRChk2Update(string itrCode, string tblIdx)
        {
            // 필수 파라미터 검증
            if (string.IsNullOrEmpty(itrCode) || string.IsNullOrEmpty(tblIdx))
            {
                return RedirectToAction("ITRChk2TotalList");
            }

            var result = _chk2Repo.GetITRChk2DetailByITRCode(itrCode, tblIdx, out var itrChkList);

            if (!result.IsSuccess || itrChkList == null || !itrChkList.Any())
            {
                return HttpNotFound("ITR 정밀점검 정보를 찾을 수 없습니다.");
            }

            var companies = new List<Company>();
            if (_companyRepo.GetAllCompanies(out companies).IsSuccess && companies != null)
            {
                ViewBag.Companies = companies;
            }
            else
            {
                ViewBag.ErrorMessage = "제작사 정보를 불러올 수 없습니다.";
            }

            var detailRecord = itrChkList.FirstOrDefault(r => r.Tbl_Idx.ToString() == tblIdx);
            if (detailRecord == null)
            {
                return HttpNotFound("해당 정밀점검 정보를 찾을 수 없습니다.");
            }

            return View("~/Views/Check/ITR/ITRChk2Update.cshtml", detailRecord); ;
        }

        [HttpPost]
        public ActionResult ITRChk2Update(ITRChk2 model)
        {
            var result = new Result(true);
            try
            {
                // 1) 작성자, 날짜 처리
                model.CHK2_Writer = Session["User_Name"]?.ToString() ?? "Anonymous";
                if (model.CHK2_Tbl_GetDate < new DateTime(1753, 1, 1))
                    model.CHK2_Tbl_GetDate = DateTime.Now;

                // 2) FoldingFunction 재계산
                //model.FoldingFunction = _scoreCalc.CalculateFoldingFunction(model);

                var scoreCalc = new ITRChkScoreCalculator();
                var (hi, pof) = scoreCalc.CalculateHiPof(model, alpha: 0.99m);
                model.FoldingFunction = (int)Math.Round(hi);

                // 3) DB 수정
                var upd = _chk2Repo.UpdateITRChk2InfoRepo(model);
                if (!upd.IsSuccess)
                {
                    result.IsSuccess = false;
                    result.Message = "ITR 정밀점검 정보 수정 실패: " + upd.Message;
                }
                else
                {
                    // 4) 반대 검사(보통) 최신 점수 조회
                    var other = _chk1Repo.GetLatestFoldingFunction(model.ITR_Code);
                    int hi1 = other.HasValue
                        ? Math.Max(model.FoldingFunction, other.Value)
                        : model.FoldingFunction;

                    // 5) RiskMatrix HI 업데이트
                    var riskUpd = _riskRepo.UpdateRiskMatrixHI(model.ITR_Code, hi1, pof);
                    if (!riskUpd.IsSuccess)
                    {
                        result.IsSuccess = false;
                        result.Message = "수정은 성공했으나 RiskMatrix HI 갱신에 실패했습니다: " + riskUpd.Message;
                    }
                    else
                    {
                        result.Message = "ITR 정밀점검 수정 및 RiskMatrix HI 갱신이 완료되었습니다.";
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "오류 발생: " + ex.Message;
                LogHelper.WriteLog("ITRChk2Update Error", ex.ToString());
            }

            return Json(new { success = result.IsSuccess, message = result.Message });
        }
    }
}