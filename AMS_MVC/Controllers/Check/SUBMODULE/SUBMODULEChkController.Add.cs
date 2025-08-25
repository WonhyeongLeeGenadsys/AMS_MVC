
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class SUBMODULEChkController : Controller
    {
        public ActionResult SUBMODULEChkAdd(string SUBMODULE_Code)
        {
            // 선택한 SUBMODULE의 기본정보 조회
            var basicInfo = submoduleBasicInfoRepository.GetSUBMODULEBasicInfoByCode(SUBMODULE_Code);

            // 모든 SUBMODULE 기본정보를 드롭다운에 바인딩
            List<SUBMODULEBasicInfo> submodules;
            var res = submoduleBasicInfoRepository.GetAllSUBMODULEBasicInfoRepo(out submodules);
            ViewBag.SUBMODULEs = new SelectList(submodules, "SUBMODULE_Code", "SERIAL_NO");

            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.SUBMODULE_Code = SUBMODULE_Code;

            var companies = new List<Company>();
            if (companyRepository.GetAllCompanies(out companies).IsSuccess && companies != null)
            {
                ViewBag.Companies = companies;
            }
            else
            {
                ViewBag.ErrorMessage = "제작사 정보를 불러올 수 없습니다.";
            }
            return View("~/Views/Check/SUBMODULE/SUBMODULEChkAdd.cshtml");
        }

        [HttpPost]
        public ActionResult SUBMODULEChkAdd(SUBMODULEChk model)
        {
            Result result = new Result(true);
            try
            {
                model.CHK_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";
                if (model.CHK_Tbl_GetDate < new DateTime(1753, 1, 1))
                    model.CHK_Tbl_GetDate = DateTime.Now;

                var scoreCalculator = new SUBMODULEChkScoreCalculator();
                var (hi, pofRaw) = scoreCalculator.CalculateHiPof(model, alpha: 1.00m);
                model.FoldingFunction = (int)Math.Truncate(hi);

                result = submoduleChkRepository.CreateSUBMODULEChkRepo(model);
                LogHelper.WriteLog("SUBMODULEChkAdd", $"[CreateSUBMODULEChkRepo] Success={result.IsSuccess}, Message={result.Message}");

                if (!result.IsSuccess)
                {
                    result.Message = "SUBMODULE 보통점검 정보를 추가하지 못했습니다.";
                }
                else
                {
                    var cofModel = cofRepo.GetLatest("SUBMODULE");
                    decimal baseCof = cofModel?.Total_Cof ?? 0m;

                    decimal adjustedCof = Math.Round(baseCof * (pofRaw / 100m), 2);

                    Result updateResult = riskMatrixRepository.UpdateRiskMatrixHI(
                        model.SUBMODULE_Code,
                        (int)Math.Truncate(hi),
                        adjustedCof,
                        pofRaw
                    );

                    LogHelper.WriteLog("SUBMODULEChkAdd",
                        $"[UpdateRiskMatrixHI] code={model.SUBMODULE_Code}, hi={(int)Math.Truncate(hi)}, baseCof={baseCof}, pof%={pofRaw}, adjustedCof={adjustedCof}, ok={updateResult.IsSuccess}");

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
                LogHelper.WriteLog("SUBMODULEChkAdd Error", ex.Message);
            }

            return Json(result);
        }
    }
}