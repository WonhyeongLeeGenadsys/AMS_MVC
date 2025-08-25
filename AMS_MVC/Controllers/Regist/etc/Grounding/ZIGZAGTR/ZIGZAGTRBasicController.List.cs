
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class ZIGZAGTRBasicController : Controller
    {
        public ActionResult ZIGZAGTRBasicList()
        {
            return View("~/Views/Regist/etc/Grounding/ZIGZAGTR/ZIGZAGTRBasicList.cshtml");
        }

        [HttpPost]
        public ActionResult GetZIGZAGTRListData()
        {
            try
            {
                LogHelper.WriteLog("ZIGZAGTRBasicController.List", "GetZIGZAGTRListData 실행");

                if (zigzagtrBasicRepository.GetAllZIGZAGTRBasicInfoRepo(out var zigzagtrBasicInfos).IsSuccess)
                {
                    LogHelper.WriteLog("ZIGZAGTRBasicController.List", "ZIGZAGTR 데이터 로드 성공");

                    var formattedData = zigzagtrBasicInfos.Select(item => new
                    {
                        item.Tbl_Idx,
                        item.ZIGZAGTR_Code,
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
                    LogHelper.WriteLog("ZIGZAGTRBasicController.List", "ZIGZAGTR 데이터 로드 실패");
                    return Json(new { success = false, message = "ZIGZAGTR 데이터 로드 실패" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("ZIGZAGTRBasicController.List", $"GetZIGZAGTRListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}