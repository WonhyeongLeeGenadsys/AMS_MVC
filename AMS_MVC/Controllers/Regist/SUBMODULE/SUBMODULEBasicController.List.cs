
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class SUBMODULEBasicController : Controller
    {
        public ActionResult SUBMODULEBasicList()
        {
            return View("~/Views/Regist/SUBMODULE/SUBMODULEBasicList.cshtml");
        }

        [HttpPost]
        public ActionResult GetSUBMODULEListData()
        {
            try
            {
                LogHelper.WriteLog("SUBMODULEBasicController.List", "GetSUBMODULEListData 실행");

                if (submoduleBasicRepository.GetAllSUBMODULEBasicInfoRepo(out var submoduleBasicInfos).IsSuccess)
                {
                    LogHelper.WriteLog("SUBMODULEBasicController.List", "SUBMODULE 데이터 로드 성공");

                    var formattedData = submoduleBasicInfos.Select(item => new
                    {
                        item.Tbl_Idx,
                        item.SUBMODULE_Code,
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
                    LogHelper.WriteLog("SUBMODULEBasicController.List", "SUBMODULE 데이터 로드 실패");
                    return Json(new { success = false, message = "SUBMODULE 데이터 로드 실패" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("SUBMODULEBasicController.List", $"GetSUBMODULEListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}