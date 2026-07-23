// Controllers/Check/ITRChkController.Add.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
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
        public ActionResult ITRChkAdd1(ITRChk1 model)
        {
            var result = new Result(true);
            try
            {
                model.CHK1_Writer = (Session["User_Name"]?.ToString()) ?? "Anonymous";
                if (model.CHK1_Tbl_GetDate < new DateTime(1753, 1, 1)) model.CHK1_Tbl_GetDate = DateTime.Now;

                var calc = new ITRChkScoreCalculator();
                var (hi1, pof1) = calc.CalculateHiPof(model, 1.00m);
                model.FoldingFunction = (int)decimal.Truncate(hi1);

                result = _chk1Repo.CreateITRChk1InfoRepo(model);
                if (!result.IsSuccess)
                    return Json(new Result(false) { Message = "ITR 보통점검 추가 실패: " + result.Message });

                // 최신 정밀점검 가져와 합산 계산하기
                _chk2Repo.GetLatestITRChk2ByITRCode(model.ITR_Code, out var list2);
                var latest2 = list2?.OrderBy(x => x.Tbl_Idx).LastOrDefault();

                decimal hiFinalDec, pofFinal;
                if (latest2 != null)
                    (hiFinalDec, pofFinal) = calc.CalculateHiPofCombined(model, latest2, 1.00m);
                else
                    (hiFinalDec, pofFinal) = (hi1, pof1);

                int hiFinal = (int)decimal.Truncate(hiFinalDec);
                pofFinal = Math.Min(100m, Math.Max(0m, pofFinal));

                var cofModel = cofRepo.GetLatest("ITR");
                decimal baseCof = cofModel?.Total_Cof ?? 0m;

                var upd = _riskRepo.UpdateRiskMatrixHI(model.ITR_Code, hiFinal, baseCof, pofFinal);

                if (!upd.IsSuccess)
                    return Json(new Result(false) { Message = "RiskMatrix 업데이트 실패: " + upd.Message });

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new Result(false) { Message = "오류 발생: " + ex.Message });
            }
        }

        [HttpPost]
        public ActionResult ITRChkAdd2(ITRChk2 model)
        {
            var result = new Result(true);
            try
            {
                model.CHK2_Writer = (Session["User_Name"]?.ToString()) ?? "Anonymous";
                if (model.CHK2_Tbl_GetDate < new DateTime(1753, 1, 1)) model.CHK2_Tbl_GetDate = DateTime.Now;

                var calc = new ITRChkScoreCalculator();
                var (hi2, pof2) = calc.CalculateHiPof(model, 1.00m);
                model.FoldingFunction = (int)decimal.Truncate(hi2);

                result = _chk2Repo.CreateITRChk2InfoRepo(model);
                if (!result.IsSuccess)
                    return Json(new Result(false) { Message = "ITR 정밀점검 추가 실패: " + result.Message });

                // 최신 보통점검 가져와 합산 계산하기
                _chk1Repo.GetLatestITRChk1ByITRCode(model.ITR_Code, out var list1);
                var latest1 = list1?.OrderBy(x => x.Tbl_Idx).LastOrDefault();

                decimal hiFinalDec, pofFinal;
                if (latest1 != null)
                    (hiFinalDec, pofFinal) = calc.CalculateHiPofCombined(latest1, model, 1.00m);
                else
                    (hiFinalDec, pofFinal) = (hi2, pof2);

                int hiFinal = (int)decimal.Truncate(hiFinalDec);
                pofFinal = Math.Min(100m, Math.Max(0m, pofFinal));

                var cofModel = cofRepo.GetLatest("ITR");
                decimal baseCof = cofModel?.Total_Cof ?? 0m;

                var upd = _riskRepo.UpdateRiskMatrixHI(model.ITR_Code, hiFinal, baseCof, pofFinal);

                if (!upd.IsSuccess)
                    return Json(new Result(false) { Message = "RiskMatrix 업데이트 실패: " + upd.Message });

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new Result(false) { Message = "오류 발생: " + ex.Message });
            }
        }

    }
}
