
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class WALLBUSHINGChkController : Controller
    {
        public ActionResult WALLBUSHINGChkAdd(string WALLBUSHING_Code)
        {
            // 선택한 WALLBUSHING의 기본정보 조회
            var basicInfo = wallbushingBasicInfoRepository.GetWALLBUSHINGBasicInfoByCode(WALLBUSHING_Code);

            // 모든 WALLBUSHING 기본정보를 드롭다운에 바인딩
            List<WALLBUSHINGBasicInfo> wallbushings;
            var res = wallbushingBasicInfoRepository.GetAllWALLBUSHINGBasicInfoRepo(out wallbushings);
            ViewBag.WALLBUSHINGs = new SelectList(wallbushings, "WALLBUSHING_Code", "SERIAL_NO");

            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.WALLBUSHING_Code = WALLBUSHING_Code;

            var companies = new List<Company>();
            if (companyRepository.GetAllCompanies(out companies).IsSuccess && companies != null)
            {
                ViewBag.Companies = companies;
            }
            else
            {
                ViewBag.ErrorMessage = "제작사 정보를 불러올 수 없습니다.";
            }
            return View("~/Views/Check/etc/AC Yard/WALLBUSHING/WALLBUSHINGChkAdd.cshtml");
        }

        [HttpPost]
        public ActionResult WALLBUSHINGChkAdd(WALLBUSHINGChk model)
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

                result = wallbushingChkRepository.CreateWALLBUSHINGChkRepo(model);

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"오류 발생: {ex.Message}";
                LogHelper.WriteLog("WALLBUSHINGChkAdd Error", ex.Message);
            }

            return Json(result);
        }
    }
}
