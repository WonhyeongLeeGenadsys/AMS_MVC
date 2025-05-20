using AMS_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers.Check
{
    public partial class NGRChkController : Controller
    {
        public ActionResult NGRChkList(string NGR_Code)
        {
            var basicInfo = ngrBasicInfoRepository.GetNGRBasicInfoByCode(NGR_Code);
            ViewBag.NGRCode = NGR_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Check/etc/Grounding/NGR/NGRChkList.cshtml");
        }

        public ActionResult NGRChkTotalList()
        {
            return View("~/Views/Check/Total/etc/Grounding/NGRChkTotalList.cshtml");
        }

        [HttpPost]
        public ActionResult GetNGRChkListData(string ngrCode)
        {
            try
            {
                LogHelper.WriteLog("NGRChkController.List", "GetNGRChkListData 실행");
                LogHelper.WriteLog("ngrCode 조회 : ",$"{ngrCode}");

                List<NGRChk> ngrChks = new List<NGRChk>();
                var repoResult = ngrChkRepository.GetNGRChkByNGRCode(ngrCode, out ngrChks);
                if (repoResult.IsSuccess)
                {
                    var formattedData = ngrChks.Select(item => new
                    {
                        item.Tbl_Idx,
                        item.NGR_Code,
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


                    LogHelper.WriteLog("NGRChkController.List", $"조회된 데이터: {ngrChks.Count}건");
                    return Json(formattedData);
                }
                else
                {
                    LogHelper.WriteLog("NGRChkController.List", "NGR 보통점검 데이터 로드 실패");
                    return Json(new { success = false, message = "NGR 보통점검 데이터 로드 실패" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("NGRChkController.List", $"GetNGRListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalNGRChkListData()
        {
            // 1) 전체 점검 데이터 조회
            var repoResult = ngrChkRepository.GetTotalNGRChk(out var ngrChks);
            if (!repoResult.IsSuccess)
                return Json(new { success = false, message = repoResult.Message });

            // 2) NGR 기본정보 전체 조회 → 코드별 매핑용 딕셔너리 생성
            ngrBasicInfoRepository.GetAllNGRBasicInfoRepo(out var basics);
            var basicMap = basics.ToDictionary(b => b.NGR_Code, b => b);

            // 3) JSON 응답용 객체에 Name, Serial_No 및 모든 CHK_* 필드를 한 번에 담기
            var formatted = ngrChks.Select(item =>
            {
                basicMap.TryGetValue(item.NGR_Code, out var basic);
                return new
                {
                    item.Tbl_Idx,
                    item.NGR_Code,

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