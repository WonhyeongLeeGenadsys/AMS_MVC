// Controllers/Check/ITRChkController.List.cs
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AMS_MVC.Models;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Check
{
    public partial class ITRChkController
    {
        // GET: /Check/ITRChk/List/{ITR_Code}?type=1
        public ActionResult ITRChkList(string ITR_Code, int type = 1)
        {
            var basic = _basicRepo.GetITRBasicInfoByITRCode(ITR_Code);
            ViewBag.SerialNo = basic?.Serial_No ?? "";
            ViewBag.Name = basic?.Name ?? "";
            ViewBag.ITR_Code = ITR_Code;

            string view = type == 1
                ? "~/Views/Check/ITR/ITRChk1List.cshtml"
                : "~/Views/Check/ITR/ITRChk2List.cshtml";
            return View(view);
        }

        [HttpPost]
        public ActionResult GetITRChkListData(string itrCode, int type = 1)
        {
            try
            {
                if (type == 1)
                {
                    _chk1Repo.GetITRChk1ByITRCode(itrCode, out List<ITRChk1> data);
                    return Json(data);
                }
                else
                {
                    _chk2Repo.GetITRChk2ByITRCode(itrCode, out List<ITRChk2> data);
                    return Json(data);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("ITRChkController.GetListData", ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
