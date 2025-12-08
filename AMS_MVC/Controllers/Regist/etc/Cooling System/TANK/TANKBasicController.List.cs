
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class TANKBasicController : Controller
    {
        public ActionResult TANKBasicList()
        {
            return View("~/Views/Regist/etc/Cooling System/TANK/TANKBasicList.cshtml");
        }

        [HttpPost]
        public ActionResult GetTANKListData()
        {
            try
            {
                LogHelper.WriteLog("TANKBasicController.List", "GetTANKListData 실행");

                if (tankBasicRepository.GetAllTANKBasicInfoRepo(out var tankBasicInfos).IsSuccess)
                {
                    LogHelper.WriteLog("TANKBasicController.List", "TANK 데이터 로드 성공");

                    var formattedData = tankBasicInfos.Select(item => new
                    {
                        item.Tbl_Idx,
                        item.TANK_Code,
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
                    LogHelper.WriteLog("TANKBasicController.List", "TANK 데이터 로드 실패");
                    return Json(new { success = false, message = "TANK 데이터 로드 실패" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("TANKBasicController.List", $"GetTANKListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}