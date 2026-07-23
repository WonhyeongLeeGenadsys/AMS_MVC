
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class VCBChkController : Controller
    {
        public ActionResult VCBChkAdd(string VCB_Code)
        {
            // 선택한 VCB의 기본정보 조회
            var basicInfo = vcbBasicInfoRepository.GetVCBBasicInfoByCode(VCB_Code);

            // 모든 VCB 기본정보를 드롭다운에 바인딩
            List<VCBBasicInfo> vcbs;
            var res = vcbBasicInfoRepository.GetAllVCBBasicInfoRepo(out vcbs);
            ViewBag.VCBs = new SelectList(vcbs, "VCB_Code", "SERIAL_NO");

            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.VCB_Code = VCB_Code;

            var companies = new List<Company>();
            if (companyRepository.GetAllCompanies(out companies).IsSuccess && companies != null)
            {
                ViewBag.Companies = companies;
            }
            else
            {
                ViewBag.ErrorMessage = "제작사 정보를 불러올 수 없습니다.";
            }
            return View("~/Views/Check/VCB/VCBChkAdd.cshtml");
        }

        //public ActionResult VCBChkAdd(string VCB_Code)
        //{
        //    var basicInfo = vcbBasicInfoRepository.GetVCBBasicInfoByCode(VCB_Code);

        //    List<VCBBasicInfo> vcbs;
        //    var res = vcbBasicInfoRepository.GetAllVCBBasicInfoRepo(out vcbs);
        //    ViewBag.VCBs = new SelectList(vcbs, "VCB_Code", "SERIAL_NO", VCB_Code);

        //    ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
        //    ViewBag.VCB_Code = VCB_Code;

        //    var companies = new List<Company>();
        //    if (companyRepository.GetAllCompanies(out companies).IsSuccess && companies != null)
        //    {
        //        ViewBag.Companies = companies;
        //    }
        //    else
        //    {
        //        ViewBag.Companies = new List<Company>();
        //        ViewBag.ErrorMessage = "제작사 정보를 불러올 수 없습니다.";
        //    }

        //    VCBChk model;
        //    if (!string.IsNullOrWhiteSpace(VCB_Code))
        //    {
        //        vcbChkRepository.GetLatestVCBChkByVCBCode(VCB_Code, out var latestOneList);
        //        model = latestOneList?.FirstOrDefault() ?? new VCBChk { VCB_Code = VCB_Code };
        //    }
        //    else
        //    {
        //        model = new VCBChk();
        //    }

        //    return View("~/Views/Check/VCB/VCBChkAdd.cshtml", model);
        //}


        [HttpPost]
        public ActionResult VCBChkAdd(VCBChk model)
        {
            Result result = new Result(true);
            try
            {
                model.CHK_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";
                if (model.CHK_Tbl_GetDate < new DateTime(1753, 1, 1))
                    model.CHK_Tbl_GetDate = DateTime.Now;

                var scoreCalculator = new VCBChkScoreCalculator();
                var (hi, pofRaw) = scoreCalculator.CalculateHiPof(model, alpha: 1.00m);
                model.FoldingFunction = (int)Math.Truncate(hi);

                result = vcbChkRepository.CreateVCBChkRepo(model);
                LogHelper.WriteLog("VCBChkAdd", $"[CreateVCBChkRepo] Success={result.IsSuccess}, Message={result.Message}");

                if (!result.IsSuccess)
                {
                    result.Message = "VCB 보통점검 정보를 추가하지 못했습니다.";
                }
                else
                {
                    var cofModel = cofRepo.GetLatest("VCB");
                    decimal baseCof = cofModel?.Total_Cof ?? 0m;

                    Result updateResult = riskMatrixRepository.UpdateRiskMatrixHI(
                        model.VCB_Code,
                        (int)Math.Truncate(hi),
                        baseCof,
                        pofRaw
                    );

                    LogHelper.WriteLog("VCBChkAdd",
                        $"[UpdateRiskMatrixHI] code={model.VCB_Code}, hi={(int)Math.Truncate(hi)}, cof={baseCof}, pof%={pofRaw}, ok={updateResult.IsSuccess}");

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
                LogHelper.WriteLog("VCBChkAdd Error", ex.Message);
            }

            return Json(result);
        }


    }
}
