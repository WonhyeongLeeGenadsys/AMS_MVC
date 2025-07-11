// Controllers/Check/ITRChkController.List.cs
using System;
using System.Collections.Generic;
using System.Linq;
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

                    var formatted = data.Select(item => new
                    {
                        item.Tbl_Idx,
                        item.ITR_Code,
                        item.CHK1_Gongsa_Name,
                        item.CHK1_Weather,
                        item.CHK1_Temp,
                        item.CHK1_Hum,
                        item.CHK1_Company,
                        item.CHK1_Worker,
                        item.CHK1_Manager,
                        item.CHK1_Urgent_No,
                        item.CHK1_Type,                        
                        CHK1_Start_Date = item.CHK1_Start_Date?.ToString("yy.MM.dd"),
                        CHK1_End_Date = item.CHK1_End_Date?.ToString("yy.MM.dd"),
                        
                        
                    }).ToList();

                    return Json(formatted);
                }
                else
                {
                    _chk2Repo.GetITRChk2ByITRCode(itrCode, out List<ITRChk2> data);

                    var formatted = data.Select(item => new
                    {
                        item.Tbl_Idx,
                        item.ITR_Code,
                        item.CHK2_Gongsa_Name,
                        item.CHK2_Weather,
                        item.CHK2_Temp,
                        item.CHK2_Hum,
                        item.CHK2_Company,
                        item.CHK2_Worker,
                        item.CHK2_Manager,
                        item.CHK2_Urgent_No,
                        item.CHK2_Type,
                        CHK2_Start_Date = item.CHK2_Start_Date?.ToString("yy.MM.dd"),
                        CHK2_End_Date = item.CHK2_End_Date?.ToString("yy.MM.dd"),

                    }).ToList();

                    return Json(formatted);
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
