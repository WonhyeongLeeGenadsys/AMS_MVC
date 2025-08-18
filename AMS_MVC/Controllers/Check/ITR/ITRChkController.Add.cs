// Controllers/Check/ITRChkController.Add.cs
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AMS_MVC.Models;
using AMS_MVC.Services;
using AMS_MVC.Utlity;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Check
{
    public partial class ITRChkController
    {
        // GET: /Check/ITRChk/Add/{ITR_Code}?type=1
        public ActionResult ITRChkAdd(string ITR_Code, int type = 1)
        {
            var basicInfo = _basicRepo.GetITRBasicInfoByITRCode(ITR_Code);
            // 모든 ITR 기본정보를 드롭다운에 바인딩
            List<ITRBasicInfo> itrs;
            var res = _basicRepo.GetAllITRBasicInfoRepo(out itrs);
            ViewBag.ITRs = new SelectList(itrs, "ITR_Code", "SERIAL_NO");

            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.ITR_Code = ITR_Code;
            ViewBag.ActiveSubMenu = type == 1
            ? "ITRRegular"    // 보통점검
            : "ITRPrecision"; // 정밀점검
            var companies = new List<Company>();
            if (_companyRepo.GetAllCompanies(out companies).IsSuccess && companies != null)
            {
                ViewBag.Companies = companies;
            }
            else
            {
                ViewBag.ErrorMessage = "제작사 정보를 불러올 수 없습니다.";
            }

            string view = type == 1
                ? "~/Views/Check/ITR/ITRChk1Add.cshtml"

                : "~/Views/Check/ITR/ITRChk2Add.cshtml";
            return View(view);
        }

        [HttpPost]
        public ActionResult ITRChkAdd1(ITRChk1 model)  // 보통점검
        {
            Result result = new Result(true);
            try
            {
                // 1) 작성자, 날짜 처리
                model.CHK1_Writer = Session["User_Name"]?.ToString() ?? "Anonymous";

                if (model.CHK1_Tbl_GetDate < new DateTime(1753, 1, 1))
                    model.CHK1_Tbl_GetDate = DateTime.Now;

                // 2) FoldingFunction 계산
                //model.foldingfunction = _scorecalc.calculatefoldingfunction(model);

                var scoreCalculator = new ITRChkScoreCalculator();
                var (hi, pof) = scoreCalculator.CalculateHiPof(model, alpha: 0.99m);
                model.FoldingFunction = (int)Math.Round(hi);

                // 3) DB 저장
                result = _chk1Repo.CreateITRChk1InfoRepo(model);
                LogHelper.WriteLog("ITRChk1Add", $"[CreateITRChk1Repo] Success={result.IsSuccess}, Message={result.Message}");

                if (!result.IsSuccess)
                {
                    result.Message = "ITR 보통점검 정보를 추가하지 못했습니다.: " + result.Message;
                }
                else
                {
                    // 반대 검사 최신 HI 조회 → 두 검사 중 큰 값으로 HI 집계
                    var other = _chk2Repo.GetLatestFoldingFunction(model.ITR_Code);
                    int hi2 = other.HasValue ? Math.Max(model.FoldingFunction, other.Value) : model.FoldingFunction;

                    // --- CoF/PoF 계산 및 저장 ---
                    // 1) CoF 원본: 장비별 코드 우선, 없으면 타입별("ITR") 폴백
                    var cofModel = cofRepo.GetLatest(model.ITR_Code) ?? cofRepo.GetLatest("ITR");
                    decimal baseCof = cofModel?.Total_Cof ?? 0m;

                    // 2) PoF 정규화: 계산기 값이 0~1이면 %로 변환, 이미 %이면 그대로 (DB는 % 저장)
                    decimal pofPercent = (pof <= 1m) ? pof * 100m : pof;
                    if (pofPercent < 0m) pofPercent = 0m;
                    if (pofPercent > 100m) pofPercent = 100m;

                    // 3) 요구사항: PoF=50% → Total_Cof / 2
                    decimal adjustedCof = Math.Round(baseCof * (pofPercent / 100m), 2);

                    // 4) RiskMatrix 반영 (PoF는 퍼센트로 저장)
                    var upd = _riskRepo.UpdateRiskMatrixHI(model.ITR_Code, hi2, adjustedCof, pofPercent);
                    LogHelper.WriteLog("ITRChkAdd1",
                        $"[UpdateRiskMatrixHI] code={model.ITR_Code}, hi={hi2}, baseCof={baseCof}, pof%={pofPercent}, adjCof={adjustedCof}, ok={upd.IsSuccess}");
                    if (!upd.IsSuccess)
                    {
                        result.IsSuccess = false;
                        result.Message += " / RiskMatrix 업데이트 실패: " + upd.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("ITRChkAdd1 Error", ex.ToString());
            }

            return Json(result);
        }

        [HttpPost]
        public ActionResult ITRChkAdd2(ITRChk2 model)  // 정밀점검
        {
            Result result = new Result(true);
            try
            {
                // 1) 작성자, 날짜 처리
                model.CHK2_Writer = Session["User_Name"]?.ToString() ?? "Anonymous";
                if (model.CHK2_Tbl_GetDate < new DateTime(1753, 1, 1))
                    model.CHK2_Tbl_GetDate = DateTime.Now;

                // 2) FoldingFunction 계산
                //model.FoldingFunction = _scoreCalc.CalculateFoldingFunction(model);

                var scoreCalculator = new ITRChkScoreCalculator();
                var (hi, pof) = scoreCalculator.CalculateHiPof(model, alpha: 1.00m);
                model.FoldingFunction = (int)Math.Round(hi);

                // 3) DB 저장
                result = _chk2Repo.CreateITRChk2InfoRepo(model);
                LogHelper.WriteLog("ITRChk2Add", $"[CreateITRChk2Repo] Success={result.IsSuccess}, Message={result.Message}");

                if (!result.IsSuccess)
                {
                    result.Message = "ITR 정밀점검 정보를 추가하지 못했습니다.: " + result.Message;
                }
                else
                {
                    // 반대 검사 최신 HI 조회 → 두 검사 중 큰 값으로 HI 집계
                    var other = _chk1Repo.GetLatestFoldingFunction(model.ITR_Code);
                    int hi1 = other.HasValue ? Math.Max(model.FoldingFunction, other.Value) : model.FoldingFunction;

                    // --- CoF/PoF 계산 및 저장 ---
                    var cofModel = cofRepo.GetLatest(model.ITR_Code) ?? cofRepo.GetLatest("ITR");
                    decimal baseCof = cofModel?.Total_Cof ?? 0m;

                    decimal pofPercent = (pof <= 1m) ? pof * 100m : pof;
                    if (pofPercent < 0m) pofPercent = 0m;
                    if (pofPercent > 100m) pofPercent = 100m;

                    decimal adjustedCof = Math.Round(baseCof * (pofPercent / 100m), 2);

                    var upd = _riskRepo.UpdateRiskMatrixHI(model.ITR_Code, hi1, adjustedCof, pofPercent);
                    LogHelper.WriteLog("ITRChkAdd2",
                        $"[UpdateRiskMatrixHI] code={model.ITR_Code}, hi={hi1}, baseCof={baseCof}, pof%={pofPercent}, adjCof={adjustedCof}, ok={upd.IsSuccess}");
                    if (!upd.IsSuccess)
                    {
                        result.IsSuccess = false;
                        result.Message += " / RiskMatrix 업데이트 실패: " + upd.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("ITRChkAdd2 Error", ex.ToString());
            }

            return Json(result);
        }

    }
}