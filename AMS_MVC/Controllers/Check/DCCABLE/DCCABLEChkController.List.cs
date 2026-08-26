using System.Web.Mvc;

namespace AMS_MVC
{
    public partial class DCCABLEChkController : Controller
    {
        public ActionResult DCCABLEChkList(string DCCABLE_Code)
        {
            var basicInfo = dccableBasicInfoRepository.GetDCCABLEBasicInfoByCode(DCCABLE_Code);
            ViewBag.DCCABLECode = DCCABLE_Code;
            ViewBag.SerialNo = basicInfo?.Serial_No ?? string.Empty;
            ViewBag.Name = basicInfo?.Name ?? string.Empty;
            return View("~/Views/Check/DCCABLE/DCCABLEChkList.cshtml");
        }

        public ActionResult DCCABLEChkTotalList()
        {
            return View("~/Views/Check/Total/DCCABLEChkTotalList.cshtml");
        }

        [HttpPost]
        public ActionResult GetDCCABLEChkListData(string dccableCode)
        {
            return CheckListSummaryJson.Create("DCCABLE", dccableCode, "DCCABLE_Code");
        }

        [HttpPost]
        public ActionResult GetTotalDCCABLEChkListData()
        {
            return CheckListSummaryJson.Create("DCCABLE", null, "DCCABLE_Code");
        }
    }
}
