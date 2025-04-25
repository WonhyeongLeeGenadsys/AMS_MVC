// Controllers/Check/ITRChkController.Add.cs
using System;
using System.Web.Mvc;
using AMS_MVC.Models;

namespace AMS_MVC.Controllers.Check
{
    public partial class ITRChkController
    {
        // GET: /Check/ITRChk/Add/{ITR_Code}?type=1
        public ActionResult Add(string ITR_Code, int type = 1)
        {
            var basic = _basicRepo.GetITRBasicInfoByITRCode(ITR_Code);
            ViewBag.SerialNo = basic?.Serial_No ?? "";
            ViewBag.ITR_Code = ITR_Code;

            if (_companyRepo.GetAllCompanies(out var comps).IsSuccess)
                ViewBag.Companies = comps;
            else
                ViewBag.ErrorMessage = "제작사 정보를 불러올 수 없습니다.";

            // type == 1 → 보통점검, 2 → 정밀점검
            string view = type == 1
                ? "~/Views/Check/ITR/ITRChk1Add.cshtml"
                : "~/Views/Check/ITR/ITRChk2Add.cshtml";
            return View(view);
        }

        [HttpPost]
        public ActionResult Add1(ITRChk1 model)  // 보통점검
        {
            model.CHK1_Writer = Session["User_Name"]?.ToString() ?? "Anonymous";
            model.CHK1_Tbl_GetDate = model.CHK1_Tbl_GetDate < new DateTime(1753, 1, 1)
                ? DateTime.Now
                : model.CHK1_Tbl_GetDate;

            var result = _chk1Repo.CreateITRChk1InfoRepo(model);
            if (!result.IsSuccess) result.Message = "보통점검 등록 실패: " + result.Message;
            return Json(result);
        }

        [HttpPost]
        public ActionResult Add2(ITRChk2 model)  // 정밀점검
        {
            model.CHK2_Writer = Session["User_Name"]?.ToString() ?? "Anonymous";
            model.CHK2_Tbl_GetDate = model.CHK2_Tbl_GetDate < new DateTime(1753, 1, 1)
                ? DateTime.Now
                : model.CHK2_Tbl_GetDate;

            var result = _chk2Repo.CreateITRChk2InfoRepo(model);
            if (!result.IsSuccess) result.Message = "정밀점검 등록 실패: " + result.Message;
            return Json(result);
        }
    }
}
