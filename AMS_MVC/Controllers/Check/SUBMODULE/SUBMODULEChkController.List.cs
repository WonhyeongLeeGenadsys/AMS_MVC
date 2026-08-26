using System.Web.Mvc;

namespace AMS_MVC
{
    public partial class SUBMODULEChkController : Controller
    {
        public ActionResult SUBMODULEChkList(string SUBMODULE_Code)
        {
            var basicInfo = submoduleBasicInfoRepository.GetSUBMODULEBasicInfoByCode(SUBMODULE_Code);
            ViewBag.SUBMODULECode = SUBMODULE_Code;
            ViewBag.SerialNo = basicInfo?.Serial_No ?? string.Empty;
            ViewBag.Name = basicInfo?.Name ?? string.Empty;
            return View("~/Views/Check/SUBMODULE/SUBMODULEChkList.cshtml");
        }

        public ActionResult SUBMODULEChkTotalList()
        {
            return View("~/Views/Check/Total/SUBMODULEChkTotalList.cshtml");
        }

        [HttpPost]
        public ActionResult GetSUBMODULEChkListData(string submoduleCode)
        {
            return CheckListSummaryJson.Create("SUBMODULE", submoduleCode, "SUBMODULE_Code");
        }

        [HttpPost]
        public ActionResult GetTotalSUBMODULEChkListData()
        {
            return CheckListSummaryJson.Create("SUBMODULE", null, "SUBMODULE_Code");
        }
    }
}
