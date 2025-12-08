
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class DCCTBasicController : Controller
    {
        public ActionResult DCCTBasicList()
        {
            return View("~/Views/Regist/etc/DC Yard/DCCT/DCCTBasicList.cshtml");
        }

        [HttpPost]
        public ActionResult GetDCCTListData()
        {
            try
            {
                LogHelper.WriteLog("DCCTBasicController.List", "GetDCCTListData 실행");

                if (dcctBasicRepository.GetAllDCCTBasicInfoRepo(out var dcctBasicInfos).IsSuccess)
                {
                    LogHelper.WriteLog("DCCTBasicController.List", "DCCT 데이터 로드 성공");

                    var formattedData = dcctBasicInfos.Select(item => new
                    {
                        item.Tbl_Idx,
                        item.DCCT_Code,
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
                    LogHelper.WriteLog("DCCTBasicController.List", "DCCT 데이터 로드 실패");
                    return Json(new { success = false, message = "DCCT 데이터 로드 실패" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("DCCTBasicController.List", $"GetDCCTListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}