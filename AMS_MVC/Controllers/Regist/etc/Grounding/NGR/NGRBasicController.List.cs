using AMS_MVC.Models;
using AMS_MVC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers
{
    public partial class NGRBasicController : Controller
    {
        public ActionResult NGRBasicList()
        {
            return View("~/Views/Regist/etc/Grounding/NGR/NGRBasicList.cshtml");
        }

        [HttpPost]
        public ActionResult GetNGRListData()
        {
            try
            {
                LogHelper.WriteLog("NGRBasicController.List", "GetNGRListData 실행");

                if (ngrBasicRepository.GetAllNGRBasicInfoRepo(out var ngrBasicInfos).IsSuccess)
                {
                    LogHelper.WriteLog("NGRBasicController.List", "NGR 데이터 로드 성공");

                    var formattedData = ngrBasicInfos.Select(item => new
                    {
                        item.Tbl_Idx,
                        item.NGR_Code,
                        item.Name,
                        item.Serial_No,
                        Install_Date = item.Install_Date?.ToString("yy.MM.dd"), 
                        Operating_Date = item.Operating_Date?.ToString("yy.MM.dd"), 
                        Tbl_GetDate = item.Tbl_GetDate.ToString("yy.MM.dd"),
                        item.Install_Place,
                        item.Is_Diagnostics,
                        item.Is_Health
                    }).ToList();

                    return Json(new { success = true, data = formattedData });
                }
                else
                {
                    LogHelper.WriteLog("NGRBasicController.List", "NGR 데이터 로드 실패");
                    return Json(new { success = false, message = "NGR 데이터 로드 실패" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("NGRBasicController.List", $"GetNGRListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}