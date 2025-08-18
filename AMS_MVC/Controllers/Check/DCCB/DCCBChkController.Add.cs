using AMS_MVC.Models;
using AMS_MVC.Repositories;
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
    public partial class DCCBChkController : Controller
    {
        public ActionResult DCCBChkAdd(string DCCB_Code)
        {
            // 선택한 DCCB의 기본정보 조회
            var basicInfo = dccbBasicInfoRepository.GetDCCBBasicInfoByCode(DCCB_Code);

            // 모든 DCCB 기본정보를 드롭다운에 바인딩
            List<DCCBBasicInfo> dccbs;
            var res = dccbBasicInfoRepository.GetAllDCCBBasicInfoRepo(out dccbs);
            ViewBag.DCCBs = new SelectList(dccbs, "DCCB_Code", "SERIAL_NO");

            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.DCCB_Code = DCCB_Code;

            var companies = new List<Company>();
            if (companyRepository.GetAllCompanies(out companies).IsSuccess && companies != null)
            {
                ViewBag.Companies = companies;
            }
            else
            {
                ViewBag.ErrorMessage = "제작사 정보를 불러올 수 없습니다.";
            }
            return View("~/Views/Check/DCCB/DCCBChkAdd.cshtml");
        }

        [HttpPost]
        public ActionResult DCCBChkAdd(DCCBChk model)
        {
            Result result = new Result(true);
            try
            {
                model.CHK_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";
                if (model.CHK_Tbl_GetDate < new DateTime(1753, 1, 1))
                    model.CHK_Tbl_GetDate = DateTime.Now;

                var scoreCalculator = new DCCBChkScoreCalculator();
                var (hi, pofRaw) = scoreCalculator.CalculateHiPof(model, alpha: 1.00m);
                model.FoldingFunction = (int)Math.Round(hi);

                result = dccbChkRepository.CreateDCCBChkRepo(model);
                LogHelper.WriteLog("DCCBChkAdd", $"[CreateDCCBChkRepo] Success={result.IsSuccess}, Message={result.Message}");

                if (!result.IsSuccess)
                {
                    result.Message = "DCCB 보통점검 정보를 추가하지 못했습니다.";
                }
                else
                {
                    var cofModel = cofRepo.GetLatest(model.DCCB_Code) ?? cofRepo.GetLatest("DCCB");
                    decimal baseCof = cofModel?.Total_Cof ?? 0m;

                    decimal pofPercent = (pofRaw <= 1m) ? pofRaw * 100m : pofRaw;
                    if (pofPercent < 0m) pofPercent = 0m;
                    if (pofPercent > 100m) pofPercent = 100m;

                    decimal adjustedCof = Math.Round(baseCof * (pofPercent / 100m), 2);

                    Result updateResult = riskMatrixRepository.UpdateRiskMatrixHI(
                        model.DCCB_Code,
                        (int)Math.Round(hi),
                        adjustedCof,
                        pofPercent
                    );

                    LogHelper.WriteLog("DCCBChkAdd",
                        $"[UpdateRiskMatrixHI] code={model.DCCB_Code}, hi={(int)Math.Round(hi)}, baseCof={baseCof}, pof%={pofPercent}, adjustedCof={adjustedCof}, ok={updateResult.IsSuccess}");

                    if (!updateResult.IsSuccess)
                    {
                        result.IsSuccess = false;
                        result.Message += " / HI 업데이트 실패: " + updateResult.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("DCCBChkAdd Error", ex.Message);
            }

            return Json(result);
        }
    }
}