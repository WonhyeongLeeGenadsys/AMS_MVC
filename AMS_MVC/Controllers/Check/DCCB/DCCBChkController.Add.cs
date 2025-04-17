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
                // 작성자 설정
                model.CHK_Writer = Session["User_Name"] != null ? Session["User_Name"].ToString() : "Anonymous";

                //// DB 저장 시 SqlDateTime 범위(1753-01-01 ~ 9999-12-31)에 벗어나지 않도록 날짜 값이 없는 경우 현재 날짜로 설정
                if (model.CHK_Tbl_GetDate < new DateTime(1753, 1, 1))
                {
                    model.CHK_Tbl_GetDate = DateTime.Now;
                }

                DCCBChkScoreCalculator scoreCalculator = new DCCBChkScoreCalculator();
                model.FoldingFunction = scoreCalculator.CalculateFoldingFunction(model);

                result = dccbChkRepository.CreateDCCBChkRepo(model);

                if (!result.IsSuccess)
                {
                    result.Message = "DCCB 보통점검 정보를 추가하지 못했습니다.";
                }
                else
                {
                    // HI(건전도)를 Riskmatrix에 업데이트: model.DCCB_Code에 대해 FoldingFunction 값을 HI에 넣는다.
                    Result updateResult = riskMatrixRepository.UpdateRiskMatrixHI(model.DCCB_Code, model.FoldingFunction);
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
                LogHelper.WriteLog("DCCBChkAdd Error", ex.Message);
            }

            return Json(result);
        }
    }
}
