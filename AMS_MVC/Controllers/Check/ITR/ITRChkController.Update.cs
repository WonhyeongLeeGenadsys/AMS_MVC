// Controllers/Check/ITRChkController.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
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
                var (hi, pof) = scoreCalc.CalculateHiPof(model, alpha: 1.00m);
                model.FoldingFunction = (int)Math.Truncate(hi);

                // 3) DB 수정
                var upd = _chk1Repo.UpdateITRChk1InfoRepo(model);
                if (!upd.IsSuccess)
                {
                    result.IsSuccess = false;
                    result.Message = "ITR 보통점검 정보 수정 실패: " + upd.Message;
                }
                else
                {
                    // 4) 최신 정밀점검과 합산하여 최종 HI/PoF 계산
                    _chk2Repo.GetLatestITRChk2ByITRCode(model.ITR_Code, out var list2);
                    var latest2 = list2?.OrderBy(x => x.Tbl_Idx).LastOrDefault();

                    decimal hiFinalDec, pofFinal;
                    if (latest2 != null)
                        (hiFinalDec, pofFinal) = scoreCalc.CalculateHiPofCombined(model, latest2, 1.00m);
                    else
                        (hiFinalDec, pofFinal) = (hi, pof);

                    int hiFinal = (int)Math.Truncate(hiFinalDec);
                    pofFinal = Math.Min(100m, Math.Max(0m, pofFinal));

                    var cofModel = cofRepo.GetLatest("ITR");
                    decimal baseCof = cofModel?.Total_Cof ?? 0m;

                    // 5) RiskMatrix에는 기본 CoF와 최종 PoF를 각각 저장
                    var riskUpd = _riskRepo.UpdateRiskMatrixHI(
                        model.ITR_Code,
                        hiFinal,
                        baseCof,
                        pofFinal);

                    if (!riskUpd.IsSuccess)
                    {
                        result.IsSuccess = false;
                        result.Message = "수정은 성공했으나 RiskMatrix 갱신에 실패했습니다: " + riskUpd.Message;
                    }
                    else
                    {
                        result.Message = "ITR 보통점검 수정 및 RiskMatrix 반영이 완료되었습니다.";
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
                var (hi, pof) = scoreCalc.CalculateHiPof(model, alpha: 1.00m);
                model.FoldingFunction = (int)Math.Truncate(hi);

                // 3) DB 수정
                var upd = _chk2Repo.UpdateITRChk2InfoRepo(model);
                if (!upd.IsSuccess)
                {
                    result.IsSuccess = false;
                    result.Message = "ITR 정밀점검 정보 수정 실패: " + upd.Message;
                }
                else
                {
                    // 4) 최신 보통점검과 합산하여 최종 HI/PoF 계산
                    _chk1Repo.GetLatestITRChk1ByITRCode(model.ITR_Code, out var list1);
                    var latest1 = list1?.OrderBy(x => x.Tbl_Idx).LastOrDefault();

                    decimal hiFinalDec, pofFinal;
                    if (latest1 != null)
                        (hiFinalDec, pofFinal) = scoreCalc.CalculateHiPofCombined(latest1, model, 1.00m);
                    else
                        (hiFinalDec, pofFinal) = (hi, pof);

                    int hiFinal = (int)Math.Truncate(hiFinalDec);
                    pofFinal = Math.Min(100m, Math.Max(0m, pofFinal));

                    var cofModel = cofRepo.GetLatest("ITR");
                    decimal baseCof = cofModel?.Total_Cof ?? 0m;

                    // 5) RiskMatrix에는 기본 CoF와 최종 PoF를 각각 저장
                    var riskUpd = _riskRepo.UpdateRiskMatrixHI(
                        model.ITR_Code,
                        hiFinal,
                        baseCof,
                        pofFinal);

                    if (!riskUpd.IsSuccess)
                    {
                        result.IsSuccess = false;
                        result.Message = "수정은 성공했으나 RiskMatrix 갱신에 실패했습니다: " + riskUpd.Message;
                    }
                    else
                    {
                        result.Message = "ITR 정밀점검 수정 및 RiskMatrix 반영이 완료되었습니다.";
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
