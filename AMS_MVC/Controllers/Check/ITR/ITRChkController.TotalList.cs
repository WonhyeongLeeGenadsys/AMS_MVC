// Controllers/Check/ITRChkController.TotalList.cs
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AMS_MVC.Models;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Check
{
    public partial class ITRChkController
    {
        // GET: /Check/ITRChk/TotalList?type=1
        public ActionResult TotalList(int type = 1)
        {
            string view = type == 1
                ? "~/Views/Check/Total/ITRChk1TotalList.cshtml"
                : "~/Views/Check/Total/ITRChk2TotalList.cshtml";
            return View(view);
        }

        [HttpPost]
        public ActionResult GetTotalListData(int type = 1)
        {
            try
            {
                if (type == 1)
                {
                    _chk1Repo.GetTotalITRChk1(out List<ITRChk1> data);
                    return Json(data);
                }
                else
                {
                    _chk2Repo.GetTotalITRChk2(out List<ITRChk2> data);
                    return Json(data);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("ITRChkController.GetTotalListData", ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
