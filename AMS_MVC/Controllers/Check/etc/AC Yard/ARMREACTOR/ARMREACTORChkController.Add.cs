
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class ARMREACTORChkController : Controller
    {
        public ActionResult ARMREACTORChkAdd(string ARMREACTOR_Code)
        {
            // 선택한 ARMREACTOR의 기본정보 조회
            var basicInfo = armreactorBasicInfoRepository.GetARMREACTORBasicInfoByCode(ARMREACTOR_Code);

            // 모든 ARMREACTOR 기본정보를 드롭다운에 바인딩
            List<ARMREACTORBasicInfo> armreactors;
            var res = armreactorBasicInfoRepository.GetAllARMREACTORBasicInfoRepo(out armreactors);
            ViewBag.ARMREACTORs = new SelectList(armreactors, "ARMREACTOR_Code", "SERIAL_NO");

            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.ARMREACTOR_Code = ARMREACTOR_Code;

            var companies = new List<Company>();
            if (companyRepository.GetAllCompanies(out companies).IsSuccess && companies != null)
            {
                ViewBag.Companies = companies;
            }
            else
            {
                ViewBag.ErrorMessage = "제작사 정보를 불러올 수 없습니다.";
            }
            return View("~/Views/Check/etc/AC Yard/ARMREACTOR/ARMREACTORChkAdd.cshtml");
        }

        [HttpPost]
        public ActionResult ARMREACTORChkAdd(ARMREACTORChk model)
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

                result = armreactorChkRepository.CreateARMREACTORChkRepo(model);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("ARMREACTORChkAdd Error", ex.Message);
            }

            return Json(result);
        }
    }
}
