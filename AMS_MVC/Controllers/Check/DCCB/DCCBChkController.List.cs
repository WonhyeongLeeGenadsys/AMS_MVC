using System.Web.Mvc;

namespace AMS_MVC
{
    public partial class DCCBChkController : Controller
    {
        public ActionResult DCCBChkList(string DCCB_Code)
        {
            var basicInfo = dccbBasicInfoRepository.GetDCCBBasicInfoByCode(DCCB_Code);
            ViewBag.DCCBCode = DCCB_Code;
            ViewBag.SerialNo = basicInfo?.Serial_No ?? string.Empty;
            ViewBag.Name = basicInfo?.Name ?? string.Empty;
            return View("~/Views/Check/DCCB/DCCBChkList.cshtml");
        }

        public ActionResult DCCBChkTotalList()
        {
            return View("~/Views/Check/Total/DCCBChkTotalList.cshtml");
        }

        [HttpPost]
        public ActionResult GetDCCBChkListData(string dccbCode)
        {
            return CheckListSummaryJson.Create("DCCB", dccbCode, "DCCB_Code");
        }

        [HttpPost]
        public ActionResult GetTotalDCCBChkListData()
        {
            return CheckListSummaryJson.Create("DCCB", null, "DCCB_Code");
        }
    }
}
