
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class SABasicController : Controller
    {
        public ActionResult SABasicList()
        {
            return View("~/Views/Regist/etc/DC Yard/SA/SABasicList.cshtml");
        }

        [HttpPost]
        public ActionResult GetSAListData()
        {
            try
            {
                LogHelper.WriteLog("SABasicController.List", "GetSAListData 실행");

                if (saBasicRepository.GetAllSABasicInfoRepo(out var saBasicInfos).IsSuccess)
                {
                    LogHelper.WriteLog("SABasicController.List", "SA 데이터 로드 성공");

                    var formattedData = saBasicInfos.Select(item => new
                    {
                        item.Tbl_Idx,
                        item.SA_Code,
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
                    LogHelper.WriteLog("SABasicController.List", "SA 데이터 로드 실패");
                    return Json(new { success = false, message = "SA 데이터 로드 실패" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("SABasicController.List", $"GetSAListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}