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
    public partial class DCCABLEChkController : Controller
    {
        public ActionResult DCCABLEChkAdd(string DCCABLE_Code)
        {
            // 선택한 DCCABLE의 기본정보 조회
            var basicInfo = dccableBasicInfoRepository.GetDCCABLEBasicInfoByCode(DCCABLE_Code);

            // 모든 DCCABLE 기본정보를 드롭다운에 바인딩
            List<DCCABLEBasicInfo> dccables;
            var res = dccableBasicInfoRepository.GetAllDCCABLEBasicInfoRepo(out dccables);
            ViewBag.DCCABLEs = new SelectList(dccables, "DCCABLE_Code", "SERIAL_NO");

            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.DCCABLE_Code = DCCABLE_Code;

            var companies = new List<Company>();
            if (companyRepository.GetAllCompanies(out companies).IsSuccess && companies != null)
            {
                ViewBag.Companies = companies;
            }
            else
            {
                ViewBag.ErrorMessage = "제작사 정보를 불러올 수 없습니다.";
            }
            return View("~/Views/Check/DCCABLE/DCCABLEChkAdd.cshtml");
        }

        [HttpPost]
        public ActionResult DCCABLEChkAdd(DCCABLEChk model)
        {
            Result result = new Result(true);
            try
            {
                model.CHK_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";
                if (model.CHK_Tbl_GetDate < new DateTime(1753, 1, 1))
                    model.CHK_Tbl_GetDate = DateTime.Now;

                var scoreCalculator = new DCCABLEChkScoreCalculator();
                var (hi, pofRaw) = scoreCalculator.CalculateHiPof(model, alpha: 1.00m);
                model.FoldingFunction = (int)Math.Round(hi);

                result = dccableChkRepository.CreateDCCABLEChkRepo(model);
                LogHelper.WriteLog("DCCABLEChkAdd", $"[CreateDCCABLEChkRepo] Success={result.IsSuccess}, Message={result.Message}");

                if (!result.IsSuccess)
                {
                    result.Message = "DCCABLE 보통점검 정보를 추가하지 못했습니다.";
                }
                else
                {
                    var cofModel = cofRepo.GetLatest(model.DCCABLE_Code) ?? cofRepo.GetLatest("DCCABLE");
                    decimal baseCof = cofModel?.Total_Cof ?? 0m;

                    decimal pofPercent = (pofRaw <= 1m) ? pofRaw * 100m : pofRaw;
                    if (pofPercent < 0m) pofPercent = 0m;
                    if (pofPercent > 100m) pofPercent = 100m;

                    decimal adjustedCof = Math.Round(baseCof * (pofPercent / 100m), 2);

                    Result updateResult = riskMatrixRepository.UpdateRiskMatrixHI(
                        model.DCCABLE_Code,
                        (int)Math.Round(hi),
                        adjustedCof,
                        pofPercent
                    );

                    LogHelper.WriteLog("DCCABLEChkAdd",
                        $"[UpdateRiskMatrixHI] code={model.DCCABLE_Code}, hi={(int)Math.Round(hi)}, baseCof={baseCof}, pof%={pofPercent}, adjustedCof={adjustedCof}, ok={updateResult.IsSuccess}");

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
                LogHelper.WriteLog("DCCABLEChkAdd Error", ex.Message);
            }

            return Json(result);
        }
    }
}