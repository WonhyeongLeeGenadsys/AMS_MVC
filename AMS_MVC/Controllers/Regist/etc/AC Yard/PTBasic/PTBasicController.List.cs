
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class PTBasicController : Controller
    {
        public ActionResult PTBasicList()
        {
            return View("~/Views/Regist/etc/AC Yard/PT/PTBasicList.cshtml");
        }

        [HttpPost]
        public ActionResult GetPTListData()
        {
            try
            {
                LogHelper.WriteLog("PTBasicController.List", "GetPTListData 실행");

                if (ptBasicRepository.GetAllPTBasicInfoRepo(out var ptBasicInfos).IsSuccess)
                {
                    LogHelper.WriteLog("PTBasicController.List", "PT 데이터 로드 성공");

                    var formattedData = ptBasicInfos.Select(item => new
                    {
                        item.Tbl_Idx,
                        item.PT_Code,
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
                    LogHelper.WriteLog("PTBasicController.List", "PT 데이터 로드 실패");
                    return Json(new { success = false, message = "PT 데이터 로드 실패" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("PTBasicController.List", $"GetPTListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}