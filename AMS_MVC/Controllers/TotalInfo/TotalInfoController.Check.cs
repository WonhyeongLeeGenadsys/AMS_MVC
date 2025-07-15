using AMS_MVC.Models;
using AMS_MVC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS_MVC.Controllers
{
    public partial class TotalInfoController : Controller
    {
        // 5대 장비 기본정보 
        private readonly VCBBasicInfoRepository _vcbBasic = new VCBBasicInfoRepository();
        private readonly ITRBasicInfoRepository _itrBasic = new ITRBasicInfoRepository();
        private readonly DCCBBasicInfoRepository _dccbBasic = new DCCBBasicInfoRepository();
        private readonly DCCABLEBasicInfoRepository _dccableBasic = new DCCABLEBasicInfoRepository();
        private readonly SUBMODULEBasicInfoRepository _subBasic = new SUBMODULEBasicInfoRepository();

        // 5대 장비 보통점검, 정밀점검 
        private readonly VCBChkRepository vcbRepo = new VCBChkRepository();
        private readonly ITRChk1Repository itrChk1Repo = new ITRChk1Repository();
        private readonly ITRChk2Repository itrChk2Repo = new ITRChk2Repository();
        private readonly DCCBChkRepository dccbRepo = new DCCBChkRepository();
        private readonly DCCABLEChkRepository dccableRepo = new DCCABLEChkRepository();
        private readonly SUBMODULEChkRepository submoduleRepo = new SUBMODULEChkRepository();
        
        public JsonResult GetScheduleData(int year, int month)
        {
            List<VCBChk> vcbChecks;
            List<ITRChk1> itrChecks1;
            List<ITRChk2> itrChecks2;
            List<DCCBChk> dccbChecks;
            List<DCCABLEChk> dccableChecks;
            List<SUBMODULEChk> submoduleChecks;

            vcbRepo.GetTotalVCBChk(out vcbChecks);
            itrChk1Repo.GetTotalITRChk1(out itrChecks1);
            itrChk2Repo.GetTotalITRChk2(out itrChecks2);
            dccbRepo.GetTotalDCCBChk(out dccbChecks);
            dccableRepo.GetTotalDCCABLEChk(out dccableChecks);
            submoduleRepo.GetTotalSUBMODULEChk(out submoduleChecks);

            var schedules = new List<dynamic>();
            
            void AddRange<T>(IEnumerable<T> records, Func<T, DateTime?> getStart, Func<T, DateTime?> getEnd, string codeField, string type, string status)
            {
                foreach (var c in records)
                {
                    var code = (string)c.GetType().GetProperty(codeField).GetValue(c);

                    // 시리얼번호 가져오기
                    string serial;
                    switch (type)
                    {
                        case "VCB":
                            serial = _vcbBasic.GetVCBBasicInfoByCode(code)?.Serial_No;
                            break;
                        case "ITR":
                            serial = _itrBasic.GetITRBasicInfoByITRCode(code)?.Serial_No;
                            break;
                        case "DCCB":
                            serial = _dccbBasic.GetDCCBBasicInfoByCode(code)?.Serial_No;
                            break;
                        case "DCCABLE":
                            serial = _dccableBasic.GetDCCABLEBasicInfoByCode(code)?.Serial_No;
                            break;
                        case "SUBMODULE":
                            serial = _subBasic.GetSUBMODULEBasicInfoByCode(code)?.Serial_No;
                            break;
                        default:
                            serial = null;
                            break;
                    }

                    var s = getStart(c);
                    if (s.HasValue && s.Value.Year == year && s.Value.Month == month)
                    {
                        var e = getEnd(c);
                        schedules.Add(new
                        {
                            Code = (string)c.GetType().GetProperty(codeField).GetValue(c),
                            Serial_No = serial,
                            Type = type,
                            Start = s.Value.ToString("yyyy-MM-dd"),
                            End = e?.ToString("yyyy-MM-dd"),
                            Status = status
                        });
                    }
                    // overdue: End + 3개월 
                    if (getEnd(c)?.AddMonths(3) is DateTime due
                        && due.Year == year && due.Month == month)
                    {
                        schedules.Add(new
                        {
                            Code = (string)c.GetType().GetProperty(codeField).GetValue(c),
                            Serial_No = serial,
                            Type = type,
                            Start = due.ToString("yyyy-MM-dd"),
                            End = (string)null,
                            Status = "overdue"
                        });
                    }
                }
            }

            AddRange(vcbChecks, x => x.CHK_Start_Date, x => x.CHK_End_Date, "VCB_Code", "VCB", "confirmed");
            AddRange(itrChecks1, x => x.CHK1_Start_Date, x => x.CHK1_End_Date, "ITR_Code", "ITR", "confirmed");
            AddRange(itrChecks2, x => x.CHK2_Start_Date, x => x.CHK2_End_Date, "ITR_Code", "ITR ", "risk");
            AddRange(dccbChecks, x => x.CHK_Start_Date, x => x.CHK_End_Date, "DCCB_Code", "DCCB", "confirmed");
            AddRange(dccableChecks, x => x.CHK_Start_Date, x => x.CHK_End_Date, "DCCABLE_Code", "DCCABLE", "confirmed");
            AddRange(submoduleChecks, x => x.CHK_Start_Date, x => x.CHK_End_Date, "SUBMODULE_Code", "SUBMODULE", "confirmed");

            return Json(schedules, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ScheduleDetail(string type, string code)
        {
            ViewBag.Type = type;
            ViewBag.Code = code;

            // 실제 점검 이력을 담을 객체
            IEnumerable<object> items = Enumerable.Empty<object>();

            switch (type)
            {
                case "VCB":
                    vcbRepo.GetVCBChkByVCBCode(code, out var vcbList);
                    items = vcbList;
                    break;

                case "ITR":
                    itrChk1Repo.GetITRChk1ByITRCode(code, out var t1);
                    itrChk2Repo.GetITRChk2ByITRCode(code, out var t2);
                    items = t1.Cast<object>().Concat(t2);
                    break;

                case "DCCB":
                    dccbRepo.GetDCCBChkByDCCBCode(code, out var dcb);
                    items = dcb;
                    break;

                case "DCCABLE":
                    dccableRepo.GetDCCABLEChkByDCCABLECode(code, out var cab);
                    items = cab;
                    break;

                case "SUBMODULE":
                    submoduleRepo.GetSUBMODULEChkBySUBMODULECode(code, out var sm);
                    items = sm;
                    break;
            }

            return View("~/Views/TotalInfo/ScheduleDetail.cshtml", items);
        }
    }
}