
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class ESBasicController : Controller
    {
        public ActionResult ESBasicList()
        {
            return View("~/Views/Regist/etc/Grounding/ES/ESBasicList.cshtml");
        }

        [HttpPost]
        public ActionResult GetESListData()
        {
            try
            {
                LogHelper.WriteLog("ESBasicController.List", "GetESListData 실행");

                if (esBasicRepository.GetAllESBasicInfoRepo(out var esBasicInfos).IsSuccess)
                {
                    LogHelper.WriteLog("ESBasicController.List", "ES 데이터 로드 성공");

                    var formattedData = esBasicInfos.Select(item => new
                    {
                        item.Tbl_Idx,
                        item.ES_Code,
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
                    LogHelper.WriteLog("ESBasicController.List", "ES 데이터 로드 실패");
                    return Json(new { success = false, message = "ES 데이터 로드 실패" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("ESBasicController.List", $"GetESListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}