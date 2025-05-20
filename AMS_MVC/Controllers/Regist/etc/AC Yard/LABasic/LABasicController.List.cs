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
    public partial class LABasicController : Controller
    {
        public ActionResult LABasicList()
        {
            return View("~/Views/Regist/etc/AC Yard/LA/LABasicList.cshtml");
        }

        [HttpPost]
        public ActionResult GetLAListData()
        {
            try
            {
                LogHelper.WriteLog("LABasicController.List", "GetLAListData 실행");

                if (laBasicRepository.GetAllLABasicInfoRepo(out var laBasicInfos).IsSuccess)
                {
                    LogHelper.WriteLog("LABasicController.List", "LA 데이터 로드 성공");

                    var formattedData = laBasicInfos.Select(item => new
                    {
                        item.Tbl_Idx,
                        item.LA_Code,
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
                    LogHelper.WriteLog("LABasicController.List", "LA 데이터 로드 실패");
                    return Json(new { success = false, message = "LA 데이터 로드 실패" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("LABasicController.List", $"GetLAListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}