using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class VCBChkController : Controller
    {
        public ActionResult VCBChkList(string VCB_Code)
        {
            var basicInfo = vcbBasicInfoRepository.GetVCBBasicInfoByCode(VCB_Code);
            ViewBag.VCBCode = VCB_Code;
            ViewBag.SerialNo = basicInfo != null ? basicInfo.Serial_No : "";
            ViewBag.Name = basicInfo != null ? basicInfo.Name : "";
            return View("~/Views/Check/VCB/VCBChkList.cshtml");
        }

        public ActionResult VCBChkTotalList()
        {
            return View("~/Views/Check/Total/VCBChkTotalList.cshtml");
        }

        [HttpPost]
        public ActionResult GetVCBChkListData(string vcbCode)
        {
            try
            {
                LogHelper.WriteLog("VCBChkController.List", "GetVCBChkListData 실행");
                LogHelper.WriteLog("vcbCode 조회 : ",$"{vcbCode}");

                List<VCBChk> vcbChks = new List<VCBChk>();
                var repoResult = vcbChkRepository.GetVCBChkByVCBCode(vcbCode, out vcbChks);
                if (repoResult.IsSuccess)
                {
                    var formattedData = vcbChks.Select(item => new
                    {
                        item.Tbl_Idx,
                        item.VCB_Code,
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
                        item.CHK_Loc,
                        item.CHK_Chuk_Loc,
                        item.CHK_Con_Status,
                        item.CHK_Bolt_Nut_Status,
                        item.CHK_Contact_Volume,
                        item.CHK_Vacuum_Degree,
                        item.CHK_Coil_A,
                        item.CHK_Contact_R,
                        item.CHK_Main_Circuit,
                        item.CHK_Control_Circuit,
                        item.CHK_Input_Time,
                        item.CHK_Open_Time,
                        item.CHK_3_Phase_Open_Gap,
                        item.CHK_Chattering_Time,
                        item.CHK_O_C_O,
                        item.CHK_Operate_Time,
                        item.CHK_OC_Test,
                        item.CHK_Indicator,
                        item.CHK_VCB_Count,
                        item.CHK_Cutoff_Count,
                        item.CHK_A_Rate,
                        item.CHK_Short_A_Rate,
                        item.CHK_Writer,
                    }).ToList();


                    LogHelper.WriteLog("VCBChkController.List", $"조회된 데이터: {vcbChks.Count}건");
                    return Json(formattedData);
                }
                else
                {
                    LogHelper.WriteLog("VCBChkController.List", "VCB 보통점검 데이터 로드 실패");
                    return Json(new { success = false, message = "VCB 보통점검 데이터 로드 실패" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("VCBChkController.List", $"GetVCBListData 실패: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTotalVCBChkListData()
        {
            //  전체 점검 데이터 조회
            var repoResult = vcbChkRepository.GetTotalVCBChk(out var vcbChks);
            if (!repoResult.IsSuccess)
                return Json(new { success = false, message = repoResult.Message });

            //  VCB 기본정보 전체 조회 → 코드별 매핑용 딕셔너리 생성
            vcbBasicInfoRepository.GetAllVCBBasicInfoRepo(out var basics);
            var basicMap = basics.ToDictionary(b => b.VCB_Code, b => b);

            var formatted = vcbChks.Select(item =>
            {
                basicMap.TryGetValue(item.VCB_Code, out var basic);
                return new
                {
                    item.Tbl_Idx,
                    item.VCB_Code,

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

                    item.CHK_Loc,
                    item.CHK_Chuk_Loc,
                    item.CHK_Con_Status,
                    item.CHK_Bolt_Nut_Status,

                    item.CHK_Contact_Volume,
                    item.CHK_Vacuum_Degree,
                    item.CHK_Coil_A,
                    item.CHK_Contact_R,
                    item.CHK_Main_Circuit,
                    item.CHK_Control_Circuit,

                    item.CHK_Input_Time,
                    item.CHK_Open_Time,
                    item.CHK_3_Phase_Open_Gap,
                    item.CHK_Chattering_Time,
                    item.CHK_O_C_O,
                    item.CHK_Operate_Time,
                    item.CHK_OC_Test,
                    item.CHK_Indicator,

                    item.CHK_VCB_Count,
                    item.CHK_Cutoff_Count,
                    item.CHK_A_Rate,
                    item.CHK_Short_A_Rate,

                    item.CHK_Writer,

                    item.CHK_ContactWearPercent,
                    item.CHK_VacuumLeakCurrent,
                    item.CHK_ContactResistance,
                    item.CHK_InsulationResistance,
                    item.CHK_HotSpot,
                    item.CHK_PdPatternValue,
                    item.CHK_MotorCurrent,
                    item.CHK_AccumShortCircuitCurrent,
                    item.CHK_ShortCircuitCount,
                    item.CHK_OperationCount,
                    item.CHK_OpenCloseTime,
                    item.CHK_VisualCheck,

                    // 알고리즘 점수
                    item.FoldingFunction,

                    // DB 반영 일시
                    CHK_Tbl_GetDate = item.CHK_Tbl_GetDate.ToString("yyyy-MM-dd HH:mm:ss")
                };
            }).ToList();

            return Json(formatted);
        }

    }
}