using AMS_MVC.Models;
using AMS_MVC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers
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
                
                if(vcbBasicRepository.GetAllVCBBasicInfoRepo(out var vcbBasicInfos).IsSuccess)
                {
                    LogHelper.WriteLog("VCBBasicController.List", "VCB 데이터 로드 성공");
                    return Json(vcbBasicInfos);
                }
                else
                {
                    LogHelper.WriteLog("VCBBasicController.List", "VCB 데이터 로드 실패");
                    return Json(new { success = false, message = "VCB 데이터 로드 실패" });
                }
            }
            catch(Exception ex)
            {
                LogHelper.WriteLog("VCBBasicController.List", $"GetVCBListData 실패: {ex.Message}");
                return Json(new { sucess = false, message = ex.Message });
            }
        }
    }
}