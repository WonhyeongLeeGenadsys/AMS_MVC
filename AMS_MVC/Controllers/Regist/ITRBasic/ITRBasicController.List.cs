using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers
{
    public partial class ITRBasicController : Controller
    {
        // GET: ITR
        public ActionResult ITRBasicList()
        {
            return View("~/Views/Regist/ITR/ITRBasicList.cshtml");
        }

        [HttpPost]
        public ActionResult GetITRListData()
        {
            try
            {
                LogHelper.WriteLog("ITRBasicController.List", "GetITRListData 실행");

                if (itrBasicRepository.GetAllITRBasicInfoRepo(out var itrBasicInfos).IsSuccess)
                {
                    LogHelper.WriteLog("ITRBasicController.List", "ITR 데이터 로드 성공");

                    var formattedData = itrBasicInfos.Select(item => new
                    {
                        item.Tbl_Idx,
                        item.ITR_Code,
                        item.Name,
                        item.Serial_No,
                        Install_Date = item.Install_Date?.ToString("yy.MM.dd"),
                        Operating_Date = item.Operating_Date?.ToString("yy.MM.dd"),
                        Tbl_GetDate = item.Tbl_GetDate.ToString("yy.MM.dd"),
                        item.Install_Place,
                        item.Price,
                        item.Capacity,
                        item.Rated_A,
                        item.Rated_V,
                        item.Constant,
                        item.Make_Company,
                        item.Make_No,
                        item.Photo,
                        item.Writer,
                        item.Is_Diagnostics,
                        item.Is_Health
                    }).ToList();

                    return Json(new { success = true, data = formattedData });
                }
                else
                {
                    LogHelper.WriteLog("ITRBasicController.List", "ITR 데이터 로드 실패");
                    return Json(new { success = false, message = "ITR 데이터 로드 실패" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("ITRBasicController.List", $"ITRListData 실패: {ex.Message}");
                return Json(new { sucess = false, message = ex.Message });
            }
        }
    }
}