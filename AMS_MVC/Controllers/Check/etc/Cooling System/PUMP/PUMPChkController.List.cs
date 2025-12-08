using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class PUMPChkController : Controller
    {
        public ActionResult PUMPChkList(string PUMP_Code)
        {
            var basicInfo = pumpBasicInfoRepository.GetPUMPBasicInfoByCode(PUMP_Code);
            ViewBag.PUMPCode = PUMP_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Check/etc/Cooling System/PUMP/PUMPChkList.cshtml");
        }

        public ActionResult PUMPChkTotalList()
        {
            return View("~/Views/Check/Total/etc/Cooling System/PUMPChkTotalList.cshtml");
        }

        [HttpPost]
        public ActionResult GetPUMPChkListData(string pumpCode)
        {
            try
            {
                LogHelper.WriteLog("PUMPChkController.List", "GetPUMPChkListData 실행");
                LogHelper.WriteLog("pumpCode 조회 : ",$"{pumpCode}");

                List<PUMPChk> pumpChks = new List<PUMPChk>();
                var repoResult = pumpChkRepository.GetPUMPChkByPUMPCode(pumpCode, out pumpChks);
                if (repoResult.IsSuccess)
                {
                    var formattedData = pumpChks.Select(item => new
                    {
                        item.Tbl_Idx,
                        item.PUMP_Code,
                        item.CHK_Gongsa_Name,
                        item.CHK_Weather,
                        item.CHK_Temp,
                        item.CHK_Hum,
                        item.CHK_Company,
                        item.CHK_Worker,
                        item.CHK_Manager,
                        item.CHK_Urgent_No,
                        item.CHK_Type,
                        CHK_Start_Date = item.CHK_Start_Date?.ToString("yy.MM.dd"),
                        CHK_End_Date = item.CHK_End_Date?.ToString("yy.MM.dd"),

                        item.CHK_Writer,
                    }).ToList();


                    LogHelper.WriteLog("PUMPChkController.List", $"조회된 데이터: {pumpChks.Count}건");
                    return Json(formattedData);
                }
                else
                {
                    LogHelper.WriteLog("PUMPChkController.List", "PUMP 보통점검 데이터 로드 실패");
                    return Json(new { success = false, message = "PUMP 보통점검 데이터 로드 실패" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("PUMPChkController.List", $"GetPUMPListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalPUMPChkListData()
        {
            // 1) 전체 점검 데이터 조회
            var repoResult = pumpChkRepository.GetTotalPUMPChk(out var pumpChks);
            if (!repoResult.IsSuccess)
                return Json(new { success = false, message = repoResult.Message });

            // 2) PUMP 기본정보 전체 조회 → 코드별 매핑용 딕셔너리 생성
            pumpBasicInfoRepository.GetAllPUMPBasicInfoRepo(out var basics);
            var basicMap = basics.ToDictionary(b => b.PUMP_Code, b => b);

            // 3) JSON 응답용 객체에 Name, Serial_No 및 모든 CHK_* 필드를 한 번에 담기
            var formatted = pumpChks.Select(item =>
            {
                basicMap.TryGetValue(item.PUMP_Code, out var basic);
                return new
                {
                    item.Tbl_Idx,
                    item.PUMP_Code,

                    // 추가한 기본정보
                    Name = basic?.Name ?? "",
                    Serial_No = basic?.Serial_No ?? "",

                    item.CHK_Gongsa_Name,
                    item.CHK_Weather,
                    item.CHK_Temp,
                    item.CHK_Hum,
                    item.CHK_Company,
                    item.CHK_Worker,
                    item.CHK_Manager,
                    item.CHK_Urgent_No,
                    item.CHK_Type,

                    CHK_Start_Date = item.CHK_Start_Date?.ToString("yy.MM.dd"),
                    CHK_End_Date = item.CHK_End_Date?.ToString("yy.MM.dd"),

                    item.CHK_Writer,

                    // DB 반영 일시
                    CHK_Tbl_GetDate = item.CHK_Tbl_GetDate.ToString("yyyy-MM-dd HH:mm:ss")
                };
            }).ToList();

            return Json(formatted);
        }

    }
}