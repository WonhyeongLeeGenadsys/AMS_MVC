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
    public partial class WALLBUSHINGBasicController : Controller
    {
        public ActionResult WALLBUSHINGBasicList()
        {
            return View("~/Views/Regist/etc/AC Yard/WALLBUSHING/WALLBUSHINGBasicList.cshtml");
        }

        [HttpPost]
        public ActionResult GetWALLBUSHINGListData()
        {
            try
            {
                LogHelper.WriteLog("WALLBUSHINGBasicController.List", "GetWALLBUSHINGListData 실행");

                if (wallbushingBasicRepository.GetAllWALLBUSHINGBasicInfoRepo(out var wallbushingBasicInfos).IsSuccess)
                {
                    LogHelper.WriteLog("WALLBUSHINGBasicController.List", "WALLBUSHING 데이터 로드 성공");

                    var formattedData = wallbushingBasicInfos.Select(item => new
                    {
                        item.Tbl_Idx,
                        item.WALLBUSHING_Code,
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
                    LogHelper.WriteLog("WALLBUSHINGBasicController.List", "WALLBUSHING 데이터 로드 실패");
                    return Json(new { success = false, message = "WALLBUSHING 데이터 로드 실패" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("WALLBUSHINGBasicController.List", $"GetWALLBUSHINGListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}