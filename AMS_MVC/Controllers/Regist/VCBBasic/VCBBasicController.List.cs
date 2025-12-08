
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class VCBBasicController : Controller
    {
        public ActionResult VCBBasicList()
        {
            return View("~/Views/Regist/VCB/VCBBasicList.cshtml");
        }

        [HttpPost]
        public ActionResult GetVCBListData()
        {
            try
            {
                LogHelper.WriteLog("VCBBasicController.List", "GetVCBListData 실행");

                if (vcbBasicRepository.GetAllVCBBasicInfoRepo(out var vcbBasicInfos).IsSuccess)
                {
                    LogHelper.WriteLog("VCBBasicController.List", "VCB 데이터 로드 성공");

                    var formattedData = vcbBasicInfos.Select(item => new
                    {
                        item.Tbl_Idx,
                        item.VCB_Code,
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
                    LogHelper.WriteLog("VCBBasicController.List", "VCB 데이터 로드 실패");
                    return Json(new { success = false, message = "VCB 데이터 로드 실패" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("VCBBasicController.List", $"GetVCBListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}