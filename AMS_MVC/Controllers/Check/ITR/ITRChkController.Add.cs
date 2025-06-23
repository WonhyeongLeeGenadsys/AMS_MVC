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
                    // 4) 반대 검사(정밀) 최신 점수 조회
                    var other = _chk2Repo.GetLatestFoldingFunction(model.ITR_Code);
                    int hi2 = other.HasValue
                        ? Math.Max(model.FoldingFunction, other.Value)
                        : model.FoldingFunction;

                    // 5) Riskmatrix.HI 업데이트
                    //var upd = _riskRepo.UpdateRiskMatrixHI(model.ITR_Code, hi2);
                    //var upd = _riskRepo.UpdateRiskMatrixHI(model.ITR_Code, hi2, pof);
                    //if (!upd.IsSuccess)
                    //{
                    //    LogHelper.WriteLog("Riskmatrix HI 갱신 실패", upd.Message);
                    //    result.Message += " (RiskMatrix HI 갱신에 실패했습니다.)";
                    //}

                    var cofModel = cofRepo.GetLatest("ITR");
                    decimal cofValue = cofModel.Total_Cof;

                    Result updateResult = _riskRepo.UpdateRiskMatrixHI(model.ITR_Code, hi2, cofValue, pof);
                    LogHelper.WriteLog("ITRChkAdd2", $"[UpdateRiskMatrixHI] code={model.ITR_Code}, hi={hi2}, cof={cofValue}, pof={pof}");
                    if (!updateResult.IsSuccess)
                    {
                        // HI 업데이트 실패 시 메시지 추가
                        result.IsSuccess = false;
                        result.Message += " / HI 업데이트 실패: " + updateResult.Message;
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
                var (hi, pof) = scoreCalculator.CalculateHiPof(model, alpha: 0.99m);
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
                    // 4) 반대 검사(보통) 최신 점수 조회
                    var other = _chk1Repo.GetLatestFoldingFunction(model.ITR_Code);
                    int hi1 = other.HasValue
                        ? Math.Max(model.FoldingFunction, other.Value)
                        : model.FoldingFunction;

                    // 5) Riskmatrix.HI 업데이트
                    //var upd = _riskRepo.UpdateRiskMatrixHI(model.ITR_Code, hi);
                    //var upd = _riskRepo.UpdateRiskMatrixHI(model.ITR_Code, hi1, pof);

                    //if (!upd.IsSuccess)
                    //{
                    //    LogHelper.WriteLog("Riskmatrix HI 갱신 실패", upd.Message);
                    //    result.Message += " (RiskMatrix HI 갱신에 실패했습니다.)";
                    //}

                    var cofModel = cofRepo.GetLatest("ITR");
                    decimal cofValue = cofModel.Total_Cof;

                    Result updateResult = _riskRepo.UpdateRiskMatrixHI(model.ITR_Code, hi1, cofValue, pof);
                    LogHelper.WriteLog("ITRChkAdd2", $"[UpdateRiskMatrixHI] code={model.ITR_Code}, hi={hi1}, cof={cofValue}, pof={pof}");
                    if (!updateResult.IsSuccess)
                    {
                        // HI 업데이트 실패 시 메시지 추가
                        result.IsSuccess = false;
                        result.Message += " / HI 업데이트 실패: " + updateResult.Message;
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