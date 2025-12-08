// Controllers/Check/ITRChkController.TotalList.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class ITRChkController
    {
        // GET: /Check/ITRChk/TotalList?type=1
        public ActionResult ITRChkTotalList(int type = 1)
        {
            if (type == 1)
            {
                ViewBag.ActiveSubMenu = "ITRRegular";   // 보통점검
                return View("~/Views/Check/Total/ITRChk1TotalList.cshtml");
            }
            else
            {
                ViewBag.ActiveSubMenu = "ITRPrecision"; // 정밀점검
                return View("~/Views/Check/Total/ITRChk2TotalList.cshtml");
            }
        }


        [HttpPost]
        public ActionResult GetTotalITRChkListData(int type = 1)
        {
            try
            {
                _basicRepo.GetAllITRBasicInfoRepo(out var basics);
                var basicMap = basics.ToDictionary(b => b.ITR_Code, b => b);

                if (type == 1)
                {
                    var repoResult = _chk1Repo.GetTotalITRChk1(out List<ITRChk1> data);
                    if (!repoResult.IsSuccess)
                        return Json(new { success = false, message = repoResult.Message });

                    var formattedData = data.Select(item =>
                    {
                        basicMap.TryGetValue(item.ITR_Code, out var basic);
                        return new
                        {
                            item.Tbl_Idx,
                            item.ITR_Code,
                            Name = basic?.Name ?? "",
                            Serial_No = basic?.Serial_No ?? "",
                            item.CHK1_Gongsa_Name,
                            item.CHK1_Weather,
                            item.CHK1_Temp,
                            item.CHK1_Hum,
                            item.CHK1_Company,
                            item.CHK1_Worker,
                            item.CHK1_Manager,
                            item.CHK1_Urgent_No,
                            item.CHK1_Type,
                            CHK1_Start_Date = item.CHK1_Start_Date?.ToString("yy.MM.dd"),
                            CHK1_End_Date = item.CHK1_End_Date?.ToString("yy.MM.dd"),
                            item.FoldingFunction,
                            item.CHK1_Writer,
                            CHK1_Tbl_GetDate = item.CHK1_Tbl_GetDate.ToString("yy.MM.dd HH:mm")
                        };
                    }).ToList();

                    return Json(formattedData);
                }
                else
                {
                    var repoResult = _chk2Repo.GetTotalITRChk2(out List<ITRChk2> data);
                    if (!repoResult.IsSuccess)
                        return Json(new { success = false, message = repoResult.Message });

                    var formattedData = data.Select(item =>
                    {
                        basicMap.TryGetValue(item.ITR_Code, out var basic);
                        return new
                        {
                            item.Tbl_Idx,
                            item.ITR_Code,
                            Name = basic?.Name ?? "",
                            Serial_No = basic?.Serial_No ?? "",
                            item.CHK2_Gongsa_Name,
                            item.CHK2_Weather,
                            item.CHK2_Temp,
                            item.CHK2_Hum,
                            item.CHK2_Company,
                            item.CHK2_Worker,
                            item.CHK2_Manager,
                            item.CHK2_Urgent_No,
                            item.CHK2_Type,
                            CHK2_Start_Date = item.CHK2_Start_Date?.ToString("yy.MM.dd"),
                            CHK2_End_Date = item.CHK2_End_Date?.ToString("yy.MM.dd"),
                            item.CHK2_Computerized_Price,
                            item.CHK2_Water_Content,
                            item.CHK2_Furfural,
                            item.CHK2_Excitation_Current,
                            item.CHK2_Short_Current,
                            item.CHK2_Voltage_Ratio,
                            item.CHK2_PD,
                            item.FoldingFunction,
                            item.CHK2_Writer,
                            CHK2_Tbl_GetDate = item.CHK2_Tbl_GetDate.ToString("yy.MM.dd HH:mm")
                        };
                    }).ToList();

                    return Json(formattedData);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("ITRChkController.GetTotalListData", ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}
