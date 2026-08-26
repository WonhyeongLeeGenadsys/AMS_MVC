using System.Web.Mvc;

namespace AMS_MVC
{
    public partial class ITRChkController
    {
        public ActionResult ITRChkTotalList(int type = 1)
        {
            if (type == 1)
            {
                ViewBag.ActiveSubMenu = "ITRRegular";
                return View("~/Views/Check/Total/ITRChk1TotalList.cshtml");
            }

            ViewBag.ActiveSubMenu = "ITRPrecision";
            return View("~/Views/Check/Total/ITRChk2TotalList.cshtml");
        }

        [HttpPost]
        public ActionResult GetTotalITRChkListData(int type = 1)
        {
            return type == 1
                ? CheckListSummaryJson.Create("ITR1", null, "ITR_Code", "CHK1")
                : CheckListSummaryJson.Create("ITR2", null, "ITR_Code", "CHK2");
        }
    }
}
