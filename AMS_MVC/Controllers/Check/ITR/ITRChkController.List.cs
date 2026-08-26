using System.Web.Mvc;

namespace AMS_MVC
{
    public partial class ITRChkController
    {
        public ActionResult ITRChkList(string ITR_Code, int type = 1)
        {
            var basic = _basicRepo.GetITRBasicInfoByITRCode(ITR_Code);
            ViewBag.SerialNo = basic?.Serial_No ?? string.Empty;
            ViewBag.Name = basic?.Name ?? string.Empty;
            ViewBag.ITR_Code = ITR_Code;

            var view = type == 1
                ? "~/Views/Check/ITR/ITRChk1List.cshtml"
                : "~/Views/Check/ITR/ITRChk2List.cshtml";
            return View(view);
        }

        [HttpPost]
        public ActionResult GetITRChkListData(string itrCode, int type = 1)
        {
            return type == 1
                ? CheckListSummaryJson.Create("ITR1", itrCode, "ITR_Code", "CHK1")
                : CheckListSummaryJson.Create("ITR2", itrCode, "ITR_Code", "CHK2");
        }
    }
}
