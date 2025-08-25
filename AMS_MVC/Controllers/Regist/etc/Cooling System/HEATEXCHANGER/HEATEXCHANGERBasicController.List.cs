
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class HEATEXCHANGERBasicController : Controller
    {
        public ActionResult HEATEXCHANGERBasicList()
        {
            return View("~/Views/Regist/etc/Cooling System/HEATEXCHANGER/HEATEXCHANGERBasicList.cshtml");
        }

        [HttpPost]
        public ActionResult GetHEATEXCHANGERListData()
        {
            try
            {
                LogHelper.WriteLog("HEATEXCHANGERBasicController.List", "GetHEATEXCHANGERListData 실행");

                if (heatexchangerBasicRepository.GetAllHEATEXCHANGERBasicInfoRepo(out var heatexchangerBasicInfos).IsSuccess)
                {
                    LogHelper.WriteLog("HEATEXCHANGERBasicController.List", "HEATEXCHANGER 데이터 로드 성공");

                    var formattedData = heatexchangerBasicInfos.Select(item => new
                    {
                        item.Tbl_Idx,
                        item.HEATEXCHANGER_Code,
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
                    LogHelper.WriteLog("HEATEXCHANGERBasicController.List", "HEATEXCHANGER 데이터 로드 실패");
                    return Json(new { success = false, message = "HEATEXCHANGER 데이터 로드 실패" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("HEATEXCHANGERBasicController.List", $"GetHEATEXCHANGERListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}