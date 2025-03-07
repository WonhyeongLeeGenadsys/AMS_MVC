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
        private readonly VCBChkRepository vcbRepo = new VCBChkRepository();
        private readonly ITRChk1Repository itrChk1Repo = new ITRChk1Repository();
        private readonly ITRChk2Repository itrChk2Repo = new ITRChk2Repository();

        // GET: 점검 일정 API
        public JsonResult GetScheduleData(int year, int month)
        {
            List<VCBChk> vcbChecks;
            List<ITRChk1> itrChecks1;
            List<ITRChk2> itrChecks2;

            vcbRepo.GetTotalVCBChk(out vcbChecks);
            itrChk1Repo.GetTotalITRChk1(out itrChecks1);
            itrChk2Repo.GetTotalITRChk2(out itrChecks2);

            var schedules = new List<object>();

            schedules.AddRange(vcbChecks
                .Where(c => c.CHK_Start_Date.HasValue && c.CHK_Start_Date.Value.Year == year && c.CHK_Start_Date.Value.Month == month)
                .Select(c => new
                {
                    Code = c.VCB_Code,
                    Type = "VCB",
                    Start = c.CHK_Start_Date.Value.ToString("yyyy-MM-dd"),
                    End = c.CHK_End_Date?.ToString("yyyy-MM-dd"),
                    Status = "confirmed"
                }));

            schedules.AddRange(itrChecks1
                .Where(c => c.CHK1_Start_Date.HasValue && c.CHK1_Start_Date.Value.Year == year && c.CHK1_Start_Date.Value.Month == month)
                .Select(c => new
                {
                    Code = c.ITR_Code,
                    Type = "Interface TR",
                    Start = c.CHK1_Start_Date.Value.ToString("yyyy-MM-dd"),
                    End = c.CHK1_End_Date?.ToString("yyyy-MM-dd"),
                    Status = "risk"
                }));

            schedules.AddRange(itrChecks2
                .Where(c => c.CHK2_Start_Date.HasValue && c.CHK2_Start_Date.Value.Year == year && c.CHK2_Start_Date.Value.Month == month)
                .Select(c => new
                {
                    Code = c.ITR_Code,
                    Type = "Interface TR",
                    Start = c.CHK2_Start_Date.Value.ToString("yyyy-MM-dd"),
                    End = c.CHK2_End_Date?.ToString("yyyy-MM-dd"),
                    Status = "risk"
                }));

            return Json(schedules, JsonRequestBehavior.AllowGet);
        }
    }
}