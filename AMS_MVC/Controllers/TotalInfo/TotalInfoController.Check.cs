
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
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
        
        public JsonResult GetScheduleData(int? year = null, int? month = null)
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

            var schedules = new List<InspectionScheduleItem>();
            var equipmentMetadata = new Dictionary<string, InspectionScheduleMetadata>(StringComparer.OrdinalIgnoreCase);
            
            void AddRange<T>(
                IEnumerable<T> records,
                Func<T, DateTime?> getStart,
                Func<T, DateTime?> getEnd,
                string codeField,
                string type,
                string inspectionType)
            {
                foreach (var c in records ?? Enumerable.Empty<T>())
                {
                    var codeProperty = c.GetType().GetProperty(codeField);
                    var code = codeProperty?.GetValue(c) as string;
                    if (string.IsNullOrWhiteSpace(code))
                    {
                        continue;
                    }

                    var metadataKey = type + "|" + code;
                    if (!equipmentMetadata.TryGetValue(metadataKey, out var metadata))
                    {
                        string serial;
                        string equipmentName;
                        int regularCycleMonths;
                        int precisionCycleMonths;
                        switch (type)
                        {
                            case "VCB":
                                var vcb = _vcbBasic.GetVCBBasicInfoByCode(code);
                                serial = vcb?.Serial_No;
                                equipmentName = vcb?.Name;
                                regularCycleMonths = GetCycleMonths(vcb?.Regular_Inspection_Cycle_Months, 3);
                                precisionCycleMonths = GetCycleMonths(vcb?.Precision_Inspection_Cycle_Months, 12);
                                break;
                            case "ITR":
                                var itr = _itrBasic.GetITRBasicInfoByITRCode(code);
                                serial = itr?.Serial_No;
                                equipmentName = itr?.Name;
                                regularCycleMonths = GetCycleMonths(itr?.Regular_Inspection_Cycle_Months, 3);
                                precisionCycleMonths = GetCycleMonths(itr?.Precision_Inspection_Cycle_Months, 12);
                                break;
                            case "DCCB":
                                var dccb = _dccbBasic.GetDCCBBasicInfoByCode(code);
                                serial = dccb?.Serial_No;
                                equipmentName = dccb?.Name;
                                regularCycleMonths = GetCycleMonths(dccb?.Regular_Inspection_Cycle_Months, 3);
                                precisionCycleMonths = GetCycleMonths(dccb?.Precision_Inspection_Cycle_Months, 12);
                                break;
                            case "DCCABLE":
                                var dccable = _dccableBasic.GetDCCABLEBasicInfoByCode(code);
                                serial = dccable?.Serial_No;
                                equipmentName = dccable?.Name;
                                regularCycleMonths = GetCycleMonths(dccable?.Regular_Inspection_Cycle_Months, 3);
                                precisionCycleMonths = GetCycleMonths(dccable?.Precision_Inspection_Cycle_Months, 12);
                                break;
                            case "SUBMODULE":
                                var submodule = _subBasic.GetSUBMODULEBasicInfoByCode(code);
                                serial = submodule?.Serial_No;
                                equipmentName = submodule?.Name;
                                regularCycleMonths = GetCycleMonths(submodule?.Regular_Inspection_Cycle_Months, 3);
                                precisionCycleMonths = GetCycleMonths(submodule?.Precision_Inspection_Cycle_Months, 12);
                                break;
                            default:
                                serial = null;
                                equipmentName = null;
                                regularCycleMonths = 3;
                                precisionCycleMonths = 12;
                                break;
                        }

                        metadata = new InspectionScheduleMetadata
                        {
                            SerialNo = serial,
                            EquipmentName = equipmentName,
                            RegularCycleMonths = regularCycleMonths,
                            PrecisionCycleMonths = precisionCycleMonths
                        };
                        equipmentMetadata[metadataKey] = metadata;
                    }

                    var start = getStart(c);
                    var end = getEnd(c);
                    var inspectionDate = end ?? start;
                    if (!inspectionDate.HasValue)
                    {
                        continue;
                    }

                    int cycleMonths = inspectionType == "정밀점검"
                        ? metadata.PrecisionCycleMonths
                        : metadata.RegularCycleMonths;
                    var dueDate = inspectionDate.Value.AddMonths(cycleMonths);
                    schedules.Add(new InspectionScheduleItem
                    {
                        Code = code,
                        Serial_No = metadata.SerialNo,
                        Type = type,
                        EquipmentName = string.IsNullOrWhiteSpace(metadata.EquipmentName) ? type : metadata.EquipmentName,
                        Category = type == "VCB" || type == "ITR" ? "AC" : "DC",
                        InspectionType = inspectionType,
                        Start = start?.ToString("yyyy-MM-dd"),
                        End = end?.ToString("yyyy-MM-dd"),
                        DueDate = dueDate.ToString("yyyy-MM-dd"),
                        Status = inspectionType == "정밀점검" ? "risk" : "confirmed",
                        ScheduleStatus = dueDate.Date < DateTime.Today ? "overdue" : "scheduled"
                    });
                }
            }

            AddRange(vcbChecks, x => x.CHK_Start_Date, x => x.CHK_End_Date, "VCB_Code", "VCB", "보통점검");
            AddRange(itrChecks1, x => x.CHK1_Start_Date, x => x.CHK1_End_Date, "ITR_Code", "ITR", "보통점검");
            AddRange(itrChecks2, x => x.CHK2_Start_Date, x => x.CHK2_End_Date, "ITR_Code", "ITR", "정밀점검");
            AddRange(dccbChecks, x => x.CHK_Start_Date, x => x.CHK_End_Date, "DCCB_Code", "DCCB", "보통점검");
            AddRange(dccableChecks, x => x.CHK_Start_Date, x => x.CHK_End_Date, "DCCABLE_Code", "DCCABLE", "보통점검");
            AddRange(submoduleChecks, x => x.CHK_Start_Date, x => x.CHK_End_Date, "SUBMODULE_Code", "SUBMODULE", "보통점검");

            // 일정 화면에는 자산/점검유형별 가장 최근 일정 중 오늘부터 3개월 이내의 항목만 표시한다.
            DateTime today = DateTime.Today;
            DateTime scheduleLimit = today.AddMonths(3);
            var latestSchedules = schedules
                .GroupBy(x => new { x.Code, x.InspectionType })
                .Select(g => g.OrderByDescending(x => x.DueDate).First())
                .Where(x =>
                {
                    DateTime dueDate;
                    return DateTime.TryParse(x.DueDate, out dueDate)
                        && dueDate.Date >= today
                        && dueDate.Date <= scheduleLimit;
                })
                .OrderBy(x => x.DueDate)
                .ThenBy(x => x.Code)
                .Select((x, index) =>
                {
                    x.Priority = index + 1;
                    return x;
                })
                .ToList();

            return Json(latestSchedules, JsonRequestBehavior.AllowGet);
        }

        private static int GetCycleMonths(int? configuredMonths, int defaultMonths)
        {
            return configuredMonths.HasValue && configuredMonths.Value > 0
                ? configuredMonths.Value
                : defaultMonths;
        }

        private sealed class InspectionScheduleMetadata
        {
            public string SerialNo { get; set; }
            public string EquipmentName { get; set; }
            public int RegularCycleMonths { get; set; }
            public int PrecisionCycleMonths { get; set; }
        }

        private sealed class InspectionScheduleItem
        {
            public int Priority { get; set; }
            public string Code { get; set; }
            public string Serial_No { get; set; }
            public string Type { get; set; }
            public string EquipmentName { get; set; }
            public string Category { get; set; }
            public string InspectionType { get; set; }
            public string Start { get; set; }
            public string End { get; set; }
            public string DueDate { get; set; }
            public string Status { get; set; }
            public string ScheduleStatus { get; set; }
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
