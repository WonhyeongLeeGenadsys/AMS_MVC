using System.Web.Mvc;

namespace AMS_MVC
{
    public partial class VCBChkController : Controller
    {
        public ActionResult VCBChkList(string VCB_Code)
        {
            var basicInfo = vcbBasicInfoRepository.GetVCBBasicInfoByCode(VCB_Code);
            ViewBag.VCBCode = VCB_Code;
            ViewBag.SerialNo = basicInfo?.Serial_No ?? string.Empty;
            ViewBag.Name = basicInfo?.Name ?? string.Empty;
            return View("~/Views/Check/VCB/VCBChkList.cshtml");
        }

        public ActionResult VCBChkTotalList()
        {
            return View("~/Views/Check/Total/VCBChkTotalList.cshtml");
        }

        [HttpPost]
        public ActionResult GetVCBChkListData(string vcbCode)
        {
            return CheckListSummaryJson.Create("VCB", vcbCode, "VCB_Code");
        }

        [HttpPost]
        public ActionResult GetTotalVCBChkListData()
        {
            return CheckListSummaryJson.Create("VCB", null, "VCB_Code");
        }
    }
}
